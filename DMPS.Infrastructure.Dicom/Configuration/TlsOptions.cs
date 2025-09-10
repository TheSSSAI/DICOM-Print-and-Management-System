namespace DMPS.Infrastructure.Dicom.Configuration;

/// <summary>
/// Represents configuration options for Transport Layer Security (TLS) used in DICOM communications.
/// </summary>
public class TlsOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "DicomTls";

    /// <summary>
    /// Gets or sets a value indicating whether TLS should be enabled for the communication.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the path to the certificate file (e.g., .pfx).
    /// Required if TLS is enabled for a server (SCP). Can be optional for clients if server certificate validation is disabled.
    /// </summary>
    public string? CertificatePath { get; set; }

    /// <summary>
    /// Gets or sets the password for the certificate file.
    /// </summary>
    public string? CertificatePassword { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore server certificate validation errors.
    /// WARNING: Setting this to true is insecure and should only be used in trusted, controlled environments.
    /// </summary>
    public bool IgnoreCertificateValidation { get; set; } = false;
}