using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class ContentBlockConfiguration : IEntityTypeConfiguration<ContentBlock>
{
    public void Configure(EntityTypeBuilder<ContentBlock> builder)
    {
        builder.Property(cb => cb.Type)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(cb => cb.Payload)
               .HasColumnType("jsonb")
               .IsRequired();

        builder.Property(cb => cb.Style)
               .HasColumnType("jsonb");

        builder.Property(cb => cb.Version)
               .IsRowVersion()
               .HasDefaultValue(1);

        builder.HasOne(cb => cb.Page)
               .WithMany(p => p.Blocks)
               .HasForeignKey(cb => cb.PageId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cb => new { cb.PageId, cb.UpdatedUtc })
               .IncludeProperties(cb => new { cb.X, cb.Y, cb.Width, cb.Height });

        builder.HasIndex(cb => cb.Payload)
               .HasMethod("GIN")
               .HasOperators("jsonb_path_ops");
    }
}
