using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the PacsConfiguration entity.
/// </summary>
public sealed class PacsConfigurationConfiguration : IEntityTypeConfiguration<PacsConfiguration>
{
    public void Configure(EntityTypeBuilder<PacsConfiguration> builder)
    {
        // Table mapping
        builder.ToTable("pacs_configurations");

        // Primary Key
        builder.HasKey(p => p.PacsConfigurationId);
        builder.Property(p => p.PacsConfigurationId).ValueGeneratedOnAdd();
        
        // Property configurations
        builder.Property(p => p.AeTitle)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(p => p.Hostname)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Port)
            .IsRequired();

        builder.Property(p => p.SupportsCFind)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(p => p.SupportsCMove)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(p => p.SupportsCStore)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}