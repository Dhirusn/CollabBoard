import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HubConnectionBuilder, HubConnection, LogLevel } from '@microsoft/signalr';
import { Awareness } from 'y-protocols/awareness';
import { YjsService } from './yjs.service';
import { SignalRHubService } from './signalr-hub.service';

interface SignalRUser {
  connectionId: string;
  userName: string;
  color: string;
  tool: string;
}

@Injectable({ providedIn: 'root' })
export class ToolService {
  /* ------------ state ------------ */
  private tool$ = new BehaviorSubject<string>('select');
  private users$ = new BehaviorSubject<SignalRUser[]>([]);

  private awareness: Awareness;
  // private hub: HubConnection;

  constructor(private yjs: YjsService, private hub: SignalRHubService) {
    this.awareness = yjs.getAwareness();

    /* 1️⃣  SignalR connection */
    // this.hub = new HubConnectionBuilder()
    //   .withUrl('https://localhost:5001/hubs/collab')
    //   .withAutomaticReconnect()
    //   .configureLogging(LogLevel.Debug)
    //   .build();

    // this.hub.start()
    //   .then(() => console.log('SignalR connected'))
    //   .catch(err => console.error(err));

    // /* 2️⃣  incoming presence */
    // this.hub.on('UserPresenceChanged', (dto: SignalRUser) => {
    //   const list = this.users$.value.filter(u => u.connectionId !== dto.connectionId);
    //   this.users$.next([...list, dto]);
    // });

    // this.hub.on('UserDisconnected', (id: string) => {
    //   this.users$.next(this.users$.value.filter(u => u.connectionId !== id));
    // });

    /* 3️⃣  Yjs awareness → local tool */
    this.awareness.on('change', ({ added, updated }) => {
      const ids = added.concat(updated);
      ids.forEach(clientId => {
        const state = this.awareness.getStates().get(clientId);
        if (state && clientId === this.awareness.clientID) {
          this.hub.getHub().invoke('UpdatePresence', {
            userName: state.user?.name ?? 'Anonymous',
            color: state.user?.color ?? '#999',
            tool: state.tool
          });
        }
      });
    });

    /* 4️⃣  keep Yjs in sync */
    this.tool$.subscribe(tool => this.awareness.setLocalStateField('tool', tool));
  }

  /* ------------ public API ------------ */
  setTool(tool: string): void {
    this.tool$.next(tool);
  }

  getTool(): Observable<string> {
    return this.tool$.asObservable();
  }

  getPresence(): Observable<SignalRUser[]> {
    return this.users$.asObservable();
  }

  moveCursor(x: number, y: number): void {
    this.hub.getHub().invoke('MoveCursor', { x, y });
  }

  /* convenience getter */
  get currentTool(): string {
    return this.tool$.value;
  }
}