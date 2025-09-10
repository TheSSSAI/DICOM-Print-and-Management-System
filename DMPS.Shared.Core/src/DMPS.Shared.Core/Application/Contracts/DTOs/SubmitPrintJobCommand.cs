// DMPS.Shared.Core/Application/Contracts/DTOs/SubmitPrintJobCommand.cs
namespace DMPS.Shared.Core.Application.Contracts.DTOs
{
    /// <summary>
    /// A Data Transfer Object representing a command to process and spool a new print job.
    /// This command is published by the client application to a message queue for asynchronous handling
    /// by the background printing service.
    /// </summary>
    /// <param name="PrintJobId">The unique identifier for the print job entity in the database.</param>
    /// <param name="SubmittedByUserId">The unique identifier of the user who submitted the job.</param>
    /// <param name="JobPayload">A serialized JSON string containing all details for the print job, including layout, image references, and settings.</param>
    /// <param name="PrinterName">The name of the target Windows printer.</param>
    /// <param name="Priority">The priority of the print job.</param>
    /// <param name="CorrelationId">A unique identifier to trace this operation across different services and logs.</param>
    public sealed record SubmitPrintJobCommand(
        Guid PrintJobId,
        Guid SubmittedByUserId,
        string JobPayload,
        string PrinterName,
        int Priority,
        Guid CorrelationId);
}