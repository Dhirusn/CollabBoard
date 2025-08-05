import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SendChatMessageCommand, InviteMemberCommand, UserDto, BoardsClient } from 'src/app/web-api-client'; // adjust path
import { ActivatedRoute } from '@angular/router';
import { debounceTime, distinctUntilChanged, Observable, of, switchMap } from 'rxjs';
import { Client } from 'src/app/web-api-client';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {
  messages: { sender: string; text: string }[] = [];
  newMessage = '';
  inviteEmail = '';

  private boardId!: string;
  private userId!: string; // get it from auth service / route if needed
  searchCtrl = new FormControl('');
  foundUsers: UserDto[] = [];
  selectedUser: UserDto | null = null;

  constructor(
    private boardsClient: BoardsClient,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // Resolve board id from URL: /boards/:id/chat
    this.boardId = this.route.snapshot.paramMap.get('id')!;

    // Load existing chat history
    this.loadChatHistory();

    this.searchCtrl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(q => this.searchUsers(q ?? ''))
      )
      .subscribe(users => {
        this.foundUsers = users!;
        this.selectedUser = null; // reset when query changes
      });
  }
  // searchUsers(arg0: string): any {
  //   throw new Error('Method not implemented.');
  // }

  /* ---------------- Chat ---------------- */
  sendMessage(): void {
    if (!this.newMessage.trim()) return;

    const cmd = new SendChatMessageCommand();
    cmd.boardId = this.boardId;

    this.boardsClient.sendChatMessage(this.boardId, cmd)
      .subscribe({
        next: () => {
          this.messages.push({ sender: 'You', text: this.newMessage.trim() });
          this.newMessage = '';
        },
        error: err => console.error(err)
      });
  }

  private loadChatHistory(): void {
    this.boardsClient.getChatMessages(this.boardId, this.boardId) // id & boardId are the same here
      .subscribe({
        next: page => {
          this.messages = (page.items ?? []).map(m => ({
            sender: 'Server', // map real sender when DTO is richer
            text: (m as any).text ?? '' // adapt once ChatMessageDto has a text field
          }));
        },
        error: err => console.error(err)
      });
  }

  /* ---------------- Invite ---------------- */
  searchUsers(query: string): Observable<UserDto[]> {
    if (!query.trim()) return of([]);
    // TODO: replace with real endpoint
     return this.boardsClient.searchUsers(query);

  }

  /* -------- invitation -------- */
  inviteSelectedUser(): void {
    if (!this.selectedUser) return;

    const cmd = new InviteMemberCommand();
    cmd.boardId = this.boardId;
    cmd.targetUserEmail = this.selectedUser.email;
    cmd.role = 1; // Editor

    this.boardsClient.requestBoardMember(cmd).subscribe({
      next: () => {
        alert(`Invitation sent to ${this.selectedUser!.email}`);
        this.searchCtrl.setValue('');
        this.foundUsers = [];
        this.selectedUser = null;
      },
      error: err => alert(err.message ?? 'Invitation failed')
    });
  }
}