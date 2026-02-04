using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class SnapshotConfiguration : IEntityTypeConfiguration<Snapshot>
{
    public void Configure(EntityTypeBuilder<Snapshot> builder)
    {
        builder.HasIndex(s => new { s.BoardId, s.TakenUtc });

        builder.Property(s => s.PagesBlob)
               .HasColumnType("jsonb")
               .IsRequired();

        builder.HasOne(s => s.Board)
                     .WithMany(b => b.Snapshots)
                     .HasForeignKey(s => s.BoardId)
                     .OnDelete(DeleteBehavior.Cascade);

        builder.Property<string>("YjsState")
               .HasColumnName("YjsState")
               .HasColumnType("text")
               .IsRequired(false);

    }
}
