using Serilog.Core;
using Serilog.Events;
using System.Text.RegularExpressions;

namespace DMPS.CrossCutting.Logging.Enrichers
{
    /// <summary>
    /// A custom Serilog enricher that redacts Protected Health Information (PHI) from log event properties.
    /// This is a critical component for ensuring HIPAA compliance as per requirement REQ-1-039.
    /// </summary>
    /// <remarks>
    /// This enricher uses a pre-compiled regular expression to identify and mask known PHI patterns
    /// within string values of log event properties. It is designed to be highly performant and robust,
    /// preventing exceptions from crashing the application's logging pipeline.
    /// </remarks>
    public sealed class PhiMaskingEnricher : ILogEventEnricher
    {
        // A pre-compiled, static, case-insensitive Regex for performance and thread-safety.
        // This pattern looks for common PHI keywords followed by a colon, optional whitespace, and then captures the value to be redacted.
        // The value is assumed to be a single "word" (no spaces) or an alphanumeric sequence.
        // This pattern is an example and should be expanded based on specific PHI data formats logged by the application.
        private static readonly Regex PhiRedactionRegex = new(
            @"(?<keyword>PatientName|PatientID|MRN|AccessionNumber|Patient Name|Patient ID)\s*:\s*(?<value>[a-zA-Z0-9\^\-]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private const string RedactedPlaceholder = "[REDACTED]";

        /// <summary>
        /// Enriches the provided log event by masking any detected PHI in its properties.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">A factory for creating new or modified log event properties.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent is null) return;

            try
            {
                // We create a list of properties to update to avoid modifying the collection while iterating.
                List<KeyValuePair<string, LogEventProperty>> propertiesToUpdate = new();

                foreach (var property in logEvent.Properties)
                {
                    if (property.Value is ScalarValue scalar && scalar.Value is string stringValue)
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            continue;
                        }

                        // Use a MatchEvaluator for a clean replacement logic
                        string maskedValue = PhiRedactionRegex.Replace(stringValue, match =>
                        {
                            // Reconstruct the string with the keyword but with a redacted value.
                            // e.g., "PatientID:12345" becomes "PatientID: [REDACTED]"
                            return $"{match.Groups["keyword"].Value}: {RedactedPlaceholder}";
                        });

                        // If the value was changed, we need to update the property.
                        if (maskedValue != stringValue)
                        {
                            var newProperty = propertyFactory.CreateProperty(property.Key, maskedValue);
                            propertiesToUpdate.Add(new KeyValuePair<string, LogEventProperty>(property.Key, newProperty));
                        }
                    }
                }

                // Apply the updates to the log event.
                foreach (var kvp in propertiesToUpdate)
                {
                    logEvent.AddOrUpdateProperty(kvp.Value);
                }
            }
            catch (Exception)
            {
                // CRITICAL: An enricher must NOT throw an exception, as it can crash the entire logging pipeline
                // and potentially the application itself. If any error occurs during redaction, we silently
                // ignore it and log the original, unredacted message. This prioritizes application stability.
                // For debugging, Serilog's SelfLog can be used, but in production, it's safer to just swallow.
                // Serilog.Debugging.SelfLog.WriteLine("Error in PhiMaskingEnricher: {0}", ex);
            }
        }
    }
}