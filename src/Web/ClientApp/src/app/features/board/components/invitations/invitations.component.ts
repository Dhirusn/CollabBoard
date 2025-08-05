import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BoardsClient, RejectInvitationCommand, InvitationDto } from 'src/app/web-api-client';;
import { Observable } from 'rxjs';


@Component({
  selector: 'app-invitations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './invitations.component.html',
  styleUrls: ['./invitations.component.scss']
})
export class InvitationsComponent implements OnInit {
  invitations$: Observable<InvitationDto[]> = new Observable();

  constructor(private boardsClient: BoardsClient) { }

  ngOnInit(): void {
    this.loadInvitations();
  }

  private loadInvitations(): void {
    this.invitations$ = this.boardsClient.getMyInvitations(); // see step 3
  }

  accept(invitationId: string): void {
    //const cmd = new AcceptInvitationCommand();
   // cmd.invitationId = invitationId;

    this.boardsClient.acceptInvitation(invitationId).subscribe({
      next: () => this.loadInvitations(),
      error: err => alert(err.message || 'Accept failed')
    });
  }

  reject(invitationId: string): void {
    const cmd = new RejectInvitationCommand();
    cmd.invitationId = invitationId;

    this.boardsClient.rejectInvitation(cmd).subscribe({
      next: () => this.loadInvitations(),
      error: err => alert(err.message || 'Reject failed')
    });
  }
}