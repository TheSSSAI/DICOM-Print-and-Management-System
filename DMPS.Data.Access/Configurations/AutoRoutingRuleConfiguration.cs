using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the AutoRoutingRule entity.
/// </summary>
public sealed class AutoRoutingRuleConfiguration : IEntityTypeConfiguration<AutoRoutingRule>
{
    public void Configure(EntityTypeBuilder<AutoRoutingRule> builder)
    {
        // Table mapping
        builder.ToTable("auto_routing_rules");

        // Primary Key
        builder.HasKey(r => r.AutoRoutingRuleId);
        builder.Property(r => r.AutoRoutingRuleId).ValueGeneratedOnAdd();

        // Property configurations
        builder.Property(r => r.RuleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Criteria)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(r => r.DestinationPath)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(r => r.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(r => r.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        // Indexes
        builder.HasIndex(r => r.Criteria)
            .HasMethod("gin");
    }
}