using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the AuditLog entity.
/// </summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // Table mapping
        builder.ToTable("audit_logs");

        // Primary Key
        builder.HasKey(a => a.AuditLogId);
        builder.Property(a => a.AuditLogId).UseIdentityAlwaysColumn();

        // Property configurations
        builder.Property(a => a.EventTimestamp)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(a => a.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityName)
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .HasMaxLength(128);

        builder.Property(a => a.Details)
            .HasColumnType("jsonb");

        // Indexes for efficient querying
        builder.HasIndex(a => new { a.EventTimestamp, a.EventType });
        builder.HasIndex(a => new { a.UserId, a.EventTimestamp });
        builder.HasIndex(a => a.CorrelationId);

        // Relationships
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull); // Keep audit log even if user is deleted
    }
}