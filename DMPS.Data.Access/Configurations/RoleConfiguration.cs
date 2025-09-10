using DMPS.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMPS.Data.Access.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Role entity.
/// </summary>
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table mapping
        builder.ToTable("roles");

        // Primary Key
        builder.HasKey(r => r.RoleId);
        builder.Property(r => r.RoleId).ValueGeneratedOnAdd();

        // Property configurations
        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasColumnType("TEXT");

        // Indexes
        builder.HasIndex(r => r.RoleName)
            .IsUnique();
            
        // Seed Data
        builder.HasData(
            new Role { RoleId = Guid.NewGuid(), RoleName = "Administrator", Description = "Full system access and administrative privileges." },
            new Role { RoleId = Guid.NewGuid(), RoleName = "Technician", Description = "Standard operational access for viewing and printing studies." }
        );
    }
}