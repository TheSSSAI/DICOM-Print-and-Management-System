using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a single DICOM image instance within a series.
/// This entity stores metadata about the image and the path to its physical file.
/// </summary>
public sealed class Image : Entity<Guid>
{
    /// <summary>
    /// Foreign key referencing the Series this image belongs to.
    /// </summary>
    public Guid SeriesId { get; private set; }

    /// <summary>
    /// DICOM SOP Instance UID (0008,0018). Uniquely identifies this image instance worldwide.
    /// </summary>
    public string SopInstanceUid { get; private set; }

    /// <summary>
    /// DICOM Instance Number (0020,0013). The sequential number of this image within the series.
    /// </summary>
    public int? InstanceNumber { get; private set; }

    /// <summary>
    /// The full physical or network path to the DICOM file on the storage system.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// The size of the DICOM file in bytes.
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// A soft delete flag used by data retention policies. If true, the image is marked for deletion.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Timestamp of when the image record was created in the system.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private Image() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Image"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the image.</param>
    /// <param name="seriesId">The ID of the series this image belongs to.</param>
    /// <param name="sopInstanceUid">The SOP Instance UID of the image.</param>
    /// <param name="instanceNumber">The instance number within the series.</param>
    /// <param name="filePath">The path to the DICOM file.</param>
    /// <param name="fileSize">The size of the file in bytes.</param>
    public Image(Guid id, Guid seriesId, string sopInstanceUid, int? instanceNumber, string filePath, long fileSize) : base(id)
    {
        Guard.Against.Default(seriesId, nameof(seriesId));
        Guard.Against.NullOrWhiteSpace(sopInstanceUid, nameof(sopInstanceUid));
        Guard.Against.NullOrWhiteSpace(filePath, nameof(filePath));
        Guard.Against.NegativeOrZero(fileSize, nameof(fileSize));

        SeriesId = seriesId;
        SopInstanceUid = sopInstanceUid;
        InstanceNumber = instanceNumber;
        FilePath = filePath;
        FileSize = fileSize;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the image as deleted as part of a data retention policy.
    /// </summary>
    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}