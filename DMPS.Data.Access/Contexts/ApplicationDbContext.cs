using System.Reflection;
using DMPS.Shared.Core.Abstractions;
using DMPS.Shared.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMPS.Data.Access.Contexts;

/// <summary>
/// Represents the session with the PostgreSQL database for the DMPS application.
/// This context acts as the central point for all data operations and configuration,
/// effectively serving as the Unit of Work for the data access layer.
/// </summary>
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    /// <summary>
    /// Provides access to and manages the encryption key used for data-at-rest encryption.
    /// While not directly used in this class, it is made available for consumption by
    /// components within the data access layer, such as value converters.
    /// </summary>
    public IEncryptionKeyProvider EncryptionKeyProvider { get; }

    #region DbSets

    /// <summary>
    /// Gets or sets the DbSet for AuditLog entities.
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for AutoRoutingRule entities.
    /// </summary>
    public DbSet<AutoRoutingRule> AutoRoutingRules { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for HangingProtocol entities.
    /// </summary>
    public DbSet<HangingProtocol> HangingProtocols { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Image entities.
    /// </summary>
    public DbSet<Image> Images { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for PacsConfiguration entities.
    /// </summary>
    public DbSet<PacsConfiguration> PacsConfigurations { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for PasswordHistory entities.
    /// </summary>
    public DbSet<PasswordHistory> PasswordHistories { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Patient entities.
    /// </summary>
    public DbSet<Patient> Patients { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for PresentationState entities.
    /// </summary>
    public DbSet<PresentationState> PresentationStates { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for PrintJob entities.
    /// </summary>
    public DbSet<PrintJob> PrintJobs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Role entities.
    /// </summary>
    public DbSet<Role> Roles { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Series entities.
    /// </summary>
    public DbSet<Series> Series { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Study entities.
    /// </summary>
    public DbSet<Study> Studies { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for SystemSetting entities.
    /// </summary>
    public DbSet<SystemSetting> SystemSettings { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for User entities.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for UserPreference entities.
    /// </summary>
    public DbSet<UserPreference> UserPreferences { get; set; } = null!;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by a DbContext.</param>
    /// <param name="encryptionKeyProvider">The provider for the data-at-rest encryption key.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEncryptionKeyProvider encryptionKeyProvider)
        : base(options)
    {
        EncryptionKeyProvider = encryptionKeyProvider ?? throw new ArgumentNullException(nameof(encryptionKeyProvider));
    }

    /// <summary>
    /// Commits all changes made in this context to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Future logic for domain event dispatching can be added here.
        // For now, it directly calls the base implementation.
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Configures the schema needed for the identity framework.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable the pgcrypto extension in PostgreSQL, which is required for data-at-rest encryption.
        // This is a critical prerequisite for REQ-1-083.
        modelBuilder.HasPostgresExtension("pgcrypto");

        // Automatically discover and apply all IEntityTypeConfiguration classes from this assembly.
        // This keeps the DbContext clean and delegates mapping concerns to dedicated configuration classes.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}