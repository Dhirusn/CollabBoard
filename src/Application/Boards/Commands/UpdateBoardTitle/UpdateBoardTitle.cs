using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.UpdateBoardTitle;

public record UpdateBoardTitleCommand(Guid Id) : IRequest;

public class UpdateBoardTitleCommandValidator : AbstractValidator<UpdateBoardTitleCommand>
{
    public UpdateBoardTitleCommandValidator()
    {
    }
}

public class UpdateBoardTitleCommandHandler : IRequestHandler<UpdateBoardTitleCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateBoardTitleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Handle(UpdateBoardTitleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
