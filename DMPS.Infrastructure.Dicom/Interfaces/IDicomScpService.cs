using FellowOakDicom;

namespace DMPS.Infrastructure.Dicom.Interfaces;

/// <summary>
/// Defines the contract for a DICOM C-STORE Service Class Provider (SCP).
/// This service is responsible for listening for incoming DICOM associations and processing C-STORE requests.
/// It acts as the primary data ingestion point from external modalities.
/// </summary>
public interface IDicomScpService : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the SCP listener is currently running.
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    /// Starts the DICOM C-STORE SCP listener on the configured port.
    /// </summary>
    /// <param name="onFileReceived">
    /// A callback action that is invoked when a DICOM file is successfully received.
    /// The action receives the DicomFile object and the calling Application Entity Title (AET) of the source.
    /// The consumer of this callback is responsible for any further processing, such as storage and database insertion.
    /// </param>
    /// <exception cref="InvalidOperationException">Thrown if the listener is already running.</exception>
    /// <exception cref="DicomIntegrationException">Thrown for configuration or network-related errors during startup.</exception>
    void StartListening(Action<DicomFile, string> onFileReceived);

    /// <summary>
    /// Stops the DICOM C-STORE SCP listener.
    /// </summary>
    void StopListening();
}