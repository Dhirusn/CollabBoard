import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { BoardComponent } from './board.component';
import { ChatComponent } from './components/chat/chat.component';
import { CanvasComponent } from './components/canvas/canvas.component';


const routes: Routes = [
 {
    path: ':id', // board/:id
    component: BoardComponent
  }
];

@NgModule({
  declarations: [
    BoardComponent
  ],
  imports: [
    CommonModule,
    ChatComponent,
    CanvasComponent,
    RouterModule.forChild(routes)
  ]
})
export class BoardModule { }
