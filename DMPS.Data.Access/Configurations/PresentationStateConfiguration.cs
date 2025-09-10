using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the PresentationState entity.
/// </summary>
public sealed class PresentationStateConfiguration : IEntityTypeConfiguration<PresentationState>
{
    public void Configure(EntityTypeBuilder<PresentationState> builder)
    {
        // Table mapping
        builder.ToTable("presentation_states");

        // Primary Key
        builder.HasKey(ps => ps.PresentationStateId);
        builder.Property(ps => ps.PresentationStateId).ValueGeneratedOnAdd();
        
        // Property configurations
        builder.Property(ps => ps.SopInstanceUid)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(ps => ps.FilePath)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(ps => ps.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(ps => ps.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(ps => ps.SopInstanceUid)
            .IsUnique();
        
        // Relationships
        builder.HasOne(ps => ps.Series)
            .WithMany(s => s.PresentationStates)
            .HasForeignKey(ps => ps.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.CreatedByUser)
            .WithMany()
            .HasForeignKey(ps => ps.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull); // Keep the presentation state even if the creating user is deleted
    }
}