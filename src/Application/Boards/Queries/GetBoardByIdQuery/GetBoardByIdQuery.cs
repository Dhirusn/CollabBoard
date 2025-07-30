
using CollabBoard.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Queries.GetBoardByIdQuery;
public record GetBoardByIdQuery(Guid Id) : IRequest<BoardDto>;

public class GetBoardByIdQueryValidator : AbstractValidator<GetBoardByIdQuery>
{
    public GetBoardByIdQueryValidator()
    {
        // Add validation rules here
    }
}

public class GetBoardByIdQueryHandler : IRequestHandler<GetBoardByIdQuery, BoardDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetBoardByIdQueryHandler> _logger;
    public GetBoardByIdQueryHandler(IApplicationDbContext context, ILogger<GetBoardByIdQueryHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }
    public async Task<BoardDto> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _context.Boards
                                      .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (board == null) throw new KeyNotFoundException("Board not found.");

           return new BoardDto
           {
               Id = board.Id,
               Title = board.Title,
               CreatedBy = board.CreatedBy,
               Created = board.Created,
               // Map other properties as needed
           };
        }
        catch (Exception ex)
        {
            // enrich with extra properties, then re-throw
            _logger.LogError(ex,
                "Failed to retrive board with ");

            throw;   // preserves the original stack trace
        }
    }
}
