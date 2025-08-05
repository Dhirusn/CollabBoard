// CollabBoard.Web.Endpoints.Boards.cs  
using System.Security.Claims;
using CollabBoard.Application.Boards.Commands.AcceptInvitation;
using CollabBoard.Application.Boards.Commands.AddPage;
using CollabBoard.Application.Boards.Commands.CreateBoard;
using CollabBoard.Application.Boards.Commands.DeleteBoard;
using CollabBoard.Application.Boards.Commands.InviteMember;
using CollabBoard.Application.Boards.Commands.JoinBoard;
using CollabBoard.Application.Boards.Commands.RejectInvitation;
using CollabBoard.Application.Boards.Commands.SendChatMessage;
using CollabBoard.Application.Boards.Commands.UpdateBoardTitle;
using CollabBoard.Application.Boards.Queries.GetBoardByIdQuery;
using CollabBoard.Application.Boards.Queries.GetBoardMembers;
using CollabBoard.Application.Boards.Queries.GetBoardsForUserQuery;
using CollabBoard.Application.Boards.Queries.GetBoardSnapshot;
using CollabBoard.Application.Boards.Queries.GetMyInvitationsQuery;
using CollabBoard.Application.Boards.Queries.SearchUsersQuery;
using CollabBoard.Application.Chat.Queries.GetChatMessages;
using CollabBoard.Application.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CollabBoard.Web.Endpoints;

