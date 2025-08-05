using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;
using CollabBoard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Commands.AcceptInvitation;

public record AcceptInvitationCommand(Guid InvitationId) : IRequest<Guid>;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Guid>
{
    private readonly IApplicationDbContext _ctx;
    private readonly IUser _current;
    private readonly ILogger<AcceptInvitationCommandHandler> _logger;
    public AcceptInvitationCommandHandler(IApplicationDbContext ctx, IUser current, ILogger<AcceptInvitationCommandHandler> logger)
    {
        _ctx = ctx;
        _current = current;
        _logger = logger;
    }
    public async Task<Guid> Handle(AcceptInvitationCommand req, CancellationToken ct)
    {
        try
        {
            var invite = await _ctx.BoardInvitations
           .Include(i => i.Board)
           .FirstOrDefaultAsync(i => i.Id == req.InvitationId && i.TargetUserId == _current.Id, ct)
           ?? throw new KeyNotFoundException("Invitation not found.");

            if (invite.Status != InvitationStatus.Pending)
                throw new InvalidOperationException("Invitation already handled.");

            invite.Status = InvitationStatus.Accepted;
            invite.RespondedUtc = DateTime.UtcNow;

            var member = new BoardMember
            {
                BoardId = invite.BoardId,
                UserId = invite.TargetUserId,
                Role = invite.RequestedRole,
                JoinedUtc = DateTime.UtcNow
            };

            _ctx.BoardsMembers.Add(member);
            await _ctx.SaveChangesAsync(ct);
            return member.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting invitation {InvitationId} for user {UserId}", req.InvitationId, _current.Id);
            throw;
        }
       
    }
}
