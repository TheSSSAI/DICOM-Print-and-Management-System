using DMPS.CrossCutting.Logging.Context;
using Serilog.Core;
using Serilog.Events;

namespace DMPS.CrossCutting.Logging.Enrichers;

/// <summary>
/// Enriches log events with a CorrelationId property from the CorrelationContext.
/// This is essential for tracing a single operation or request across multiple
/// log entries, services, and asynchronous boundaries, fulfilling REQ-1-090.
/// </summary>
public sealed class CorrelationIdEnricher : ILogEventEnricher
{
    /// <summary>
    /// The property name to add to the log event.
    /// </summary>
    private const string CorrelationIdPropertyName = "CorrelationId";

    /// <summary>
    /// Enrich the log event with a CorrelationId property.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(propertyFactory);

        try
        {
            var correlationId = CorrelationContext.GetCorrelationId();

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                // If no correlation ID is set in the context, do not add the property.
                return;
            }

            // Create the property and add it to the log event.
            // Using AddPropertyIfAbsent is a safe way to ensure we don't overwrite
            // a CorrelationId that might have been added by another mechanism.
            var correlationIdProperty = propertyFactory.CreateProperty(CorrelationIdPropertyName, correlationId);
            logEvent.AddPropertyIfAbsent(correlationIdProperty);
        }
        catch (Exception)
        {
            // An enricher must never throw an exception, as it can crash the
            // entire logging pipeline and potentially the application.
            // This catch block is a safeguard against any unforeseen issues within
            // the enrichment logic. We consciously swallow the exception to prioritize
            // application stability over perfect log enrichment.
        }
    }
}