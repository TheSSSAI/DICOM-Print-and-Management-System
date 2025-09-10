using FellowOakDicom;
using DMPS.Shared.Core.Models;

namespace DMPS.Infrastructure.Dicom.Interfaces;

/// <summary>
/// Defines the contract for a DICOM Service Class User (SCU).
/// This service provides functionalities for communicating with external PACS servers,
/// including verification (C-ECHO), querying (C-FIND), and retrieving (C-MOVE) studies.
/// </summary>
public interface IDicomScuService
{
    /// <summary>
    /// Verifies the connection to a remote PACS by sending a DICOM C-ECHO request.
    /// </summary>
    /// <param name="config">The configuration of the remote PACS server.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the connection is successful; otherwise, false.</returns>
    Task<bool> VerifyPacsConnectionAsync(PacsConfiguration config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries a remote PACS for studies matching the specified criteria using a DICOM C-FIND request.
    /// </summary>
    /// <param name="config">The configuration of the remote PACS server.</param>
    /// <param name="criteria">The criteria to use for the query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An asynchronous stream of DicomDataset objects, each representing a matching study found on the PACS.</returns>
    /// <exception cref="PacsConnectionException">Thrown if the connection to the PACS fails or is rejected.</exception>
    /// <exception cref="DicomIntegrationException">Thrown for other DICOM protocol or configuration related errors.</exception>
    IAsyncEnumerable<DicomDataset> QueryStudiesAsync(PacsConfiguration config, DicomQueryCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a retrieval of a specific study from a remote PACS using a DICOM C-MOVE request.
    /// This commands the remote PACS to send the study to a specified destination Application Entity Title (AET).
    /// </summary>
    /// <param name="config">The configuration of the remote PACS server.</param>
    /// <param name="studyInstanceUid">The Study Instance UID of the study to retrieve.</param>
    /// <param name="destinationAet">The Application Entity Title of the local C-STORE SCP where the study should be sent.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the C-MOVE request has been successfully acknowledged by the PACS.</returns>
    /// <exception cref="PacsConnectionException">Thrown if the connection to the PACS fails or is rejected.</exception>
    /// <exception cref="DicomIntegrationException">Thrown if the PACS returns a failure status, such as 'Move Destination Unknown'.</exception>
    Task MoveStudyAsync(PacsConfiguration config, string studyInstanceUid, string destinationAet, CancellationToken cancellationToken = default);
}