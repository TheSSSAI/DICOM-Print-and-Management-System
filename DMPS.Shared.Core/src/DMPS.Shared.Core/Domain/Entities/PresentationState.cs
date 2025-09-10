using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a DICOM Grayscale Softcopy Presentation State (GSPS) object.
/// This entity stores non-destructive annotations, measurements, and display settings for a series.
/// </summary>
public sealed class PresentationState : Entity<Guid>
{
    /// <summary>
    /// Foreign key referencing the Series to which this presentation state applies.
    /// </summary>
    public Guid SeriesId { get; private set; }

    /// <summary>
    /// DICOM SOP Instance UID of the GSPS object itself. Uniquely identifies this presentation state.
    /// </summary>
    public string SopInstanceUid { get; private set; }

    /// <summary>
    /// The full physical or network path to the GSPS DICOM file on the storage system.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// Foreign key referencing the User who created this presentation state.
    /// </summary>
    public Guid CreatedByUserId { get; private set; }

    /// <summary>
    /// Timestamp of when the presentation state record was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Timestamp of the last update to the presentation state record.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private PresentationState() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PresentationState"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the presentation state.</param>
    /// <param name="seriesId">The ID of the series this presentation state applies to.</param>
    /// <param name="sopInstanceUid">The SOP Instance UID of the GSPS object.</param>
    /// <param name="filePath">The path to the GSPS DICOM file.</param>
    /// <param name="createdByUserId">The ID of the user who created the presentation state.</param>
    public PresentationState(Guid id, Guid seriesId, string sopInstanceUid, string filePath, Guid createdByUserId) : base(id)
    {
        Guard.Against.Default(seriesId, nameof(seriesId));
        Guard.Against.NullOrWhiteSpace(sopInstanceUid, nameof(sopInstanceUid));
        Guard.Against.NullOrWhiteSpace(filePath, nameof(filePath));
        Guard.Against.Default(createdByUserId, nameof(createdByUserId));

        SeriesId = seriesId;
        SopInstanceUid = sopInstanceUid;
        FilePath = filePath;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    /// <summary>
    /// Updates the timestamp to reflect a modification.
    /// </summary>
    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}