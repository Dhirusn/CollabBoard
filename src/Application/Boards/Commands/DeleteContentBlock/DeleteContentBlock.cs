using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.DeleteContentBlock;

public record DeleteContentBlockCommand(Guid BoardId, Guid blockId) : IRequest;

public class DeleteContentBlockCommandValidator : AbstractValidator<DeleteContentBlockCommand>
{
    public DeleteContentBlockCommandValidator()
    {
    }
}

public class DeleteContentBlockCommandHandler : IRequestHandler<DeleteContentBlockCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteContentBlockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(DeleteContentBlockCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
