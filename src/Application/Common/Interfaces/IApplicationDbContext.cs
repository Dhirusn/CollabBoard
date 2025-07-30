using CollabBoard.Domain.Entities;

namespace CollabBoard.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    public DbSet<Board> Boards { get; }
    public DbSet<BoardInvitation> BoardInvitations { get; }
    public DbSet<BoardMember> BoardsMembers { get; }
    public DbSet<ChatMessage> ChatMessages { get; }
    public DbSet<ContentBlock> ContentBlocks { get; }
    public DbSet<OperationLog> OperationLogs { get; }
    public DbSet<Page> Pages { get; }
    public DbSet<Snapshot> Snapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
