// CollabBoard.Web.Endpoints.Boards.cs
using CollabBoard.Application.Boards.Commands.CreateBoard;
using CollabBoard.Application.Boards.Commands.UpdateBoardTitle;
using CollabBoard.Application.Boards.Commands.DeleteBoard;
using CollabBoard.Application.Boards.Queries.GetBoardSnapshot;
using CollabBoard.Application.Boards.Commands.AddPage;
using CollabBoard.Application.Boards.Commands.UpsertContentBlock;
using CollabBoard.Application.Boards.Commands.DeleteContentBlock;
using CollabBoard.Application.Boards.Queries.GetBoardMembers;
using CollabBoard.Application.Boards.Commands.InviteMember;
using CollabBoard.Application.Boards.Commands.SendChatMessage;
using CollabBoard.Application.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using CollabBoard.Application.Chat.Queries.GetChatMessages;

namespace CollabBoard.Web.Endpoints;
public class Boards : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetBoardSnapshot, "{id}")
             .MapPost(CreateBoard)
            .MapPut(UpdateBoardTitle,"{id}/title")
            .MapDelete(DeleteBoard, "{id}")

            // Pages
            .MapPost(AddPage, "{id}/pages")

            // Content blocks (real-time sync companion)
            .MapPut(UpsertContentBlock, "{id}/blocks")
            .MapDelete(DeleteContentBlock, "{id}/blocks/{blockId}")

            // Members & presence
            .MapGet(GetBoardMembers, "{id}/members")
            .MapPost(InviteMember, "{id}/members")

            // Chat
            .MapGet(GetChatMessages, "{id}/chat")
            .MapPost(SendChatMessage, "{id}/chat"); ;
    }

    /* ==========================  BOARD  ========================== */

    public async Task<Ok<BoardSnapshotDto>> GetBoardSnapshot(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetBoardSnapshotQuery(id));
        return TypedResults.Ok(result);
    }

    public async Task<Created<Guid>> CreateBoard(ISender sender, CreateBoardCommand command)
    {
        var boardId = await sender.Send(command);
        return TypedResults.Created($"/boards/{boardId}", boardId);
    }

    public async Task<NoContent> UpdateBoardTitle(ISender sender, Guid id, UpdateBoardTitleCommand command)
    {
        command = command with { Id = id };
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    public async Task<NoContent> DeleteBoard(ISender sender, Guid id)
    {
        await sender.Send(new DeleteBoardCommand(id));
        return TypedResults.NoContent();
    }

    /* ==========================  PAGES  ========================== */

    public async Task<Created<Guid>> AddPage(ISender sender, Guid id, AddPageCommand command)
    {
        command = command with { BoardId = id };
        var pageId = await sender.Send(command);
        return TypedResults.Created($"/boards/{id}/pages/{pageId}", pageId);
    }

    /* ==========================  CONTENT BLOCKS  ========================== */

    public async Task<Created<Guid>> UpsertContentBlock(ISender sender, Guid id, UpsertContentBlockCommand command)
    {
        command = command with { BoardId = id };
        var blockId = await sender.Send(command);
        return TypedResults.Created($"/boards/{id}/blocks/{blockId}", blockId);
    }

    public async Task<NoContent> DeleteContentBlock(ISender sender, Guid id, Guid blockId)
    {
        await sender.Send(new DeleteContentBlockCommand(id, blockId));
        return TypedResults.NoContent();
    }

    /* ==========================  MEMBERS  ========================== */

    public async Task<Ok<List<BoardMemberDto>>> GetBoardMembers(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetBoardMembersQuery(id));
        return TypedResults.Ok(result);
    }

    public async Task<Created<Guid>> InviteMember(ISender sender, Guid id, InviteMemberCommand command)
    {
        command = command with { BoardId = id };
        var memberId = await sender.Send(command);
        return TypedResults.Created($"/boards/{id}/members/{memberId}", memberId);
    }

    /* ==========================  CHAT  ========================== */

    public async Task<Ok<PaginatedList<ChatMessageDto>>> GetChatMessages(ISender sender, Guid id, [AsParameters] GetChatMessagesQuery query)
    {
        var result = await sender.Send(query with { BoardId = id });
        return TypedResults.Ok(result);
    }

    public async Task<Created<Guid>> SendChatMessage(ISender sender, Guid id, SendChatMessageCommand command)
    {
        command = command with { BoardId = id };
        var msgId = await sender.Send(command);
        return TypedResults.Created($"/boards/{id}/chat/{msgId}", msgId);
    }
}
