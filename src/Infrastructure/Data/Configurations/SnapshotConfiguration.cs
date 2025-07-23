using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class SnapshotConfiguration : IEntityTypeConfiguration<Snapshot>
{
    public void Configure(EntityTypeBuilder<Snapshot> builder)
    {
        builder.Property(s => s.PagesBlob)
               .HasColumnType("jsonb")
               .IsRequired();

        builder.HasOne<Board>()
               .WithMany(b => b.Snapshots)
               .HasForeignKey(s => s.BoardId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.BoardId, s.TakenUtc });

        builder.HasOne(s => s.Board)
                .WithMany()
                .HasForeignKey(s => s.BoardId)
               .OnDelete(DeleteBehavior.Cascade);

    }
}
