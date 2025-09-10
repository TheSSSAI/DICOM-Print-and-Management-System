namespace DMPS.Infrastructure.Dicom.Configuration;

/// <summary>
/// Represents configuration options for the DICOM C-STORE SCP service.
/// </summary>
public class DicomScpOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "DicomScp";

    /// <summary>
    /// Gets or sets the TCP port on which the SCP service will listen for incoming associations.
    /// </summary>
    public int Port { get; set; } = 11112;

    /// <summary>
    /// Gets or sets the Application Entity Title (AET) of the local SCP service.
    /// </summary>
    public string Aet { get; set; } = "DMPSSCP";
    
    /// <summary>
    /// Gets or sets the TLS options for the SCP service.
    /// </summary>
    public TlsOptions Tls { get; set; } = new();
}