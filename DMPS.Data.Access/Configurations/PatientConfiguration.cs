using DMPS.Shared.Core.Domain.Entities;
using DMPS.Data.Access.Services;
using DMPS.Data.Access.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Patient entity.
/// This configuration is critical for security as it implements pgcrypto encryption for all PHI fields.
/// </summary>
public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    private readonly IEncryptionKeyProvider _encryptionKeyProvider;

    public PatientConfiguration(IEncryptionKeyProvider encryptionKeyProvider)
    {
        _encryptionKeyProvider = encryptionKeyProvider;
    }

    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Table mapping
        builder.ToTable("patients");

        // Primary Key
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).ValueGeneratedOnAdd();
        
        // Value converter for encryption
        var encryptedStringConverter = new EncryptedStringConverter(_encryptionKeyProvider);

        // Property configurations with encryption for PHI
        builder.Property(p => p.DicomPatientId)
            .IsRequired()
            .HasConversion(encryptedStringConverter);

        builder.Property(p => p.PatientName)
            .HasConversion(encryptedStringConverter);

        builder.Property(p => p.PatientBirthDate)
            .HasConversion(encryptedStringConverter);

        builder.Property(p => p.PatientSex)
            .HasConversion(encryptedStringConverter);
        
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes for encrypted fields to support searching
        // Note: PostgreSQL GIN with pg_trgm is needed for efficient LIKE queries on encrypted text.
        // The encrypted value will be bytea, so we must index it appropriately.
        builder.HasIndex(p => p.DicomPatientId)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder.HasIndex(p => p.PatientName)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
            
        // Relationships
        builder.HasMany(p => p.Studies)
            .WithOne(s => s.Patient)
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.Cascade); // Deleting a patient deletes their studies
    }
}