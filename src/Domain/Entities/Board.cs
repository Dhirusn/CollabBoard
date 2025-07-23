namespace CollabBoard.Domain.Entities;
public class Board : BaseEntity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<BoardMember> Members { get; private set; } = [];
    public ICollection<Page> Pages { get; private set; } = [];   // Word → multi-page
    public ICollection<ChatMessage> ChatMessages { get; private set; } = [];
    public ICollection<Snapshot>? Snapshots { get; set; }
}
