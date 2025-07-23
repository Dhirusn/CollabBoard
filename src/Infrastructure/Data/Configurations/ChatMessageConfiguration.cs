using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.Property(cm => cm.Text)
               .HasMaxLength(2000)
               .IsRequired();

        builder.HasOne(cm => cm.Board)
               .WithMany(b => b.ChatMessages)
               .HasForeignKey(cm => cm.BoardId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cm => new { cm.BoardId, cm.SentUtc });
    }
}
