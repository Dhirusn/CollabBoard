using System.Reflection;
using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollabBoard.Infrastructure.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardMember> BoardsMember => Set<BoardMember>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Snapshot> Snapshots => Set<Snapshot>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
