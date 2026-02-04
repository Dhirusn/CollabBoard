using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;
using CollabBoard.Web.Hubs;
using Microsoft.EntityFrameworkCore;

namespace CollabBoard.Web.Services;

public sealed class SnapshotBoardStore : IBoardStore
{
    private readonly IApplicationDbContext _ctx;
    public SnapshotBoardStore(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<bool> UserCanAccessAsync(string userId, Guid boardId)
    {
        return await _ctx.BoardsMembers
                         .AnyAsync(bm => bm.UserId == userId &&
                                         bm.BoardId == boardId);
    }

    public async Task<byte[]?> LoadStateAsync(Guid boardId)
    {
        var row = await _ctx.Snapshots
                            .Where(s => s.BoardId == boardId)
                            .OrderByDescending(s => s.TakenUtc)
                            .Select(s => s.PagesBlob)
                            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(row)) return null;

        return Convert.FromBase64String(row);   // PagesBlob already Base64
    }

    public async Task SaveStateAsync(Guid boardId, byte[] state)
    {
        var base64 = Convert.ToBase64String(state);

        var existing = await _ctx.Snapshots
                                 .FindAsync(boardId);
        var ctoken = new CancellationToken();
        if (existing is null)
        {
            _ctx.Snapshots.Add(new Snapshot
            {
                Id = Guid.NewGuid(),
                BoardId = boardId,
                PagesBlob = base64,
                TakenUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.PagesBlob = base64;
            existing.TakenUtc = DateTime.UtcNow;
        }

        await _ctx.SaveChangesAsync(ctoken);
    }
}
