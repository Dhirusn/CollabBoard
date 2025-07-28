using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.CreateBoard;

public record CreateBoardCommand : IRequest<Guid>
{
}

public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
    }
}

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBoardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
