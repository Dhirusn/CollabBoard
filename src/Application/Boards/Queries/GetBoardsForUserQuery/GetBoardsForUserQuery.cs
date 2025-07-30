using CollabBoard.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Queries.GetBoardsForUserQuery;
public record GetBoardsForUserQuery(string UserId) : IRequest<List<BoardSummaryDto>>;

public class GetBoardsForUserQueryHandler : IRequestHandler<GetBoardsForUserQuery, List<BoardSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<GetBoardsForUserQueryHandler> _logger;
    private readonly IMapper _mapper;
    public GetBoardsForUserQueryHandler(IApplicationDbContext context, IUser user, ILogger<GetBoardsForUserQueryHandler> logger, IMapper mapper)
    {
        _context = context;
        _currentUser = user;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<List<BoardSummaryDto>> Handle(GetBoardsForUserQuery request, CancellationToken cancellationToken)
    {
        var userId = !string.IsNullOrWhiteSpace(request.UserId) && Guid.TryParse(request.UserId, out _)
                            ? request.UserId: _currentUser.Id!.ToString();
        try
        {

            var boards = await _context.Boards
            .Where(b => b.OwnerId == userId.ToString() || b.CreatedBy == userId.ToString() ||
                        b.Members.Any(m => m.UserId == userId.ToString()))
            .OrderByDescending(b => b.Created)
            .ToListAsync(cancellationToken);

            return _mapper.Map<List<BoardSummaryDto>>(boards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving boards for user {UserId}", userId);
            throw;
        }

    }
}
