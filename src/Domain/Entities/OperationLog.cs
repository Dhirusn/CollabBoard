namespace CollabBoard.Domain.Entities;
public class OperationLog : BaseEntity<long>
{
    public string? UserId { get; set; }
    public Guid BoardId { get; set; }
    public EntityType EntityType { get; set; }   // ContentBlock | Page
    public Guid EntityId { get; set; }
    public Operation Operation { get; set; }
    public string Delta { get; set; } = string.Empty; // JSON Patch / OT
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; } = null!;
    public Board Board { get; set; } = null!;
}
