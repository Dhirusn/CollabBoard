using CollabBoard.Application.Boards.Commands.CreateBoard;
using CollabBoard.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(Guid Id) : IRequest;
public class DeleteBoardCommandValidator : AbstractValidator<DeleteBoardCommand>
{
    public DeleteBoardCommandValidator()
    {
    }
}

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<DeleteBoardCommandHandler> _logger;
    public DeleteBoardCommandHandler(IApplicationDbContext context, IUser user, ILogger<DeleteBoardCommandHandler> logger)
    {
        _context = context;
        _currentUser = user;
        _logger = logger;
    }

    public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _context.Boards
                                      .FirstOrDefaultAsync(b => b.Id == request.Id && b.OwnerId == _currentUser.Id, cancellationToken);

            if (board == null) throw new KeyNotFoundException("Board not found or user don't have permission to update");
            _context.Boards.Remove(board);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // enrich with extra properties, then re-throw
            _logger.LogError(ex,
                "Failed to delete board");

            throw;   // preserves the original stack trace
        }
    }
}
