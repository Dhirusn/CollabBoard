using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(Guid id) : IRequest;
public class DeleteBoardCommandValidator : AbstractValidator<DeleteBoardCommand>
{
    public DeleteBoardCommandValidator()
    {
    }
}

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBoardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        throw new NotImplementedException();
    }
}
