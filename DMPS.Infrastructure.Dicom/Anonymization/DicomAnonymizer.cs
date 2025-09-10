using DMPS.Infrastructure.Dicom.Anonymization.Interfaces;
using FellowOakDicom;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace DMPS.Infrastructure.Dicom.Anonymization;

/// <summary>
/// Provides DICOM metadata anonymization services using a strategy pattern.
/// This service orchestrates the anonymization process by selecting and applying the appropriate
/// anonymization strategy based on the specified profile.
/// </summary>
public sealed class DicomAnonymizer : IDicomAnonymizer
{
    private readonly IReadOnlyDictionary<AnonymizationProfile, IAnonymizationStrategy> _strategies;
    private readonly ILogger<DicomAnonymizer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DicomAnonymizer"/> class.
    /// It discovers and maps all available anonymization strategies provided via dependency injection.
    /// </summary>
    /// <param name="strategies">An enumeration of all registered anonymization strategies.</param>
    /// <param name="logger">The logger for diagnostics and error reporting.</param>
    public DicomAnonymizer(IEnumerable<IAnonymizationStrategy> strategies, ILogger<DicomAnonymizer> logger)
    {
        _logger = logger;
        _strategies = BuildStrategyMap(strategies);
    }

    /// <inheritdoc />
    public DicomFile Anonymize(DicomFile original, AnonymizationProfile profile)
    {
        ArgumentNullException.ThrowIfNull(original);

        var studyInstanceUid = original.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Unknown");
        _logger.LogInformation(
            "Starting anonymization for StudyInstanceUID: {StudyInstanceUid} with profile: {Profile}",
            studyInstanceUid,
            profile);

        try
        {
            if (!_strategies.TryGetValue(profile, out var strategy))
            {
                _logger.LogError("No anonymization strategy found for profile: {Profile}. This is a configuration error.", profile);
                throw new NotSupportedException($"Anonymization profile '{profile}' is not supported or no corresponding strategy has been registered.");
            }

            // Per REQ-1-065, we must create a new de-identified copy and leave the original unmodified.
            // fo-dicom's Clone() performs a deep copy of the dataset.
            var anonymizedFile = original.Clone();

            _logger.LogDebug("Applying strategy '{StrategyName}' for profile '{Profile}'", strategy.GetType().Name, profile);
            strategy.Anonymize(anonymizedFile.Dataset);
            
            anonymizedFile.Dataset.AddOrUpdate(DicomTag.PatientIdentityRemoved, "YES");
            anonymizedFile.Dataset.AddOrUpdate(DicomTag.DeidentificationMethod, $"DMPS Anonymizer - Profile: {profile}");

            _logger.LogInformation(
                "Successfully completed anonymization for StudyInstanceUID: {StudyInstanceUid} with profile: {Profile}",
                studyInstanceUid,
                profile);

            return anonymizedFile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during DICOM anonymization for StudyInstanceUID: {StudyInstanceUid}", studyInstanceUid);
            throw;
        }
    }

    private IReadOnlyDictionary<AnonymizationProfile, IAnonymizationStrategy> BuildStrategyMap(IEnumerable<IAnonymizationStrategy> strategies)
    {
        var strategyMap = new Dictionary<AnonymizationProfile, IAnonymizationStrategy>();

        foreach (var strategy in strategies)
        {
            if (strategyMap.ContainsKey(strategy.Profile))
            {
                _logger.LogWarning(
                    "Duplicate anonymization strategy registration detected for profile '{Profile}'. " +
                    "Existing: '{ExistingStrategy}', New: '{NewStrategy}'. The new strategy will be ignored.",
                    strategy.Profile,
                    strategyMap[strategy.Profile].GetType().Name,
                    strategy.GetType().Name);
            }
            else
            {
                _logger.LogDebug("Registering anonymization strategy '{StrategyName}' for profile '{Profile}'", 
                    strategy.GetType().Name, strategy.Profile);
                strategyMap.Add(strategy.Profile, strategy);
            }
        }
        
        if (strategyMap.Count == 0)
        {
            _logger.LogWarning("No anonymization strategies were found or registered. The DicomAnonymizer will not be functional.");
        }

        return new ReadOnlyDictionary<AnonymizationProfile, IAnonymizationStrategy>(strategyMap);
    }
}