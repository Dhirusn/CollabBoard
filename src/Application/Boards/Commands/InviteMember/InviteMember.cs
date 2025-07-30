using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;
using CollabBoard.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Commands.InviteMember;

public record InviteMemberCommand(Guid BoardId, string TargetUserEmail, MemberRole Role) : IRequest<Guid>;

public class InviteMemberCommandValidator : AbstractValidator<InviteMemberCommand>
{
    public InviteMemberCommandValidator()
    {
    }
}

public class InviteMemberCommandHandler : IRequestHandler<InviteMemberCommand, Guid>
{
    private readonly IApplicationDbContext _ctx;
    private readonly IUser _current;
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly ILogger<InviteMemberCommandHandler> _logger;

    public InviteMemberCommandHandler(
        IApplicationDbContext ctx,
        IUser current,
        UserManager<ApplicationUser> userMgr,
        ILogger<InviteMemberCommandHandler>logger)
    {
        _ctx = ctx;
        _current = current;
        _userMgr = userMgr;
        _logger = logger;
    }

    public async Task<Guid> Handle(InviteMemberCommand req, CancellationToken ct)
    {
        try
        {
            var board = await _ctx.Boards
                              .Include(b => b.Members)
                              .FirstOrDefaultAsync(b => b.Id == req.BoardId, ct)
                    ?? throw new KeyNotFoundException("Board not found.");

            if (board.OwnerId != _current.Id &&
                !board.Members.Any(m => m.UserId == _current.Id && m.Role == MemberRole.Editor))
                throw new UnauthorizedAccessException("Only owner or editors can invite.");

            var target = await _userMgr.FindByEmailAsync(req.TargetUserEmail)
                         ?? throw new KeyNotFoundException("User not found.");

            var existing = await _ctx.BoardInvitations
                .AnyAsync(i => i.BoardId == req.BoardId && i.TargetUserId == target.Id && i.Status == InvitationStatus.Pending, ct);
            if (existing) throw new InvalidOperationException("Invitation already pending.");

            var invite = new BoardInvitation
            {
                Id = Guid.NewGuid(),
                BoardId = req.BoardId,
                InvitedByUserId = _current.Id!,
                TargetUserId = target.Id,
                RequestedRole = req.Role
            };

            _ctx.BoardInvitations.Add(invite);
            await _ctx.SaveChangesAsync(ct);
            return invite.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inviting member to board {BoardId} for user {TargetUserEmail}", req.BoardId, req.TargetUserEmail);
            throw;
        }
        
    }
}
