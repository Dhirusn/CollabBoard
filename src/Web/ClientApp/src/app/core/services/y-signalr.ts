import * as Y from 'yjs';
import { HubConnection } from '@microsoft/signalr';
import { Awareness } from 'y-protocols/awareness';

export class SignalRProvider {
    private doc: Y.Doc;
    private awareness: Awareness;
    private connection: HubConnection;

    constructor(connection: HubConnection, doc: Y.Doc) {
        this.doc = doc;
        this.awareness = new Awareness(doc);
        this.connection = connection;

        // 1️⃣  outgoing
        this.doc.on('update', (update: Uint8Array) =>
            this.connection.invoke('BroadcastYjsUpdate', Array.from(update))
        );

        // 2️⃣  incoming
        this.connection.on('SyncYjsUpdate', (bytes: number[]) =>
            Y.applyUpdate(this.doc, new Uint8Array(bytes))
        );

        // 3️⃣  awareness
        this.awareness.on('update', ({ added, updated, removed }) => {
            const changedClients = added.concat(updated).concat(removed);
            const states = changedClients
                .map(id => ({ clientId: id, state: this.awareness.getStates().get(id) }))
                .filter(Boolean);
            this.connection.invoke('BroadcastAwareness', states);
        });

        this.connection.on('SyncAwareness', states => {
            states.forEach(({ clientId, state }: any) => {
                if (state === null) this.removeState(this.awareness, clientId);
                else this.awareness.setLocalStateField(clientId, state);
            });
        });
    }
    // safe helper
    removeState(awareness: Awareness, clientId: number) {
        if ('removeAwarenessStates' in awareness) {
            (awareness as any).removeAwarenessStates([clientId]);
        } else if ('removeAwareness' in awareness) {
            (awareness as any).removeAwareness([clientId]);
        }
        // else: ignore – client will disappear on disconnect
    }
}