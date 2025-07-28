using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Application.Common.Models;

namespace CollabBoard.Application.Chat.Queries.GetChatMessages;

public record GetChatMessagesQuery(Guid BoardId) : IRequest<PaginatedList<ChatMessageDto>>;

public class GetChatMessagesQueryValidator : AbstractValidator<GetChatMessagesQuery>
{
    public GetChatMessagesQueryValidator()
    {
    }
}

public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, PaginatedList<ChatMessageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetChatMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<PaginatedList<ChatMessageDto>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
