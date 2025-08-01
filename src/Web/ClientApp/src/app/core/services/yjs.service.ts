  import { Injectable } from '@angular/core';
  import * as Y from 'yjs';
  import { Awareness } from 'y-protocols/awareness';

  @Injectable({ providedIn: 'root' })
  export class YjsService {
    private ydoc: Y.Doc;
    public awareness: Awareness;

    constructor() {
      this.ydoc = new Y.Doc();
      this.awareness = new Awareness(this.ydoc);

      // Set default local presence
      this.awareness.setLocalState({
        user: {
          name: 'Dhirendra',
          color: '#2f80ed'
        },
        tool: 'pen'
      });

      // [Optional] log awareness changes
      this.awareness.on('change', changes => {
        console.log('Awareness changed', changes);
      });
    }

    getDoc(): Y.Doc {
      return this.ydoc;
    }

    getAwareness(): Awareness {
      return this.awareness;
    }
  }
