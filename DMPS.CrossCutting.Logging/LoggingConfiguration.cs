using DMPS.CrossCutting.Logging.Extensions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;

namespace DMPS.CrossCutting.Logging
{
    /// <summary>
    /// Provides the central, static method for configuring the Serilog logger for the entire application.
    /// This is the main entry point for the logging repository.
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Configures and creates the application's primary Serilog logger instance.
        /// This method sets up the logger by reading from the provided IConfiguration,
        /// configures sinks for local file and Windows Event Log, and adds custom enrichers
        /// for PHI masking and correlation ID tracing.
        /// </summary>
        /// <param name="configuration">The application's configuration provider, used to read logging settings from appsettings.json.</param>
        /// <returns>A configured Serilog ILogger instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the configuration object is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if logger configuration fails, to prevent the application from starting in an un-loggable state.</exception>
        public static Logger ConfigureLogger(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            try
            {
                var logger = new LoggerConfiguration()
                    // Read base configuration from appsettings.json (sinks, minimum levels, enrichers etc.)
                    // This allows for environment-specific overrides without code changes.
                    // IMPORTANT: For performance, sinks like File should be configured with Serilog.Sinks.Async
                    // wrapper directly in the appsettings.json file.
                    // Example appsettings.json configuration for an async file sink:
                    // "Serilog": {
                    //   "WriteTo": [
                    //     {
                    //       "Name": "Async",
                    //       "Args": {
                    //         "configure": [
                    //           {
                    //             "Name": "File",
                    //             "Args": {
                    //               "path": "logs/dmps-log-.txt",
                    //               "rollingInterval": "Day",
                    //               "retainedFileCountLimit": 30,
                    //               "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"
                    //             }
                    //           }
                    //         ]
                    //       }
                    //     }
                    //   ]
                    // }
                    .ReadFrom.Configuration(configuration)

                    // Apply custom application-specific enrichers programmatically.
                    // This ensures these critical enrichers are always present regardless of configuration.
                    .Enrich.WithPhiMasking()
                    .Enrich.WithCorrelationId()
                    .CreateLogger();
                
                // Set the static logger instance for early-stage logging (before DI is available)
                Log.Logger = logger;

                Log.Information("Serilog logger configured successfully. Sinks and enrichers initialized.");

                return logger;
            }
            catch (Exception ex)
            {
                // If logger configuration fails, it's a critical application startup failure.
                // Log to the console as a last resort and throw an exception to halt startup.
                var errorMessage = "FATAL: Serilog logger configuration failed. Application cannot start.";
                Console.Error.WriteLine($"{errorMessage}\n{ex}");

                // Re-assign a failsafe logger to the static instance
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
                
                Log.Fatal(ex, errorMessage);

                throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }
}