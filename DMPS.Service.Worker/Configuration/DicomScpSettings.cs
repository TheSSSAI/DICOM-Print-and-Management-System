using System.ComponentModel.DataAnnotations;

namespace DMPS.Service.Worker.Configuration
{
    /// <summary>
    /// Provides strongly-typed configuration options for the DICOM C-STORE SCP (Service Class Provider) listener.
    /// This class is designed to be populated from the "DicomScp" section of the application's configuration.
    /// </summary>
    public sealed class DicomScpSettings
    {
        /// <summary>
        /// The configuration section name used in appsettings.json.
        /// </summary>
        public const string SectionName = "DicomScp";

        /// <summary>
        /// Gets the TCP port on which the DICOM SCP service will listen for incoming associations.
        /// This is a critical setting for network communication with imaging modalities.
        /// </summary>
        /// <remarks>
        /// The default value is 104, which is the standard, well-known port for DICOM services.
        /// The valid range is between 1 and 65535.
        /// </remarks>
        [Required]
        [Range(1, 65535, ErrorMessage = "The DICOM SCP port must be between 1 and 65535.")]
        public int Port { get; init; } = 104;

        /// <summary>
        /// Gets the Application Entity Title (AET) of this application's SCP service.
        /// This is a unique identifier used within the DICOM network to address this service.
        /// </summary>
        /// <remarks>
        /// The AET is case-sensitive and typically limited to 16 characters.
        /// </remarks>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The DICOM SCP AETitle cannot be null or empty.")]
        [StringLength(16, ErrorMessage = "The DICOM SCP AETitle cannot exceed 16 characters.")]
        [RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "AETitle contains invalid characters.")]
        public string AETitle { get; init; } = "DMPSSCP";
    }
}