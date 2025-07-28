using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.UpsertContentBlock;

public record UpsertContentBlockCommand(Guid BoardId) : IRequest<Guid>;


public class UpsertContentBlockCommandValidator : AbstractValidator<UpsertContentBlockCommand>
{
    public UpsertContentBlockCommandValidator()
    {
    }
}

public class UpsertContentBlockCommandHandler : IRequestHandler<UpsertContentBlockCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpsertContentBlockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public  Task<Guid> Handle(UpsertContentBlockCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
