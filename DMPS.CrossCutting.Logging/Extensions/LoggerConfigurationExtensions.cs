using DMPS.CrossCutting.Logging.Enrichers;
using Serilog;
using Serilog.Configuration;

namespace DMPS.CrossCutting.Logging.Extensions;

/// <summary>
/// Provides extension methods for <see cref="LoggerConfiguration"/> to simplify the addition of custom
/// application-specific enrichers.
/// </summary>
public static class LoggerConfigurationExtensions
{
    /// <summary>
    /// Enriches log events with a PHI (Protected Health Information) masking processor.
    /// This enricher scans log properties and redacts sensitive information to ensure compliance with HIPAA.
    /// </summary>
    /// <param name="loggerConfiguration">The Serilog logger configuration.</param>
    /// <returns>The modified logger configuration for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="loggerConfiguration"/> is null.</exception>
    public static LoggerConfiguration WithPhiMasking(this LoggerConfiguration loggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);

        return loggerConfiguration.Enrich.With<PhiMaskingEnricher>();
    }

    /// <summary>
    /// Enriches log events with a CorrelationId property.
    /// The Correlation ID is retrieved from the ambient <see cref="Context.CorrelationContext"/>,
    /// enabling end-to-end tracing of operations across asynchronous calls and service boundaries.
    /// </summary>
    /// <param name="loggerConfiguration">The Serilog logger configuration.</param>
    /// <returns>The modified logger configuration for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="loggerConfiguration"/> is null.</exception>
    public static LoggerConfiguration WithCorrelationId(this LoggerConfiguration loggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);

        return loggerConfiguration.Enrich.With<CorrelationIdEnricher>();
    }
}