using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.UndoBoardOperation;

public record UndoBoardOperationCommand : IRequest
{
}

public class UndoBoardOperationCommandValidator : AbstractValidator<UndoBoardOperationCommand>
{
    public UndoBoardOperationCommandValidator()
    {
    }
}

public class UndoBoardOperationCommandHandler : IRequestHandler<UndoBoardOperationCommand>
{
    private readonly IApplicationDbContext _context;

    public UndoBoardOperationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(UndoBoardOperationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
