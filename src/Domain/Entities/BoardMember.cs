namespace CollabBoard.Domain.Entities;
public class BoardMember : BaseEntity<Guid>
{
    public Guid BoardId { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Board Board { get; set; } = null!;
    public MemberRole Role { get; set; } = MemberRole.Viewer;
    public DateTime JoinedUtc { get; set; } = DateTime.UtcNow;
}
