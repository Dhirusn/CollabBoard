import * as Y from 'yjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Awareness, encodeAwarenessUpdate, applyAwarenessUpdate } from 'y-protocols/awareness';

// ...


export class SignalRYjsProvider {
    private connection: HubConnection;
    private awareness: Awareness;

    constructor(
        private roomId: string,
        private ydoc: Y.Doc,
        signalRHubUrl: string // e.g. "https://localhost:5001/hubs/collab"
    ) {
        this.connection = new HubConnectionBuilder()
            .withUrl(signalRHubUrl)
            .withAutomaticReconnect()
            .build();

        this.awareness = new Awareness(ydoc);

        // 1. Listen for incoming Yjs updates
        this.connection.on('ReceiveUpdate', (update: ArrayBuffer) => {
            const binary = new Uint8Array(update);
            Y.applyUpdate(this.ydoc, binary);
        });

        // 2. Broadcast awareness to others
        this.awareness.on('update', ({ added, updated, removed }) => {
            const update = encodeAwarenessUpdate(this.awareness, [...added, ...updated, ...removed]);
            this.connection.send('BroadcastAwareness', this.roomId, update);
        });

        // 3. Apply awareness updates
        this.connection.on('ReceiveAwareness', (update: ArrayBuffer) => {
            const binary = new Uint8Array(update);
            applyAwarenessUpdate(this.awareness, binary, this.connection.connectionId ?? null);
        });

        // 4. Send updates to server
        this.ydoc.on('update', update => {
            this.connection.send('BroadcastYjsUpdate', this.roomId, update);
        });

        this.connection.start().then(() => {
            console.log('Connected to SignalR hub:', this.roomId);
        });
    }

    getAwareness(): Awareness {
        return this.awareness;
    }
}
