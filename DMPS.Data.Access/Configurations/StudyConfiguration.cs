using DMPS.Shared.Core.Domain.Entities;
using DMPS.Data.Access.Services;
using DMPS.Data.Access.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Study entity.
/// </summary>
public sealed class StudyConfiguration : IEntityTypeConfiguration<Study>
{
    private readonly IEncryptionKeyProvider _encryptionKeyProvider;

    public StudyConfiguration(IEncryptionKeyProvider encryptionKeyProvider)
    {
        _encryptionKeyProvider = encryptionKeyProvider;
    }

    public void Configure(EntityTypeBuilder<Study> builder)
    {
        // Table mapping
        builder.ToTable("studies");

        // Primary Key
        builder.HasKey(s => s.StudyId);
        builder.Property(s => s.StudyId).ValueGeneratedOnAdd();
        
        // Property configurations
        builder.Property(s => s.StudyInstanceUid)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.StudyDate);

        builder.Property(s => s.StudyDescription)
            .HasColumnType("TEXT");

        // Value converter for encryption
        var encryptedStringConverter = new EncryptedStringConverter(_encryptionKeyProvider);
        
        builder.Property(s => s.AccessionNumber)
            .HasConversion(encryptedStringConverter);

        builder.Property(s => s.ReferringPhysicianName)
            .HasConversion(encryptedStringConverter);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(s => s.StudyInstanceUid)
            .IsUnique();

        builder.HasIndex(s => new { s.PatientId, s.StudyDate });

        // Query filter for soft delete
        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.HasQueryFilter(s => !s.IsDeleted);

        // Relationships
        builder.HasOne(s => s.Patient)
            .WithMany(p => p.Studies)
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.Cascade); // Deleting a patient cascades to studies

        builder.HasMany(s => s.Series)
            .WithOne(ser => ser.Study)
            .HasForeignKey(ser => ser.StudyId)
            .OnDelete(DeleteBehavior.Cascade); // Deleting a study cascades to series
    }
}