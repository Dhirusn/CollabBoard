using Microsoft.AspNetCore.Authorization;

namespace CollabBoard.Web.Infrastructure;
public class BoardAccessRequirement : IAuthorizationRequirement
{
    public Guid BoardId { get; }

    public BoardAccessRequirement(Guid boardId)
    {
        BoardId = boardId;
    }
}

