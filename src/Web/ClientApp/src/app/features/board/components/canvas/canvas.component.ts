import {
  AfterViewInit, Component, ElementRef, Input, ViewChild, OnDestroy
} from '@angular/core';
import Konva from 'konva';
import { v4 as uuid } from 'uuid';
import { ToolService } from 'src/app/core/services/tool.service';
import * as Y from 'yjs';
import { Awareness } from 'y-protocols/awareness';
type LogEntry = { action: string; shape: any; attrs?: any };

@Component({
  selector: 'app-canvas',
  standalone: true,
  templateUrl: './canvas.component.html',
  styleUrls: ['./canvas.component.scss']
})
export class CanvasComponent implements AfterViewInit, OnDestroy {
  @ViewChild('stageContainer', { static: true })
  container!: ElementRef<HTMLDivElement>;

  private stage!: Konva.Stage;
  private layer!: Konva.Layer;
  private transformer = new Konva.Transformer();

  private history: string[] = [];          // JSON snapshots for undo
  private drawingShape: Konva.Shape | null = null;
  private startPos: Konva.Vector2d | null = null;

  private ydoc!: Y.Doc;
  private awareness!: Awareness;
  private konvaMap!: Y.Map<any>;

  @Input() set zoom(z: number) {
    if (this.stage) {
      this.stage.scale({ x: z, y: z });
      this.stage.batchDraw();
    }
  }

  constructor(public toolSvc: ToolService) { }

  ngAfterViewInit(): void {
    // Get shared Yjs doc and awareness from ToolService
    this.ydoc = this.toolSvc.getYDoc();
    this.awareness = this.toolSvc.getAwareness();

    // Get or create Konva root map inside Yjs doc
    this.konvaMap = this.ydoc.getMap('konva');

    this.initCanvas();

    this.subscribeToToolChanges();

    this.subscribeToPresenceChanges();
  }

  ngOnDestroy(): void {
    // Clean up listeners if needed
    window.removeEventListener('undo-canvas', this.undo);
    window.removeEventListener('clear-canvas', this.clear);
  }

  private initCanvas(): void {
    this.stage = new Konva.Stage({
      container: this.container.nativeElement,
      width: this.container.nativeElement.offsetWidth,
      height: this.container.nativeElement.offsetHeight
    });

    this.layer = new Konva.Layer();
    this.stage.add(this.layer);
    this.layer.add(this.transformer);

    // Bind Konva layer ↔ Yjs map for collaborative sync
    this.bindKonvaToYjs();

    // Bind drawing & interaction events
    this.bindEvents();

    this.saveState();

    // Undo and Clear global event listeners
    window.addEventListener('undo-canvas', () => this.undo());
    window.addEventListener('clear-canvas', () => this.clear());
  }

  private bindKonvaToYjs() {
    // Apply remote updates to Konva layer
    this.konvaMap.observeDeep(() => {
      this.layer.destroyChildren();

      const tree = this.konvaMap.get('root') || [];
      tree.forEach(nodeObj => this.layer.add(Konva.Node.create(nodeObj)));

      this.layer.draw();
    });

    // Push local changes to Yjs map
    const push = () => {
      const json = this.stage.toJSON();
      this.konvaMap.set('root', JSON.parse(json).children[0].children);
    };

    this.layer.on('add remove dragend transformend', push);
  }

  private subscribeToToolChanges(): void {
    this.toolSvc.getTool().subscribe(tool => {
      console.log('Tool changed:', tool);
      // You can update cursor styles or UI here based on the current tool
    });
  }

  private subscribeToPresenceChanges(): void {
    this.toolSvc.getPresence().subscribe(users => {
      // Optionally show other users' cursors or presence indicators on canvas
      // console.log('Presence users:', users);
    });
  }

  private bindEvents(): void {
    this.stage.on('mousedown', e => this.onStageMouseDown(e));
    this.stage.on('mousemove', e => this.onStageMouseMove(e));
    this.stage.on('mouseup', () => this.onStageMouseUp());

    this.stage.on('wheel', e => {
      e.evt.preventDefault();
      const scaleBy = 1.1;
      const oldScale = this.stage.scaleX();
      const pointer = this.stage.getPointerPosition()!;
      const mousePointTo = {
        x: (pointer.x - this.stage.x()) / oldScale,
        y: (pointer.y - this.stage.y()) / oldScale
      };

      const newScale = e.evt.deltaY < 0 ? oldScale * scaleBy : oldScale / scaleBy;
      this.stage.scale({ x: newScale, y: newScale });

      const newPos = {
        x: pointer.x - mousePointTo.x * newScale,
        y: pointer.y - mousePointTo.y * newScale
      };
      this.stage.position(newPos);
      this.stage.batchDraw();
    });
  }

