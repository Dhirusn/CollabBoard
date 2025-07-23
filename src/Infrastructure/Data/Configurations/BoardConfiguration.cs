using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.Property(b => b.Title)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasOne(b => b.Owner)
               .WithMany()
               .HasForeignKey(b => b.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.OwnerId);
    }
}
