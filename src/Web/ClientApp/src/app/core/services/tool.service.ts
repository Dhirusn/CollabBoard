import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { Awareness } from "y-protocols/awareness";
import { YjsService } from "./yjs.service";

@Injectable({ providedIn: 'root' })
export class ToolService {
  private currentTool$ = new BehaviorSubject<string>('pen');
  private awareness: Awareness;

  constructor(private yjs: YjsService) {
    this.awareness = this.yjs.getAwareness();
    this.awareness.setLocalStateField('tool', 'pen');

    // Listen to others' tool changes
    this.awareness.on('change', changes => {
      for (const clientId of changes.added.concat(changes.updated)) {
        const state = this.awareness.getStates().get(clientId);
        if (state?.tool) {
          console.log(`User ${clientId} is now using: ${state.tool}`);
        }
      }
    });
  }

  setTool(tool: string) {
    this.currentTool$.next(tool);
    this.awareness.setLocalStateField('tool', tool);
  }

  getTool(): Observable<string> {
    return this.currentTool$.asObservable();
  }

  getAwareness(): Awareness {
    return this.awareness;
  }
}
