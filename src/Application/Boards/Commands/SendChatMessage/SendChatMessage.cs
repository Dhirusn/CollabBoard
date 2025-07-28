using CollabBoard.Application.Common.Interfaces;

namespace CollabBoard.Application.Boards.Commands.SendChatMessage;

public record SendChatMessageCommand(Guid BoardId) : IRequest<Guid>;

public class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageCommandValidator()
    {
    }
}

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public SendChatMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Guid> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