  private onStageMouseDown(e: Konva.KonvaEventObject<MouseEvent>): void {
    if (e.target !== this.stage) return;

    const pos = this.stage.getPointerPosition()!;
    this.startPos = { x: pos.x, y: pos.y };
    const tool = this.toolSvc.currentTool;

    switch (tool) {
      case 'pen':
        this.drawingShape = new Konva.Line({
          id: uuid(),
          points: [pos.x, pos.y],
          stroke: '#000',
          strokeWidth: 2,
          lineCap: 'round',
          lineJoin: 'round'
        });
        break;

      case 'rect':
        this.drawingShape = new Konva.Rect({
          id: uuid(), x: pos.x, y: pos.y, width: 0, height: 0, fill: '#7ed6df'
        });
        break;

      case 'circle':
        this.drawingShape = new Konva.Circle({
          id: uuid(), x: pos.x, y: pos.y, radius: 0, fill: '#ff7979'
        });
        break;

      case 'text':
        const text = prompt('Enter text:', 'Hello Konva') || 'Text';
        const txt = new Konva.Text({
          id: uuid(), x: pos.x, y: pos.y, text, fontSize: 20, fill: '#130f40'
        });
        this.addShape(txt);
        return; // no drag, single click

      case 'eraser':
        return; // handled in mouse move

      case 'select':
        this.transformer.nodes([]);
        return;
    }

    if (this.drawingShape) {
      this.layer.add(this.drawingShape);
    }
  }

  private onStageMouseMove(e: Konva.KonvaEventObject<MouseEvent>): void {
    const pos = this.stage.getPointerPosition();
    if (!pos) return;

    // Notify cursor movement to other users
    this.toolSvc.moveCursor(pos.x, pos.y);

    if (this.toolSvc.currentTool === 'eraser') {
      const hit = this.layer.getIntersection(pos);
      if (hit && hit.id() !== this.transformer.id()) {
        this.log({ action: 'delete', shape: hit.toObject() });
        hit.destroy();
        this.layer.batchDraw();
      }
      return;
    }

    if (!this.drawingShape || !this.startPos) return;

    switch (this.toolSvc.currentTool) {
      case 'pen':
        (this.drawingShape as Konva.Line).points(
          (this.drawingShape as Konva.Line).points().concat([pos.x, pos.y])
        );
        break;

      case 'rect':
        const r = this.drawingShape as Konva.Rect;
        r.width(Math.abs(pos.x - this.startPos.x));
        r.height(Math.abs(pos.y - this.startPos.y));
        r.x(Math.min(pos.x, this.startPos.x));
        r.y(Math.min(pos.y, this.startPos.y));
        break;

      case 'circle':
        const c = this.drawingShape as Konva.Circle;
        c.radius(Math.sqrt(Math.pow(pos.x - this.startPos.x, 2) +
          Math.pow(pos.y - this.startPos.y, 2)));
        break;
    }

    this.stage.batchDraw();
  }

  private onStageMouseUp(): void {
    if (this.drawingShape) {
      this.drawingShape.draggable(true);
      this.addShape(this.drawingShape);
      this.drawingShape = null;
    }
    this.startPos = null;
  }

  private addShape(shape: Konva.Shape): void {
    this.log({ action: 'add', shape: shape.toObject() });
    this.saveState();
    shape.on('dragend transformend', () => {
      this.log({ action: 'update', shape: shape.toObject() });
      this.saveState();
    });
    this.layer.draw();
  }

  private log(entry: LogEntry): void {
    console.table([entry]);
  }

  private saveState(): void {
    this.history.push(this.layer.toJSON());
  }

  private undo = (): void => {
    if (this.history.length < 2) return;
    this.history.pop(); // remove current state
    const json = this.history[this.history.length - 1];
    this.layer.destroyChildren();
    this.layer = Konva.Node.create(json).getLayer()!;
    this.stage.add(this.layer);
    this.layer.add(this.transformer);
    this.stage.batchDraw();
  };

  private clear = (): void => {
    this.layer.destroyChildren();
    this.layer.add(this.transformer);
    this.saveState();
    this.stage.batchDraw();
    this.log({ action: 'clear', shape: null });
  };
}
