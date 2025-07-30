namespace CollabBoard.Domain.Entities;

public class BoardInvitation : BaseEntity<Guid>
{
    public Guid BoardId { get; set; }
    public Board Board { get; set; } = null!;

    public string InvitedByUserId { get; set; } = null!;
    public string TargetUserId { get; set; } = null!;
    public MemberRole RequestedRole { get; set; } = MemberRole.Viewer;

    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime? RespondedUtc { get; set; }
}
