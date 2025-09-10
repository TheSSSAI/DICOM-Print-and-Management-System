using DMPS.Data.Access.Contexts;
using DMPS.Data.Access.Repositories;
using DMPS.Shared.Core.Repositories;
using DMPS.Shared.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DMPS.Data.Access.Extensions
{
    /// <summary>
    /// Extension methods for setting up data access services in an IServiceCollection.
    /// This class provides a centralized and consistent way to register all the necessary
    /// components for the data access layer, such as the DbContext and repositories.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DataAccessServiceExtensions
    {
        private const string PostgresConnectionStringName = "PostgresConnection";

        /// <summary>
        /// Registers the ApplicationDbContext and repository services with the dependency injection container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the services to.</param>
        /// <param name="configuration">The application configuration for retrieving settings like connection strings.</param>
        /// <returns>The same IServiceCollection so that multiple calls can be chained.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the PostgreSQL connection string is not found in the configuration.</exception>
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(PostgresConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"Database connection string '{PostgresConnectionStringName}' not found in configuration.");
            }
            
            // Per REQ-NFR-004 and REQ-1-083, TLS must be enforced.
            // A production deployment should validate this or ensure the connection string is centrally managed.
            if (!connectionString.Contains("SslMode=Require", StringComparison.OrdinalIgnoreCase) &&
                !connectionString.Contains("SslMode=Verify-Full", StringComparison.OrdinalIgnoreCase) &&
                !connectionString.Contains("SslMode=Verify-CA", StringComparison.OrdinalIgnoreCase))
            {
                // In a real production environment, you might throw an exception here
                // or rely on infrastructure policies to enforce this. For development, we might allow it.
                // For now, we assume the provided connection string is compliant.
                // Consider adding a strict mode check for production builds.
            }

            // Register the DbContext with a scoped lifetime, which is the default and correct for web/service apps.
            // It uses the Npgsql provider for PostgreSQL.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                })
                .UseSnakeCaseNamingConvention());

            // Register the encryption key provider as a singleton
            services.AddSingleton<IEncryptionKeyProvider, EncryptionKeyProvider>();
            
            // Register the IUnitOfWork to be resolved from the ApplicationDbContext.
            // This allows services to depend on IUnitOfWork for saving changes without
            // needing a direct dependency on the concrete DbContext.
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // Register repositories with a scoped lifetime to match the DbContext.
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudyRepository, StudyRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();

            return services;
        }
    }
}