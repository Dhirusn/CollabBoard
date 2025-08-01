using Microsoft.AspNetCore.SignalR;
using static CollabBoard.Web.Hubs.CollabHub;

namespace CollabBoard.Web.Hubs;

public class CollabHub : Hub<ICollabClient>
{
    public override async Task OnConnectedAsync()
    {
        // Notify everyone (including the caller) that a new user is online
        await Clients.All.UserPresenceChanged(new UserPresenceDto
        {
            ConnectionId = Context.ConnectionId,
            UserName = "Anonymous",          // or pull from auth context
            Color = "#999",
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
}

public interface ICollabClient
{
    Task UserPresenceChanged(UserPresenceDto dto);
    Task UserCursorMoved(CursorDto dto);
    Task UserDisconnected(string connectionId);
    Task SyncYjsUpdate(byte[] update);
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
