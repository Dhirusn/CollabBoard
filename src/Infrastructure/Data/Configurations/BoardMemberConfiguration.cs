using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class BoardMemberConfiguration : IEntityTypeConfiguration<BoardMember>
{
    public void Configure(EntityTypeBuilder<BoardMember> builder)
    {
        builder.HasKey(bm => bm.Id);

        builder.HasIndex(bm => new { bm.BoardId, bm.UserId })
               .IsUnique();

        builder.HasOne(bm => bm.Board)
               .WithMany(b => b.Members)
               .HasForeignKey(bm => bm.BoardId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bm => bm.User)
               .WithMany()
               .HasForeignKey(bm => bm.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
