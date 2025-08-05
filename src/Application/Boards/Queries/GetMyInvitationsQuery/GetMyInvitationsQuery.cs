using System.Linq;
using CollabBoard.Application.Boards.Queries.GetBoardByIdQuery;
using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Queries.GetMyInvitationsQuery;
public record GetMyInvitationsQuery : IRequest<List<InvitationDto>>;

public class GetMyInvitationsQueryHandler
    : IRequestHandler<GetMyInvitationsQuery, List<InvitationDto>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly IUser _user;
    private readonly ILogger<GetMyInvitationsQuery> _logger;

    public GetMyInvitationsQueryHandler(IApplicationDbContext ctx, IUser user,    ILogger<GetMyInvitationsQuery> logger)
    {
        _ctx = ctx;
        _user = user;
        _logger = logger;
    }

    public async Task<List<InvitationDto>> Handle(GetMyInvitationsQuery req,
                                                  CancellationToken ct)
    {
        try
        {
            return await _ctx.BoardInvitations
           .AsNoTracking()
           .Where(i => i.TargetUserId == _user.Id &&
                       i.Status == InvitationStatus.Pending)
           .OrderByDescending(i => i.Created)
           .Select(i => new InvitationDto(
               i.Id,
               i.Board.Title,
               i.BoardId,
               i.RequestedRole,
               (DateTime?)i.Created.UtcDateTime))
           .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
           "Failed to retrive board with ");
            throw ;
        }
       
    }
}
