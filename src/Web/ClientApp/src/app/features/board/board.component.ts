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
  currentTool: string = 'pen'; // Default tool
  private toolSub!: Subscription;

  constructor(private toolService: ToolService) {}

  ngOnInit(): void {
    // Subscribe to tool changes
    this.toolSub = this.toolService.getTool().subscribe(tool => {
      this.currentTool = tool;
    });
  }

  ngOnDestroy(): void {
    this.toolSub?.unsubscribe();
  }

  toggleTools() {
    this.showTools = !this.showTools;
  }

  toggleChat() {
    this.showChat = !this.showChat;
  }

  zoomIn() {
    this.zoomLevel = Math.min(this.zoomLevel + 0.1, 3);
  }

  zoomOut() {
    this.zoomLevel = Math.max(this.zoomLevel - 0.1, 0.5);
  }

  selectTool(tool: string) {
    this.toolService.setTool(tool);
  }
}
