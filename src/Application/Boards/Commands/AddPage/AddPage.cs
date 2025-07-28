using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;

namespace CollabBoard.Application.Boards.Commands.AddPage;

public record AddPageCommand(Guid BoardId ) : IRequest<Guid>;

public class AddPageCommandValidator : AbstractValidator<AddPageCommand>
{
    public AddPageCommandValidator()
    {
    }
}

public class AddPageCommandHandler : IRequestHandler<AddPageCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddPageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddPageCommand request, CancellationToken cancellationToken)
    {
        
        await Task.Delay(1000);
        throw new NotImplementedException();
    }
}
