using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.Property(p => p.OrderIndex)
               .IsRequired();

        builder.HasOne(p => p.Board)
               .WithMany(b => b.Pages)
               .HasForeignKey(p => p.BoardId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.BoardId, p.OrderIndex });
    }
}
