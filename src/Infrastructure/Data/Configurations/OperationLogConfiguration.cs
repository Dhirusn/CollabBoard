using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.Property(ol => ol.EntityType)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(ol => ol.Operation)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(ol => ol.Delta)
               .HasColumnType("jsonb")
               .IsRequired();

        builder.HasOne(ol => ol.Board)
               .WithMany()
                    .HasForeignKey(o => o.BoardId)
         .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ol => new { ol.BoardId, ol.Timestamp })
               .HasMethod("BRIN");
    }
}
