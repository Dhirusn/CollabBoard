import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToolService } from 'src/app/core/services/tool.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent implements OnInit, OnDestroy {
  showTools = true;
  showChat = true;
  zoomLevel = 1;
  usersOnline$ = this.toolSvc.getPresence();
  tools = [
    { id: 'pen', icon: '✏️', name: 'Pen' },
    { id: 'rect', icon: '🔲', name: 'Rectangle' },
    { id: 'circle', icon: '⭕', name: 'Circle' },
    { id: 'text', icon: '🔤', name: 'Text' },
    { id: 'eraser', icon: '🧽', name: 'Eraser' },
    { id: 'select', icon: '🤏', name: 'Select / Move' }
  ];

  constructor(public toolSvc: ToolService) { }
  ngOnDestroy(): void {
    //throw new Error('Method not implemented.');
  }
  ngOnInit(): void {
    //throw new Error('Method not implemented.');
  }

  selectTool(toolId: string) {
    this.toolSvc.setTool(toolId);
  }

  zoomIn() { this.zoomLevel = Math.min(this.zoomLevel + 0.1, 3); }
  zoomOut() { this.zoomLevel = Math.max(this.zoomLevel - 0.1, 0.5); }

  undo() { window.dispatchEvent(new Event('undo-canvas')); }
  clear() { window.dispatchEvent(new Event('clear-canvas')); }
}
