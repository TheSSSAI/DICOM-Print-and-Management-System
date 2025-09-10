// DMPS.Shared.Core/Application/Contracts/DTOs/ProcessDicomStoreCommand.cs
namespace DMPS.Shared.Core.Application.Contracts.DTOs
{
    /// <summary>
    /// A Data Transfer Object representing a command to process and persist a received DICOM study.
    /// This command is typically published to a message queue by a C-STORE SCP listener or a file import service
    /// for asynchronous processing by a background worker.
    /// </summary>
    /// <param name="StagedFilePaths">The list of file paths where the received DICOM instances are temporarily stored.</param>
    /// <param name="SourceAETitle">The Application Entity Title of the DICOM source, if available.</param>
    /// <param name="CorrelationId">A unique identifier to trace this operation across different services and logs.</param>
    public sealed record ProcessDicomStoreCommand(
        List<string> StagedFilePaths,
        string? SourceAETitle,
        Guid CorrelationId);
}