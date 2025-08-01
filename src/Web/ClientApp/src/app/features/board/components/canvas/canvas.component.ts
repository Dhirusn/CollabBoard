import {
  AfterViewInit, Component, ElementRef, Input, ViewChild
} from '@angular/core';
import Konva from 'konva';
import { v4 as uuid } from 'uuid';
import { ToolService } from 'src/app/core/services/tool.service';
import { SignalRProvider } from 'src/app/core/services/y-signalr';
import { HubConnectionBuilder } from '@microsoft/signalr';
import * as Y from 'yjs';
import { SignalRHubService } from 'src/app/core/services/signalr-hub.service';

type LogEntry = { action: string; shape: any; attrs?: any };

@Component({
  selector: 'app-canvas',
  standalone: true,
  imports: [],
  templateUrl: './canvas.component.html',
  styleUrls: ['./canvas.component.scss']
})
export class CanvasComponent implements AfterViewInit {
  @ViewChild('stageContainer', { static: true })
  container!: ElementRef<HTMLDivElement>;

  private stage!: Konva.Stage;
  private layer = new Konva.Layer();
  private history: string[] = [];          // JSON snapshots for undo
  private transformer = new Konva.Transformer();

  @Input() set zoom(z: number) {
    if (this.stage) {
      this.stage.scale({ x: z, y: z });
      this.stage.batchDraw();
    }
  }

  constructor(public toolSvc: ToolService) { }

  ngAfterViewInit(): void {
    this.initCollaboration();
    this.stage = new Konva.Stage({
      container: this.container.nativeElement,
      width: this.container.nativeElement.offsetWidth,
      height: this.container.nativeElement.offsetHeight
    });

    this.stage.add(this.layer);
    this.layer.add(this.transformer);

    this.bindEvents();
    this.saveState(); // initial empty state
  }

  private async initCollaboration(): Promise<void> {
    /* 1️⃣  shared Yjs document */
    const doc = new Y.Doc();
    const connection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/hubs/collab')
      .withAutomaticReconnect()
      .build();
    await connection.start();

    new SignalRProvider(connection, doc); // starts syncing

    /* 2️⃣  CRDT root */
    const konvaMap = doc.getMap('konva');

    /* 3️⃣  create stage & layer */
    this.stage = new Konva.Stage({
      container: this.container.nativeElement,
      width: this.container.nativeElement.offsetWidth,
      height: this.container.nativeElement.offsetHeight
    });

    this.layer = new Konva.Layer();
    this.stage.add(this.layer);

    /* 4️⃣  two-way binding (write once) */
    this.bindKonvaToYjs(this.stage, konvaMap, this.layer);
    this.bindDrawingEvents();           // your existing pen/rect/
  }
  /* -------------------------------------------------- */
  /* Konva ↔ Yjs glue – 1-time helper                   */
  private bindKonvaToYjs(stage: Konva.Stage, map: Y.Map<any>, layer: Konva.Layer) {
    // (1) apply remote updates
    map.observeDeep(() => {
      layer.destroyChildren();
      const tree = map.get('root') || [];
      tree.forEach(nodeObj => layer.add(Konva.Node.create(nodeObj)));
      layer.draw();
    });

    // (2) push local changes
    const push = () => {
      const json = stage.toJSON();
      map.set('root', JSON.parse(json).children[0].children);
    };
    layer.on('add remove dragend transformend', push);
  }

  /* -------------------------------------------------- */
  /* keep your existing drawing logic – it only adds
     shapes to the layer; the observer above broadcasts. */
  private bindDrawingEvents() {
    // … your onMouseDown / onMouseMove / onMouseUp …
  }

  /* ========== EVENT BINDINGS ========== */
  private bindEvents(): void {
    this.stage.on('mousedown', e => this.onStageMouseDown(e));
    this.stage.on('mousemove', e => this.onStageMouseMove(e));
    this.stage.on('mouseup', () => this.onStageMouseUp());

    // Mouse wheel zoom & pan
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

    // Global undo / clear listeners
    window.addEventListener('undo-canvas', () => this.undo());
    window.addEventListener('clear-canvas', () => this.clear());
  }

  /* ========== DRAWING STATE ========== */
  private drawingShape: Konva.Shape | null = null;
  private startPos: Konva.Vector2d | null = null;

  private onStageMouseDown(e: Konva.KonvaEventObject<MouseEvent>): void {
    if (e.target !== this.stage) return;

    const pos = this.stage.getPointerPosition()!;
    this.startPos = { x: pos.x, y: pos.y };
    console.log(this.toolSvc.currentTool);
    switch (this.toolSvc.currentTool) {
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
        return;  // single click, no drag
      case 'eraser':
        return; // handled in move
      case 'select':
        this.transformer.nodes([]);
        return;
    }

    if (this.drawingShape) {
      this.layer.add(this.drawingShape);
    }
  }

  private onStageMouseMove(e: Konva.KonvaEventObject<MouseEvent>): void {
    if (!this.startPos) return;

    const pos = this.stage.getPointerPosition()!;

    if (this.toolSvc.currentTool === 'eraser') {
      const hit = this.layer.getIntersection(pos);
      if (hit && hit.id() !== this.transformer.id()) { // && hit !== this.stage
        this.log({ action: 'delete', shape: hit.toObject() });
        hit.destroy();
        this.layer.batchDraw();
      }
      return;
    }

    if (!this.drawingShape) return;

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

  /* ========== HELPERS ========== */
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
    /* eslint-disable no-console */
    console.table([entry]);
  }

  private saveState(): void {
    this.history.push(this.layer.toJSON());
  }

  private undo(): void {
    if (this.history.length < 2) return;
    this.history.pop(); // remove current
    const json = this.history[this.history.length - 1];
    this.layer.destroyChildren();
    this.layer = Konva.Node.create(json).getLayer()!;
    this.stage.add(this.layer);
    this.layer.add(this.transformer);
    this.stage.batchDraw();
  }

  private clear(): void {
    this.layer.destroyChildren();
    this.layer.add(this.transformer);
    this.saveState();
    this.stage.batchDraw();
    this.log({ action: 'clear', shape: null });
  }
}