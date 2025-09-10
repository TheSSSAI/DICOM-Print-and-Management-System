using DMPS.Client.Application.Interfaces;
using DMPS.Client.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DMPS.Client.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for setting up application services in an <see cref="IServiceCollection"/>.
    /// This class acts as the composition root for the application layer, ensuring that the hosting
    /// application (e.g., the WPF client) can register all necessary services with a single call.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all application-layer services with the dependency injection container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registering the application state service as a singleton to ensure a single, consistent
            // state is maintained throughout the application's lifetime. This is crucial for tracking
            // the current user session and application-wide status.
            services.AddSingleton<IApplicationStateService, ApplicationStateService>();

            // The AuthenticationService manages the global user session and identity. It needs to be a
            // singleton to provide consistent authentication state across all parts of the application.
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // The PrintJobService is stateless; its primary role is to act as a factory for creating
            // command messages and passing them to the message producer. A singleton instance is efficient
            // as it avoids repeated object creation.
            services.AddSingleton<IPrintJobService, PrintJobService>();

            // The SystemStatusService is also stateless. It provides a utility function to check the
            // health of the background service. A singleton ensures that the underlying communication
            // client (if it has state) can be managed effectively.
            services.AddSingleton<ISystemStatusService, SystemStatusService>();

            // License validation state and grace period logic are global concerns. A singleton instance
            // ensures that this state is managed centrally and consistently from application startup
            // to shutdown.
            services.AddSingleton<ILicenseValidationService, LicenseValidationService>();

            // The SessionLockService must be a singleton. It manages a single, application-wide timer
            // for detecting user inactivity. Multiple instances would lead to incorrect and conflicting
            // lock-out behavior.
            services.AddSingleton<ISessionLockService, SessionLockService>();
            
            return services;
        }
    }
}