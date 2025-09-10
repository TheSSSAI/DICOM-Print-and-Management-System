using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a print job submitted to the system's processing queue.
/// </summary>
public sealed class PrintJob : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// The ID of the user who submitted the print job.
    /// </summary>
    public Guid SubmittedByUserId { get; private set; }

    /// <summary>
    /// A JSON object containing all details for the print job, 
    /// including layout, image references, and settings.
    /// </summary>
    public string JobPayload { get; private set; } = string.Empty;

    /// <summary>
    /// The current status of the print job (e.g., Queued, Processing, Completed, Failed).
    /// </summary>
    public string Status { get; private set; } = "Queued";

    /// <summary>
    /// The name of the target printer for the job.
    /// </summary>
    public string PrinterName { get; private set; } = string.Empty;

    /// <summary>
    /// A descriptive reason if the job failed.
    /// </summary>
    public string? FailureReason { get; private set; }

    /// <summary>
    /// The priority of the job in the queue, allowing administrators to re-order.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// A unique identifier to trace the print operation across client, queue, and service.
    /// </summary>
    public Guid? CorrelationId { get; private set; }

    /// <summary>
    /// The timestamp when the job was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; private set; }

    /// <summary>
    /// The timestamp when the job was last processed or completed.
    /// </summary>
    public DateTime? ProcessedAt { get; private set; }

    /// <summary>
    /// Navigation property to the user who submitted the job.
    /// </summary>
    public User? SubmittedByUser { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private PrintJob() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Creates a new instance of the <see cref="PrintJob"/> entity.
    /// </summary>
    /// <param name="id">The unique identifier for the print job.</param>
    /// <param name="submittedByUserId">The ID of the user submitting the job.</param>
    /// <param name="jobPayload">The serialized payload of the job.</param>
    /// <param name="printerName">The name of the target printer.</param>
    /// <param name="priority">The priority of the job.</param>
    /// <param name="correlationId">The correlation ID for tracing.</param>
    /// <returns>A new <see cref="PrintJob"/> instance.</returns>
    public static PrintJob Create(Guid id, Guid submittedByUserId, string jobPayload, string printerName, int priority, Guid? correlationId)
    {
        Guard.Against.Default(id, nameof(id));
        Guard.Against.Default(submittedByUserId, nameof(submittedByUserId));
        Guard.Against.NullOrWhiteSpace(jobPayload, nameof(jobPayload));
        Guard.Against.NullOrWhiteSpace(printerName, nameof(printerName));

        return new PrintJob
        {
            Id = id,
            SubmittedByUserId = submittedByUserId,
            JobPayload = jobPayload,
            PrinterName = printerName,
            Priority = priority,
            CorrelationId = correlationId,
            SubmittedAt = DateTime.UtcNow,
            Status = "Queued"
        };
    }

    /// <summary>
    /// Updates the status of the print job to 'Processing'.
    /// </summary>
    public void SetAsProcessing()
    {
        Status = "Processing";
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the status of the print job to 'Completed'.
    /// </summary>
    public void SetAsCompleted()
    {
        Status = "Completed";
        ProcessedAt = DateTime.UtcNow;
        FailureReason = null;
    }

    /// <summary>
    /// Updates the status of the print job to 'Failed' with a reason.
    /// </summary>
    /// <param name="reason">The reason for the failure.</param>
    public void SetAsFailed(string reason)
    {
        Guard.Against.NullOrWhiteSpace(reason, nameof(reason));
        Status = "Failed";
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the priority of the print job.
    /// </summary>
    /// <param name="newPriority">The new priority level.</param>
    public void UpdatePriority(int newPriority)
    {
        Priority = newPriority;
    }
}