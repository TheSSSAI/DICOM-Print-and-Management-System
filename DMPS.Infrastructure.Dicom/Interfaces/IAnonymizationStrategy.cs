using DMPS.Infrastructure.Dicom.Anonymization;
using FellowOakDicom.Dataset;

namespace DMPS.Infrastructure.Dicom.Interfaces;

/// <summary>
/// Defines the contract for a specific DICOM anonymization strategy.
/// This allows for different sets of anonymization rules to be applied.
/// </summary>
public interface IAnonymizationStrategy
{
    /// <summary>
    /// Gets the anonymization profile that this strategy implements.
    /// </summary>
    AnonymizationProfile Profile { get; }

    /// <summary>
    /// Applies the anonymization rules of this strategy to the provided DICOM dataset.
    /// The modification is performed in-place.
    /// </summary>
    /// <param name="dataset">The DICOM dataset to anonymize.</param>
    void Anonymize(DicomDataset dataset);
}