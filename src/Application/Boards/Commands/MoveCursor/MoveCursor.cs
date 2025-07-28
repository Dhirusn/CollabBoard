using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.MoveCursor;

public record MoveCursorCommand : IRequest;

public class MoveCursorCommandValidator : AbstractValidator<MoveCursorCommand>
{
    public MoveCursorCommandValidator()
    {
    }
}

public class MoveCursorCommandHandler : IRequestHandler<MoveCursorCommand>
{
    private readonly IApplicationDbContext _context;

    public MoveCursorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(MoveCursorCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
