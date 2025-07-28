using Microsoft.AspNetCore.SignalR;

namespace CollabBoard.Web.Hubs;

public class CollabHub : Hub
{
    public async Task JoinBoard(Guid boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
    }

    public async Task SendBlockEdit(Guid boardId, object block)
    {
        await Clients.OthersInGroup(boardId.ToString()).SendAsync("ReceiveBlockEdit", block);
    }

    public async Task SendBlockDelete(Guid boardId, Guid blockId)
    {
        await Clients.OthersInGroup(boardId.ToString()).SendAsync("ReceiveBlockDelete", blockId);
    }

    public async Task SendCursorMove(Guid boardId, string userId, object cursor)
    {
        await Clients.OthersInGroup(boardId.ToString()).SendAsync("ReceiveCursorMove", userId, cursor);
    }

    public async Task Undo(Guid boardId)
    {
        await Clients.OthersInGroup(boardId.ToString()).SendAsync("ReceiveUndo");
    }

    public async Task Redo(Guid boardId)
    {
        await Clients.OthersInGroup(boardId.ToString()).SendAsync("ReceiveRedo");
    }

    public async Task SendChatMessage(Guid boardId, object message)
    {
        await Clients.OthersInGroup(boardId.ToString()).SendAsync("ReceiveChatMessage", message);
    }
}
