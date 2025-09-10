using System.ComponentModel.DataAnnotations;

namespace DMPS.CrossCutting.Security.Configuration
{
    /// <summary>
    /// Provides strongly-typed configuration for the BCrypt password hashing service.
    /// This class is designed to be used with the .NET Options pattern.
    /// </summary>
    public sealed class BCryptSettings
    {
        /// <summary>
        /// The configuration section name in appsettings.json.
        /// </summary>
        public const string ConfigurationSectionName = "Security:BCrypt";

        /// <summary>
        /// Gets or sets the computational cost factor for the BCrypt algorithm.
        /// A higher value increases security but slows down the hashing process.
        /// The value must be between 4 and 31, inclusive.
        /// </summary>
        /// <value>The BCrypt work factor. Defaults to 12.</value>
        [Range(4, 31, ErrorMessage = "WorkFactor must be between 4 and 31.")]
        public int WorkFactor { get; set; } = 12;
    }
}