using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;
using CollabBoard.Domain.Enums;

namespace CollabBoard.Application.Boards.Commands.JoinBoard;

public record JoinBoardCommand(Guid UserId, Guid BoardId) : IRequest<Unit>; // <-- Unit

public class JoinBoardCommandValidator : AbstractValidator<JoinBoardCommand>
{
    public JoinBoardCommandValidator() { }
}

public class JoinBoardCommandHandler : IRequestHandler<JoinBoardCommand, Unit> // <-- Unit
{
    private readonly IApplicationDbContext _context;

    public JoinBoardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(JoinBoardCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.BoardsMembers
            .AnyAsync(bm => bm.BoardId == request.BoardId &&
                            bm.UserId == request.UserId.ToString(),
                      cancellationToken);

        if (!exists)
        {
            var entity = new BoardMember
            {
                BoardId = request.BoardId,
                UserId = request.UserId.ToString(),
                Role = MemberRole.Viewer
            };
            _context.BoardsMembers.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;   // <-- now compiles
    }
}
