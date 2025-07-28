using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.InviteMember;

public record InviteMemberCommand(Guid BoardId) : IRequest<Guid>;

public class InviteMemberCommandValidator : AbstractValidator<InviteMemberCommand>
{
    public InviteMemberCommandValidator()
    {
    }
}

public class InviteMemberCommandHandler : IRequestHandler<InviteMemberCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public InviteMemberCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Guid> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
