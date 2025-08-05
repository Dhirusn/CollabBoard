import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  CreateBoardCommand,
  UpdateBoardTitleCommand,
  BoardSummaryDto,
  Client,
  BoardsClient,

} from 'src/app/web-api-client';

@Component({
  selector: 'app-board-list',
  templateUrl: './board-list.component.html',
  styleUrls: ['./board-list.component.scss']
})
export class BoardListComponent implements OnInit {
  userId = 'demo-user-id'; // Replace with real user ID from auth/session
  boards: BoardSummaryDto[] = [];
  newBoardTitle = '';
  editingBoardId: string | null = null;
  updatedTitle = '';

  constructor(
    private boardsClient: BoardsClient,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadBoards();
  }

  loadBoards(): void {
    this.boardsClient.getBoardsForUser(this.userId).subscribe({
      next: (data) => (this.boards = data),
      error: (err) => console.error('Error loading boards', err)
    });
  }

  createBoard(): void {
    const cmd = new CreateBoardCommand();
    (cmd as any).title = this.newBoardTitle;

    this.boardsClient.createBoard(cmd).subscribe({
      next: (newBoardId: string) => {
        this.newBoardTitle = '';
        this.router.navigate(['/board', newBoardId]);
      },
      error: (err) => console.error('Error creating board', err)
    });
  }

  startEditing(boardId: string, currentTitle: string): void {
    this.editingBoardId = boardId;
    this.updatedTitle = currentTitle;
  }

  saveUpdatedTitle(boardId: string): void {
    const cmd = new UpdateBoardTitleCommand();
    (cmd as any).title = this.updatedTitle;

    this.boardsClient.updateBoardTitle(boardId, cmd).subscribe({
      next: () => {
        this.editingBoardId = null;
        this.loadBoards();
      },
      error: (err) => console.error('Error updating title', err)
    });
  }

  navigateToBoard(id: string): void {
    this.router.navigate(['/board', id]);
  }
}
