using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.RedoBoardOperation;

public record RedoBoardOperationCommand : IRequest
{
}

public class RedoBoardOperationCommandValidator : AbstractValidator<RedoBoardOperationCommand>
{
    public RedoBoardOperationCommandValidator()
    {
    }
}

public class RedoBoardOperationCommandHandler : IRequestHandler<RedoBoardOperationCommand>
{
    private readonly IApplicationDbContext _context;

    public RedoBoardOperationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(RedoBoardOperationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
