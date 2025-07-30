using Microsoft.AspNetCore.SignalR;

namespace CollabBoard.Web.Hubs;

public class CollabHub : Hub
{
    public async Task BroadcastUpdate(string roomId, byte[] update)
    {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveUpdate", update);
    }

    public async Task BroadcastAwareness(string roomId, byte[] awarenessUpdate)
    {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveAwareness", awarenessUpdate);
    }

    public override async Task OnConnectedAsync()
    {
        var roomId = Context.GetHttpContext()!.Request.Query["roomId"];
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId!);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var roomId = Context.GetHttpContext()!.Request.Query["roomId"];
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId!);
        await base.OnDisconnectedAsync(exception);
    }
}
