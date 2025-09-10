using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the UserPreference entity.
/// </summary>
public sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        // Table mapping
        builder.ToTable("user_preferences");

        // Composite Primary Key
        builder.HasKey(up => new { up.UserId, up.PreferenceKey });

        // Property configurations
        builder.Property(up => up.PreferenceKey)
            .HasMaxLength(100);

        builder.Property(up => up.PreferenceValue)
            .IsRequired()
            .HasColumnType("TEXT");
            
        builder.Property(up => up.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(up => up.User)
            .WithMany(u => u.Preferences)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Deleting a user deletes their preferences
    }
}