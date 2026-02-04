using System.Collections.Concurrent;
using System.Security.Claims;
using CollabBoard.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using YDotNet.Document;
using YDotNet.Server; // ✅ Required for EncodeStateAsUpdate

namespace CollabBoard.Web.Hubs;

public class CollabHub : Hub<ICollabClient>
{
    private readonly ILogger<CollabHub> _logger;
    private readonly IAuthorizationService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _context;
    // Board state memory (use Redis for scaling)
    private static readonly ConcurrentDictionary<string, Doc> BoardDocs = new();
    private static readonly ConcurrentDictionary<string, string> ConnectionBoardMap = new();

    public CollabHub(ILogger<CollabHub> logger,
    IAuthorizationService authService,
    IHttpContextAccessor httpContextAccessor,
    IApplicationDbContext context)
    {
        _logger = logger;
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    // ---------------- EXISTING METHODS (UNCHANGED) ----------------
    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;

        await Clients.All.UserPresenceChanged(new UserPresenceDto
        {
            ConnectionId = Context.ConnectionId,
            UserName = userName,
            Color = "#999",
            Tool = "select"
        });

        await base.OnConnectedAsync();
    }

    public async Task UpdatePresence(UserPresenceDto dto)
    {
        dto.ConnectionId = Context.ConnectionId;

        await Clients.Others.UserPresenceChanged(new UserPresenceDto
        {
            ConnectionId = dto.ConnectionId,
            UserName = Context.User?.Identity?.Name ?? "Anonymous",
            Color = "#999",
            Tool = "select"
        });
    }

    public async Task MoveCursor(CursorDto dto)
        => await Clients.Others.UserCursorMoved(dto);

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (ConnectionBoardMap.TryRemove(Context.ConnectionId, out var boardId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);
            await Clients.Group(boardId).UserDisconnected(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastYjsUpdate(byte[] update)
        => await Clients.Others.SyncYjsUpdate(update);

    // ---------------- UPDATED & NEW METHODS ----------------

    public async Task JoinYDoc(string boardId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Context.ConnectionId;

        // Permission check
        if (!await HasPermissionToJoinBoard(userId, boardId))
        {
            _logger.LogWarning("User {UserId} denied access to board {BoardId}", userId, boardId);
            Context.Abort();
            return;
        }

        // Join SignalR group
        await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
        ConnectionBoardMap[Context.ConnectionId] = boardId;

        _logger.LogInformation("User {UserId} joined board {BoardId}", userId, boardId);

        // Send current Yjs doc state (binary update)
        //var doc = GetOrCreateBoardDoc(boardId);
        //var update = doc.Map(boardId);
        //byte[] arr = [];
        //await Clients.Caller.SyncYjsUpdate(arr);
    }

    public async Task SendYjsUpdate(string base64Update)
    {
        if (!ConnectionBoardMap.TryGetValue(Context.ConnectionId, out var boardId))
        {
            _logger.LogWarning("SendYjsUpdate failed: user not in board");
            return;
        }

        var update = Convert.FromBase64String(base64Update);
        if (update.Length == 0)
        {
            _logger.LogWarning("Ignored blank Yjs update");
            return;
        }

        const int maxUpdateSize = 500_000;
        if (update.Length > maxUpdateSize)
        {
            _logger.LogWarning("Ignored oversized Yjs update: {0} bytes", update.Length);
            return;
        }

        // Apply Yjs update using YDotNet
        var doc = GetOrCreateBoardDoc(boardId);
        doc.WriteTransaction(update);

        // Broadcast to others
        await Clients.OthersInGroup(boardId).SyncYjsUpdate(update);
    }

    // ---------------- HELPERS ----------------

    private Doc GetOrCreateBoardDoc(string boardId)
    {
        return BoardDocs.GetOrAdd(boardId, _ => new Doc());
    }

    private async Task<bool> HasPermissionToJoinBoard(string userId, string boardIdStr)
    {
        if (!Guid.TryParse(boardIdStr, out var boardId))
            return false;

        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return false;

        var result = await _authService.AuthorizeAsync(user, null, new BoardAccessRequirement(boardId));
        return result.Succeeded;
    }

}
