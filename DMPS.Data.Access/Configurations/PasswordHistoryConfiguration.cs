using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the PasswordHistory entity.
/// </summary>
public sealed class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        // Table mapping
        builder.ToTable("password_histories");

        // Primary Key
        builder.HasKey(p => p.PasswordHistoryId);
        builder.Property(p => p.PasswordHistoryId).ValueGeneratedOnAdd();

        // Property configurations
        builder.Property(p => p.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(p => new { p.UserId, p.CreatedAt });

        // Relationships
        builder.HasOne(p => p.User)
            .WithMany(u => u.PasswordHistories)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their password history is also deleted.
    }
}