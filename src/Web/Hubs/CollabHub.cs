using Microsoft.AspNetCore.SignalR;
using static CollabBoard.Web.Hubs.CollabHub;

namespace CollabBoard.Web.Hubs;

public class CollabHub : Hub<ICollabClient>
{
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
        dto.ConnectionId = Context.ConnectionId;
        await Clients.Others.UserPresenceChanged(dto);
    }

    public async Task MoveCursor(CursorDto cursor)
    {
        cursor.ConnectionId = Context.ConnectionId;
        await Clients.Others.UserCursorMoved(cursor);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.UserDisconnected(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastYjsUpdate(byte[] update)
       => await Clients.Others.SyncYjsUpdate(update);

    public async Task BroadcastAwareness(string roomId, byte[] update)
    {
        // Forward the raw update to all others in the same room (or all if no groups)
        await Clients.Others.SendAsync("SyncAwareness", roomId, update);
    }



}

public interface ICollabClient
{
    Task UserPresenceChanged(UserPresenceDto dto);
    Task UserCursorMoved(CursorDto dto);
    Task UserDisconnected(string connectionId);
    Task SyncYjsUpdate(byte[] update);

    Task SendAsync(string name, string roomId, byte[] update);
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
