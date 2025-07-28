using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Queries.GetBoardSnapshot;

public record GetBoardSnapshotQuery(Guid id) : IRequest<BoardSnapshotDto>;

public class GetBoardSnapshotQueryValidator : AbstractValidator<GetBoardSnapshotQuery>
{
    public GetBoardSnapshotQueryValidator()
    {
    }
}

public class GetBoardSnapshotQueryHandler : IRequestHandler<GetBoardSnapshotQuery, BoardSnapshotDto>
{
    private readonly IApplicationDbContext _context;

    public GetBoardSnapshotQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public  Task<BoardSnapshotDto> Handle(GetBoardSnapshotQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
