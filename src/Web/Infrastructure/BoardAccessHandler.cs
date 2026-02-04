namespace CollabBoard.Web.Infrastructure;

using System;
using System.Security.Claims;
using CollabBoard.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public class BoardAccessHandler : AuthorizationHandler<BoardAccessRequirement>
{
    private readonly IApplicationDbContext _context;

    public BoardAccessHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BoardAccessRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return;

        var hasAccess = await _context.BoardsMembers
            .AsNoTracking()
            .AnyAsync(m => m.BoardId == requirement.BoardId && m.UserId == userId);

        if (hasAccess)
        {
            context.Succeed(requirement);
        }
    }
}

