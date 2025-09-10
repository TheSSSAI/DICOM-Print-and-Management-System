using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a series of DICOM images within a study.
/// This entity is part of the Study aggregate.
/// </summary>
public sealed class Series : Entity<Guid>, IAggregateRoot
{
    private readonly List<Image> _images = [];
    private readonly List<PresentationState> _presentationStates = [];

    /// <summary>
    /// Foreign key referencing the study to which this series belongs.
    /// </summary>
    public Guid StudyId { get; private set; }

    /// <summary>
    /// The unique identifier for the series (DICOM Tag 0020,000E).
    /// </summary>
    public string SeriesInstanceUid { get; private set; } = string.Empty;

    /// <summary>
    /// The modality of the equipment that produced the series (e.g., CT, MR) (DICOM Tag 0008,0060).
    /// </summary>
    public string? Modality { get; private set; }

    /// <summary>
    /// A number that identifies this series within a study (DICOM Tag 0020,0011).
    /// </summary>
    public int? SeriesNumber { get; private set; }

    /// <summary>
    /// A description of the series (DICOM Tag 0008,103E).
    /// </summary>
    public string? SeriesDescription { get; private set; }

    /// <summary>
    /// The body part examined in this series (DICOM Tag 0018,0015).
    /// </summary>
    public string? BodyPartExamined { get; private set; }

    /// <summary>
    /// The timestamp when this series record was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the parent study.
    /// </summary>
    public Study? Study { get; private set; }

    /// <summary>
    /// A collection of images belonging to this series.
    /// </summary>
    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();

    /// <summary>
    /// A collection of presentation states associated with this series.
    /// </summary>
    public IReadOnlyCollection<PresentationState> PresentationStates => _presentationStates.AsReadOnly();

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private Series() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Creates a new instance of the <see cref="Series"/> entity.
    /// </summary>
    /// <param name="id">The unique identifier for the series entity.</param>
    /// <param name="studyId">The identifier of the parent study.</param>
    /// <param name="seriesInstanceUid">The DICOM Series Instance UID.</param>
    /// <param name="modality">The DICOM modality.</param>
    /// <param name="seriesNumber">The DICOM series number.</param>
    /// <param name="seriesDescription">The DICOM series description.</param>
    /// <param name="bodyPartExamined">The DICOM body part examined.</param>
    /// <returns>A new <see cref="Series"/> instance.</returns>
    public static Series Create(Guid id, Guid studyId, string seriesInstanceUid, string? modality, int? seriesNumber, string? seriesDescription, string? bodyPartExamined)
    {
        Guard.Against.Default(id, nameof(id));
        Guard.Against.Default(studyId, nameof(studyId));
        Guard.Against.NullOrWhiteSpace(seriesInstanceUid, nameof(seriesInstanceUid));

        return new Series
        {
            Id = id,
            StudyId = studyId,
            SeriesInstanceUid = seriesInstanceUid,
            Modality = modality,
            SeriesNumber = seriesNumber,
            SeriesDescription = seriesDescription,
            BodyPartExamined = bodyPartExamined,
            CreatedAt = DateTime.UtcNow
        };
    }
}