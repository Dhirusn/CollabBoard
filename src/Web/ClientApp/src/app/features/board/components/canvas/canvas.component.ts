import { AfterViewInit, Component, ElementRef, Input, ViewChild } from '@angular/core';
import Konva from 'konva';
// import Konva from 'konva';
import { ToolService } from 'src/app/core/services/tool.service';
import { v4 as generateUuid } from 'uuid';

@Component({
  selector: 'app-canvas',
  standalone: true,
  imports: [],
  templateUrl: './canvas.component.html',
  styleUrl: './canvas.component.scss'
})
export class CanvasComponent {
  @ViewChild('stageContainer', { static: true }) containerRef!: ElementRef;

  private stage!: Konva.Stage;
  private layer!: Konva.Layer;
  activeTool: string = 'pen'; // or any default value

  @Input() set zoom(value: number) {
    this._zoom = value;
    if (this.stage) {
      this.stage.scale({ x: value, y: value });
      this.stage.batchDraw();
    }
  }
  private _zoom = 1;

  constructor(private toolService: ToolService) { }

  ngOnInit() {
    this.toolService.getTool().subscribe(tool => {
      this.activeTool = tool;
    });
  }

  ngAfterViewInit(): void {
    // Create stage
    this.stage = new Konva.Stage({
      container: this.containerRef.nativeElement,
      width: 800,
      height: 600
    });

    // Add layer
    this.layer = new Konva.Layer();
    this.stage.add(this.layer);

    // Handle stage clicks
    this.stage.on('click', (e) => {
      if (e.target !== this.stage) return;

      const pos = this.stage.getPointerPosition();
      if (!pos) return;

      const circle = new Konva.Circle({
        id: generateUuid(),
        x: pos.x,
        y: pos.y,
        radius: 30,
        fill: 'deepskyblue',
        stroke: 'black',
        strokeWidth: 2,
        draggable: true
      });

      this.layer.add(circle);
      this.layer.draw();
    });
  }
}
