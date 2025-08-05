using Microsoft.AspNetCore.SignalR;

namespace CollabBoard.Web.Hubs;

public class CollabHub : Hub<ICollabClient>
{
    private readonly ILogger<CollabHub> _logger;
    public CollabHub(ILogger<CollabHub> logger)
    {
        _logger = logger;
    }
    public override async Task OnConnectedAsync()
    {
        // Extract user info from the ClaimsPrincipal (assuming authentication is configured)
        var userName = Context.User?.Identity?.Name ?? "Anonymous";

        // Optionally extract user ID claim, e.g.:
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;

        // Notify everyone (including the caller) that a new user is online with actual info
        await Clients.All.UserPresenceChanged(new UserPresenceDto
        {
            ConnectionId = Context.ConnectionId,
            UserName = userName,
            Color = "#999",  // You can generate or store user colors elsewhere
            Tool = "select"
        });

        await base.OnConnectedAsync();
    }

    public async Task UpdatePresence(UserPresenceDto dto)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";

        // Optionally extract user ID claim, e.g.:
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
        dto.ConnectionId = Context.ConnectionId;
        await Clients.Others.UserPresenceChanged(new UserPresenceDto
        {
            ConnectionId = Context.ConnectionId,
            UserName = userName,
            Color = "#999",  // You can generate or store user colors elsewhere
            Tool = "select"
        });
    }

    public async Task MoveCursor(CursorDto dto)
       => await Clients.Others.UserCursorMoved(dto);

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.UserDisconnected(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastYjsUpdate(byte[] update)
       => await Clients.Others.SyncYjsUpdate(update);

    public async Task JoinYDoc()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "ydoc");
    }

    public async Task SendYjsUpdate(string base64Update)
    {
        var update = Convert.FromBase64String(base64Update);

        const int maxUpdateSize = 500_000;
        if (update.Length > maxUpdateSize)
        {
            _logger.LogWarning("Ignoring large update: {0} bytes", update.Length);
            return;
        }

        await Clients.OthersInGroup("ydoc").SyncYjsUpdate(update);
    }

}

public interface ICollabClient
{
    Task UserPresenceChanged(UserPresenceDto dto);
    Task UserCursorMoved(CursorDto dto);
    Task UserDisconnected(string connectionId);
    Task SyncYjsUpdate(byte[] update);
    Task SyncAwareness(byte[] update);   // ← add this if you need it
}

public class AwarenessState
{
    public int clientId { get; set; }
    public object? state { get; set; }
}


public class UserPresenceDto
{
    public string? ConnectionId { get; set; }
    public string? UserName { get; set; }
    public string? Color { get; set; }
    public string? Tool { get; set; }
}

public class CursorDto
{
    public string? ConnectionId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}
