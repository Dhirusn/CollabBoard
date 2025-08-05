using CollabBoard.Domain.Enums;

namespace CollabBoard.Application.Boards.Queries.GetMyInvitationsQuery;
public record InvitationDto(
    Guid InvitationId,
    string BoardTitle,
    Guid BoardId,
    MemberRole RequestedRole,
    DateTime? CreatedUtc);
