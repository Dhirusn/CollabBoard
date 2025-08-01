import { Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SignalRHubService {
  private hub: HubConnection;
  userJoined$ = new Subject<string>();

  constructor() {
    this.hub = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/hubs/collab') // same URL as Startup
      .withAutomaticReconnect()
      .build();

    // listen for the new connection id
    this.hub.on('UserPresenceChanged', dto => this.userJoined$.next(dto.connectionId));

    this.hub.start()
      .then(() => console.log('SignalR connected, my id:', this.hub.connectionId))
      .catch(err => console.error(err));
  }

  get connectionId(): string | null {
    return this.hub.connectionId;
  }

  getHub(): HubConnection {
    return this.hub;
  }
}