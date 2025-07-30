using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Commands.CreateBoard;

public record CreateBoardCommand(string Title) : IRequest<Guid>;

public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
    }
}

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<CreateBoardCommandHandler> _logger;
    public CreateBoardCommandHandler(IApplicationDbContext context, IUser currentUser,ILogger<CreateBoardCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken ct)
    {
        try
        {
            var board = new Board
            {
               // Id = Guid.NewGuid(),
                Title = request.Title,
                OwnerId = _currentUser.Id ?? throw new UnauthorizedAccessException(),
                CreatedBy = _currentUser.Id ?? throw new UnauthorizedAccessException(),
                Created = DateTimeOffset.UtcNow
            };

            _context.Boards.Add(board);
            await _context.SaveChangesAsync(ct);

            return board.Id;
        }
        catch (Exception ex)
        {
            // enrich with extra properties, then re-throw
            _logger.LogError(ex,
                "Failed to create board with title '{Title}' for user {UserId}",
                request.Title, _currentUser.Id);

            throw;   // preserves the original stack trace
        }
    }
}
