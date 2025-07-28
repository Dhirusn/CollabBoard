using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Queries.GetBoardMembers;

public record GetBoardMembersQuery(Guid id) : IRequest<List<BoardMemberDto>>;

public class GetBoardMembersQueryValidator : AbstractValidator<GetBoardMembersQuery>
{
    public GetBoardMembersQueryValidator()
    {
    }
}

public class GetBoardMembersQueryHandler : IRequestHandler<GetBoardMembersQuery, List<BoardMemberDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBoardMembersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<BoardMemberDto>> Handle(GetBoardMembersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
