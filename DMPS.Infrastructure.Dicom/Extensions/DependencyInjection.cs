using DMPS.Infrastructure.Dicom.Anonymization;
using DMPS.Infrastructure.Dicom.Configuration;
using DMPS.Infrastructure.Dicom.Interfaces;
using DMPS.Infrastructure.Dicom.Services;
using DMPS.Infrastructure.Dicom.Storage;
using FellowOakDicom.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DMPS.Infrastructure.Dicom.Extensions
{
    /// <summary>
    /// Provides extension methods for setting up DICOM infrastructure services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all necessary services for DICOM communication (SCP/SCU), file storage, and anonymization.
        /// This method acts as the composition root for the DMPS.Infrastructure.Dicom library.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The application configuration, used to bind DICOM-specific settings.</param>
        /// <returns>The same <see cref="IServiceCollection"/> so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDicomInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure fo-dicom to use the Microsoft.Extensions.Logging infrastructure.
            // This is a one-time setup to bridge the two logging systems.
            // It uses the service provider to get an ILoggerFactory and creates a logger for fo-dicom.
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                LogManager.SetImplementation(new MicrosoftExtensionsLogManager(loggerFactory));
            }

            // Bind configuration sections to strongly-typed options classes.
            // This follows the IOptions pattern for clean, configurable services.
            services.Configure<DicomScpOptions>(configuration.GetSection("Dicom:Scp"));
            services.Configure<TlsOptions>(configuration.GetSection("Dicom:Tls"));

            // Register the DICOM C-STORE SCP service.
            // It's a Singleton because it needs to manage a long-running background listener
            // that persists for the entire lifetime of the host application.
            services.AddSingleton<IDicomScpService, DicomScpService>();

            // Register the DICOM SCU service for client operations (C-FIND, C-MOVE, C-ECHO).
            // It's Scoped because SCU operations are typically part of a specific request or user action.
            // A new instance is created for each scope (e.g., a web request or a specific UI workflow),
            // which is appropriate for a stateless client.
            services.AddScoped<IDicomScuService, DicomScuService>();

            // Register the DICOM file storage service.
            // It's Scoped because file operations are often part of a larger unit of work
            // (e.g., receiving a study and storing all its files).
            services.AddScoped<IDicomFileStorage, DicomFileStorage>();

            // Register the anonymization strategies.
            // These are stateless, pure-logic services, making them ideal Singletons.
            // Registering multiple implementations of the same interface allows the
            // DicomAnonymizer to receive an IEnumerable<IAnonymizationStrategy> via DI.
            services.AddSingleton<IAnonymizationStrategy, BasicAnonymizationStrategy>();
            services.AddSingleton<IAnonymizationStrategy, FullAnonymizationStrategy>();

            // Register the main DICOM anonymizer service.
            // It's a stateless utility that depends on the registered strategies,
            // so it is also registered as a Singleton for efficiency.
            services.AddSingleton<IDicomAnonymizer, DicomAnonymizer>();

            return services;
        }
    }
}