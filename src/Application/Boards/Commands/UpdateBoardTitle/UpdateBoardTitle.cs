using CollabBoard.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Commands.UpdateBoardTitle;

public record UpdateBoardTitleCommand(Guid Id, string Title) : IRequest;

public class UpdateBoardTitleCommandValidator : AbstractValidator<UpdateBoardTitleCommand>
{
    public UpdateBoardTitleCommandValidator()
    {
    }
}

public class UpdateBoardTitleCommandHandler : IRequestHandler<UpdateBoardTitleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<UpdateBoardTitleCommandHandler> _logger;
    public UpdateBoardTitleCommandHandler(IApplicationDbContext context,ILogger<UpdateBoardTitleCommandHandler> logger, IUser currentUser)
    {
        _context = context;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateBoardTitleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var board = await _context.Boards
                                      .FirstOrDefaultAsync(b => b.Id == request.Id && (b.OwnerId == _currentUser.Id || b.CreatedBy == _currentUser.Id), cancellationToken);

            if (board == null) throw new KeyNotFoundException("Board not found or user don't have permission to update");

            board.Title = request.Title;
            await _context.SaveChangesAsync(cancellationToken);
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
