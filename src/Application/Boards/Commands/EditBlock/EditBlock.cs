using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.EditBlock;

public record EditBlockCommand : IRequest
{
}

public class EditBlockCommandValidator : AbstractValidator<EditBlockCommand>
{
    public EditBlockCommandValidator()
    {
    }
}

public class EditBlockCommandHandler : IRequestHandler<EditBlockCommand>
{
    private readonly IApplicationDbContext _context;

    public EditBlockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(EditBlockCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
