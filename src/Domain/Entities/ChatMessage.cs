namespace CollabBoard.Domain.Entities;
public class ChatMessage : BaseEntity<Guid>
{
    public Guid BoardId { get; set; }
    public string? UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentUtc { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; } = null!;
    public Board Board { get; set; } = null!; // Navigation property to parent board
}
