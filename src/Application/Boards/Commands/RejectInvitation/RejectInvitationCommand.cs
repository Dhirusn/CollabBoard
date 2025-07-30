using CollabBoard.Application.Boards.Commands.AcceptInvitation;
using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Commands.RejectInvitation;
public record RejectInvitationCommand(Guid InvitationId) : IRequest;

public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand>
{
    private readonly IApplicationDbContext _ctx;
    private readonly IUser _current;
    private readonly ILogger<AcceptInvitationCommandHandler> _logger;

    public RejectInvitationCommandHandler(IApplicationDbContext ctx, IUser current, ILogger<AcceptInvitationCommandHandler> logger)
    {
        _ctx = ctx;
        _current = current;
        _logger = logger;
    }
    public async Task Handle(RejectInvitationCommand req, CancellationToken ct)
    {
        try
        {
            var invite = await _ctx.BoardInvitations
                .FirstOrDefaultAsync(i => i.Id == req.InvitationId && i.TargetUserId == _current.Id, ct)
                ?? throw new KeyNotFoundException();

            invite.Status = InvitationStatus.Rejected;
            invite.RespondedUtc = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error while rejecting request");
            throw;
        }
    }
}
