import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { BoardListComponent } from './board-list.component';
import { FormsModule } from '@angular/forms';
import { InvitationsComponent } from '../board/components/invitations/invitations.component';


const routes: Routes = [
  { path: '', component: BoardListComponent }
];

@NgModule({
  declarations: [
    BoardListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    InvitationsComponent,
    RouterModule.forChild(routes)
  ]
})
export class BoardListModule { }
