import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule,
             FormsModule
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent {
  messages: { sender: string; text: string }[] = [
    { sender: 'Alice', text: 'Hello everyone!' },
    { sender: 'Bob', text: 'Let’s draw something cool!' }
  ];

  newMessage = '';

  sendMessage(): void {
    if (this.newMessage.trim()) {
      this.messages.push({
        sender: 'You',
        text: this.newMessage.trim()
      });
      this.newMessage = '';
    }
  }
}
