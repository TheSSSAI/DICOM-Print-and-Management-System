using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the PrintJob entity.
/// </summary>
public sealed class PrintJobConfiguration : IEntityTypeConfiguration<PrintJob>
{
    public void Configure(EntityTypeBuilder<PrintJob> builder)
    {
        // Table mapping
        builder.ToTable("print_jobs");

        // Primary Key
        builder.HasKey(p => p.PrintJobId);
        builder.Property(p => p.PrintJobId).ValueGeneratedOnAdd();
        
        // Property configurations
        builder.Property(p => p.JobPayload)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Queued");

        builder.Property(p => p.PrinterName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.FailureReason)
            .HasColumnType("TEXT");

        builder.Property(p => p.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.SubmittedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.ProcessedAt);
        
        // Indexes
        builder.HasIndex(p => new { p.Status, p.Priority, p.SubmittedAt })
            .HasDatabaseName("ix_printjob_polling");
            
        // Relationships
        builder.HasOne(p => p.SubmittedByUser)
            .WithMany()
            .HasForeignKey(p => p.SubmittedByUserId)
            .OnDelete(DeleteBehavior.SetNull); // Keep print job history even if user is deleted
    }
}