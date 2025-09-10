using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the HangingProtocol entity.
/// </summary>
public sealed class HangingProtocolConfiguration : IEntityTypeConfiguration<HangingProtocol>
{
    public void Configure(EntityTypeBuilder<HangingProtocol> builder)
    {
        // Table mapping
        builder.ToTable("hanging_protocols");

        // Primary Key
        builder.HasKey(h => h.HangingProtocolId);
        builder.Property(h => h.HangingProtocolId).ValueGeneratedOnAdd();
        
        // Property configurations
        builder.Property(h => h.ProtocolName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.LayoutDefinition)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(h => h.Criteria)
            .HasColumnType("jsonb");
            
        builder.Property(h => h.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(h => h.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .IsRequired(false) // Null for system-wide protocols
            .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their custom protocols are deleted
    }
}