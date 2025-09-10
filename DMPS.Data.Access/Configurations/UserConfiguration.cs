using DMPS.Shared.Core.Domain.Entities;
using DMPS.Data.Access.Services;
using DMPS.Data.Access.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the User entity.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly IEncryptionKeyProvider _encryptionKeyProvider;

    public UserConfiguration(IEncryptionKeyProvider encryptionKeyProvider)
    {
        _encryptionKeyProvider = encryptionKeyProvider;
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table mapping
        builder.ToTable("users");

        // Primary Key
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId).ValueGeneratedOnAdd();

        // Property configurations
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
            
        // Encrypted PHI Fields
        var encryptedStringConverter = new EncryptedStringConverter(_encryptionKeyProvider);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasConversion(encryptedStringConverter);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasConversion(encryptedStringConverter);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.IsTemporaryPassword)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.PasswordLastChangedAt)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(u => u.Username)
            .IsUnique();

        // Relationships
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent role deletion if users are assigned
    }
}