public class Boards : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
     .RequireAuthorization()

     // Board CRUD
     .MapGet(GetBoardsForUser, "")
     .MapPost(CreateBoard, "")
     .MapGet(GetBoardSnapshot, "{id:guid}/snapshot")
     .MapGet(GetBoardById, "{id:guid}")
     .MapPut(UpdateBoardTitle, "{id:guid}/title")
     .MapDelete(DeleteBoard, "{id:guid}")

     // Pages  
     .MapPost(AddPage, "{id:guid}/pages")

     // Content blocks  
     //.MapPost(AddContentBlock, "{id:guid}/pages/{pageId:guid}/blocks")
     //.MapPut(UpdateContentBlock, "{id:guid}/pages/{pageId:guid}/blocks/{blockId:guid}")
     //.MapDelete(DeleteContentBlock, "{id:guid}/pages/{pageId:guid}/blocks/{blockId:guid}")

     // Operation log / undo-redo  
     //.MapGet(GetOperationLog, "{id:guid}/operations")
     //.MapPost(Undo, "{id:guid}/undo")
     //.MapPost(Redo, "{id:guid}/redo")

     // Members  
     .MapGet(GetBoardMembers, "{id:guid}/members")
     .MapGet(SearchUsers, "searchmembers")
     .MapGet(GetMyInvitations, "myInvitations")
     .MapPost(RequestBoardMember, "requestinvite")
     .MapPost(AcceptInvitation, "{id:guid}/accept")
     .MapPost(RejectInvitation, "{id:guid}/members/{memberId:guid}/reject")

     // Chat  
     .MapGet(GetChatMessages, "{id:guid}/chat")
     .MapPost(SendChatMessage, "{id:guid}/chat");



    }

    public async Task<Ok<BoardSnapshotDto>> GetBoardSnapshot(ISender sender, string id)
    {
        var result = await sender.Send(new GetBoardSnapshotQuery(id));
        return TypedResults.Ok(result);
    }

    public async Task<Ok<BoardDto>> GetBoardById(ISender sender, Guid id)
    {
        var board = await sender.Send(new GetBoardByIdQuery(id));
        return TypedResults.Ok(board);
    }

    public async Task<Ok<List<BoardSummaryDto>>> GetBoardsForUser(ISender sender, string userId)
    {
        var boards = await sender.Send(new GetBoardsForUserQuery(userId));
        return TypedResults.Ok(boards);
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

    // ---------------- Pages ----------------  

    public async Task<Created<Guid>> AddPage(ISender sender, Guid id, AddPageCommand command)
    {
        command = command with { BoardId = id };
        var pageId = await sender.Send(command);
        return TypedResults.Created($"/boards/{id}/pages/{pageId}", pageId);
    }

    //public async Task<Ok<PageContentDto>> GetPageContent(ISender sender, Guid boardId, Guid pageId)
    //{
    //    var content = await sender.Send(new GetPageContentQuery(boardId, pageId));
    //    return TypedResults.Ok(content);
    //}

    // ---------------- Content Blocks ----------------  

    //public async Task<Created<Guid>> AddContentBlock(
    //    ISender sender, Guid boardId, Guid pageId, AddContentBlockCommand command)
    //{
    //    command = command with { BoardId = boardId, PageId = pageId };
    //    var blockId = await sender.Send(command);
    //    return TypedResults.Created($"/boards/{boardId}/pages/{pageId}/blocks/{blockId}", blockId);
    //}

    //public async Task<NoContent> UpdateContentBlock(
    //    ISender sender, Guid boardId, Guid pageId, Guid blockId, UpdateContentBlockCommand command)
    //{
    //    command = command with { BoardId = boardId, PageId = pageId, Id = blockId };
    //    await sender.Send(command);
    //    return TypedResults.NoContent();
    //}

    //public async Task<NoContent> DeleteContentBlock(
    //    ISender sender, Guid boardId, Guid pageId, Guid blockId)
    //{
    //    await sender.Send(new DeleteContentBlockCommand(boardId, pageId, blockId));
    //    return TypedResults.NoContent();
    //}

    // ---------------- Operation Log / Undo-Redo ----------------  

    //public async Task<Ok<OperationLogDto>> GetOperationLog(ISender sender, Guid id)
    //{
    //    var log = await sender.Send(new GetOperationLogForBoardQuery(id));
    //    return TypedResults.Ok(log);
    //}

    //public async Task<NoContent> Undo(ISender sender, Guid id)
    //{
    //    await sender.Send(new UndoOperationCommand(id));
    //    return TypedResults.NoContent();
    //}

    //public async Task<NoContent> Redo(ISender sender, Guid id)
    //{
    //    await sender.Send(new RedoOperationCommand(id));
    //    return TypedResults.NoContent();
    //}

    // ---------------- Members ----------------  

    public async Task<Ok<List<BoardMemberDto>>> GetBoardMembers(ISender sender, Guid id)
    {
        var members = await sender.Send(new GetBoardMembersQuery(id));
        return TypedResults.Ok(members);
    }

    public async Task<Ok<List<UserDto>>> SearchUsers(ISender sender, string query)
    {
        var result = await sender.Send(new SearchUsersQuery(query));
        return TypedResults.Ok(result);
    }

    public async Task<Ok<Guid>> RequestBoardMember(ISender sender, InviteMemberCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<List<InvitationDto>>> GetMyInvitations(ISender sender)
    {
        var members = await sender.Send(new GetMyInvitationsQuery());
        return TypedResults.Ok(members);
    }


    public async Task<Created<Guid>> AcceptInvitation(ISender sender, Guid id)
    {
        var memberId = await sender.Send((new AcceptInvitationCommand(id)));
        return TypedResults.Created($"/boards/{memberId}/members/{memberId}", memberId);
    }

    public async Task<NoContent> RejectInvitation(ISender sender, RejectInvitationCommand command)
    {
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    // ---------------- Chat ----------------  

    public async Task<Ok<PaginatedList<ChatMessageDto>>> GetChatMessages(
        ISender sender, Guid id, [AsParameters] GetChatMessagesQuery query)
    {
        var result = await sender.Send(query with { BoardId = id });
        return TypedResults.Ok(result);
    }

    public async Task<Created<Guid>> SendChatMessage(
        ISender sender, Guid id, SendChatMessageCommand command)
    {
        command = command with { BoardId = id };
        var msgId = await sender.Send(command);
        return TypedResults.Created($"/boards/{id}/chat/{msgId}", msgId);
    }
}
