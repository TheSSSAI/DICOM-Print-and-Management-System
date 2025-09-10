// DMPS.Shared.Core/Application/Contracts/DTOs/GeneratePdfCommand.cs
namespace DMPS.Shared.Core.Application.Contracts.DTOs
{
    /// <summary>
    /// A Data Transfer Object representing a command to generate a PDF file from a print layout.
    /// This command is published by the client application to a message queue for asynchronous handling
    /// by a background worker service.
    /// </summary>
    /// <param name="SubmittedByUserId">The unique identifier of the user who initiated the export.</param>
    /// <param name="LayoutPayload">A serialized JSON string containing all details for the layout, image references, and settings.</param>
    /// <param name="OutputFilePath">The full file path where the generated PDF should be saved.</param>
    /// <param name="CorrelationId">A unique identifier to trace this operation across different services and logs.</param>
    public sealed record GeneratePdfCommand(
        Guid SubmittedByUserId,
        string LayoutPayload,
        string OutputFilePath,
        Guid CorrelationId);
}