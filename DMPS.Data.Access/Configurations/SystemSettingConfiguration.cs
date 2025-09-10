using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SystemSetting entity.
/// </summary>
public sealed class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        // Table mapping
        builder.ToTable("system_settings");

        // Primary Key
        builder.HasKey(s => s.SettingKey);

        // Property configurations
        builder.Property(s => s.SettingKey)
            .HasMaxLength(100);

        builder.Property(s => s.SettingValue)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(s => s.Description)
            .HasColumnType("TEXT");

        builder.Property(s => s.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}