import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HubConnectionBuilder, HubConnection, LogLevel } from '@microsoft/signalr';
import * as Y from 'yjs';
import { Awareness } from 'y-protocols/awareness';
import * as mutex from 'lib0/mutex';

const updateMutex = mutex.createMutex();
interface SignalRUser {
  connectionId: string;
  userName: string;
  color: string;
  tool: string;
  boardId: string;
}

@Injectable({ providedIn: 'root' })
export class ToolService {
  /* ------------ state ------------ */
  private tool$ = new BehaviorSubject<string>('select');
  private users$ = new BehaviorSubject<SignalRUser[]>([]);

  private doc: Y.Doc;
  private awareness: Awareness;
  private hub: HubConnection;
  private currentBoardId: string = '';

  constructor() {
    this.doc = new Y.Doc();
    this.awareness = new Awareness(this.doc);
  }
  connectToHub(boardId: string): void {
    this.currentBoardId = boardId;
    /* 1️⃣ SignalR connection */
    this.hub = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/hubs/collab')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Debug)
      .build();

    this.hub.start()
      .then(() => {
        console.log('SignalR connected');
        /* --- 2. join the ydoc group --- */
        return this.hub.invoke('JoinYDoc',boardId);
      })
      .catch(err => console.error(err));

    /* --- 1. apply remote updates from SignalR --- */
    this.hub.on('SyncYjsUpdate', (base64: string) => {
      updateMutex(() => {
        const bytes = Uint8Array.from(atob(base64), c => c.charCodeAt(0));
        Y.applyUpdate(this.doc, bytes);
      });
    });


    /* --- 2. listen to local changes only --- */
    // this.doc.on('update', (update: Uint8Array, origin: any) => {
    //   updateMutex(() => {
    //     if (this.hub.state === 'Connected') {
    //       this.hub.invoke('SendYjsUpdate', update.buffer).catch(console.error);
    //     }
    //   });
    // });

    /* --- 4. incoming cursor move --- */
    this.hub.on('userCursorMoved', (dto: { connectionId: string; x: number; y: number }) => {
      // draw / move the remote cursor here
      //console.log('cursor', dto.connectionId, dto.x, dto.y);
    });

    /* --- 5. incoming awareness update --- */
    this.doc.on('update', (update: Uint8Array) => {
      if (!(update instanceof Uint8Array)) {
        console.error('Invalid update payload', update);
        return;
      }

      if (this.hub.state === 'Connected') {
        this.hub.invoke('SendYjsUpdate', this.toBase64(update)).catch(console.error);
      }
    });


    // Handle incoming presence updates from other users
    this.hub.on('UserPresenceChanged', (dto: SignalRUser) => {
      const filtered = this.users$.value.filter(u => u.connectionId !== dto.connectionId);
      this.users$.next([...filtered, dto]);
    });

    this.hub.on('UserDisconnected', (id: string) => {
      this.users$.next(this.users$.value.filter(u => u.connectionId !== id));
    });

    // Yjs awareness → update local tool state & send presence update to server
    this.awareness.on('change', ({ added, updated }) => {
      const changedClients = added.concat(updated);
      changedClients.forEach(clientId => {
        const state = this.awareness.getStates().get(clientId);
        if (state && clientId === this.awareness.clientID) {
          // Send presence update to server
          if (this.hub.state === 'Connected') {
            this.hub.invoke('UpdatePresence', {
              userName: state.user?.name ?? 'Anonymous',
              color: state.user?.color ?? '#999',
              tool: state.tool ?? this.tool$.value,
              connectionId: this.hub.connectionId
            }).catch(console.error);
          } else {
            console.warn('Hub not connected yet, cannot invoke UpdatePresence');
            // Optionally retry later or ignore
          }
        }
      });
    });

    // Keep Yjs awareness tool field in sync with current tool$
    this.tool$.subscribe(tool => {
      this.awareness.setLocalStateField('tool', tool);
    });
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
    if (this.hub.state === 'Connected') {
      this.hub.invoke('MoveCursor', { x, y }).catch(console.error);
    } // silently drop or queue otherwise
  }

  getYDoc(): Y.Doc {
    return this.doc;
  }

  getAwareness(): Awareness {
    return this.awareness;
  }

  /* convenience getter */
  get currentTool(): string {
    return this.tool$.value;
  }

  toBase64(u8: Uint8Array): string {
    let binary = '';
    for (let i = 0; i < u8.byteLength; i++) {
      binary += String.fromCharCode(u8[i]);
    }
    return btoa(binary);
  }
}
