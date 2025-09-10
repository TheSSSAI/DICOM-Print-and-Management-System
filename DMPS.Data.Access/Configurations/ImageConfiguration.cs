using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Image entity.
/// </summary>
public sealed class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        // Table mapping
        builder.ToTable("images");

        // Primary Key
        builder.HasKey(i => i.ImageId);
        builder.Property(i => i.ImageId).ValueGeneratedOnAdd();

        // Property configurations
        builder.Property(i => i.SopInstanceUid)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(i => i.InstanceNumber);

        builder.Property(i => i.FilePath)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(i => i.FileSize)
            .IsRequired();
            
        builder.Property(i => i.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(i => i.SopInstanceUid)
            .IsUnique();

        builder.HasIndex(i => new { i.SeriesId, i.InstanceNumber })
            .IsUnique(false); // Can be non-unique if instance numbers are repeated, but sorted on

        // Query filter for soft delete
        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.HasQueryFilter(i => !i.IsDeleted);

        // Relationships
        builder.HasOne(i => i.Series)
            .WithMany(s => s.Images)
            .HasForeignKey(i => i.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}