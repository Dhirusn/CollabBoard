using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.DeleteBlock;

public record DeleteBlockCommand : IRequest
{
}

public class DeleteBlockCommandValidator : AbstractValidator<DeleteBlockCommand>
{
    public DeleteBlockCommandValidator()
    {
    }
}

public class DeleteBlockCommandHandler : IRequestHandler<DeleteBlockCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBlockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(DeleteBlockCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
