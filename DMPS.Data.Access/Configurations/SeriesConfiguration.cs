using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Series entity.
/// </summary>
public sealed class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        // Table mapping
        builder.ToTable("series");

        // Primary Key
        builder.HasKey(s => s.SeriesId);
        builder.Property(s => s.SeriesId).ValueGeneratedOnAdd();

        // Property configurations
        builder.Property(s => s.SeriesInstanceUid)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.Modality)
            .HasMaxLength(16);

        builder.Property(s => s.SeriesNumber);

        builder.Property(s => s.SeriesDescription)
            .HasColumnType("TEXT");

        builder.Property(s => s.BodyPartExamined)
            .HasMaxLength(64);
            
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(s => s.SeriesInstanceUid)
            .IsUnique();

        builder.HasIndex(s => s.Modality);

        // Relationships
        builder.HasOne(s => s.Study)
            .WithMany(study => study.Series)
            .HasForeignKey(s => s.StudyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Images)
            .WithOne(i => i.Series)
            .HasForeignKey(i => i.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.PresentationStates)
            .WithOne(ps => ps.Series)
            .HasForeignKey(ps => ps.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}