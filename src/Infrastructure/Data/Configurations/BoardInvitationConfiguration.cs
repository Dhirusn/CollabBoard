using CollabBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabBoard.Infrastructure.Data.Configurations;
public class BoardInvitationConfiguration : IEntityTypeConfiguration<BoardInvitation>
{
    public void Configure(EntityTypeBuilder<BoardInvitation> builder)
    {
        builder.Property(i => i.Id)
           .ValueGeneratedOnAdd();

        builder.Property(i => i.RequestedRole)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

        builder.Property(i => i.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

        builder.HasOne(i => i.Board)
                   .WithMany()
                   .HasForeignKey(i => i.BoardId)
                   .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(i => i.InvitedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(i => i.TargetUserId)
                   .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.BoardId, i.TargetUserId, i.Status })
                   .HasDatabaseName("IX_BoardInvitation_Board_Target_Status");
    }
}  
