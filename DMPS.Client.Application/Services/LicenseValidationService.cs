using DMPS.Client.Application.Interfaces;
using DMPS.Infrastructure.IO.DataAccess;
using DMPS.Shared.Core.Enums;
using DMPS.Shared.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DMPS.Client.Application.Services;

/// <summary>
/// Orchestrates the application license validation workflow on startup, including managing the grace period for network failures.
/// </summary>
public sealed class LicenseValidationService : ILicenseValidationService
{
    private const string GracePeriodStartedAtSettingKey = "LicenseGracePeriodStartedAt";
    private static readonly TimeSpan GracePeriodDuration = TimeSpan.FromHours(72);

    private readonly ILicenseApiClient _licenseApiClient;
    private readonly IApplicationStateService _applicationStateService;
    private readonly ILocalSettingsRepository _localSettingsRepository;
    private readonly ILogger<LicenseValidationService> _logger;

    public LicenseValidationService(
        ILicenseApiClient licenseApiClient,
        IApplicationStateService applicationStateService,
        ILocalSettingsRepository localSettingsRepository, // Assumed dependency from an Infrastructure project for persistence
        ILogger<LicenseValidationService> logger)
    {
        _licenseApiClient = licenseApiClient ?? throw new ArgumentNullException(nameof(licenseApiClient));
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService));
        _localSettingsRepository = localSettingsRepository ?? throw new ArgumentNullException(nameof(localSettingsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task ValidateLicenseOnStartupAsync()
    {
        _logger.LogInformation("Starting license validation...");

        string? licenseKey = await _localSettingsRepository.GetSettingAsync("LicenseKey");
        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            _logger.LogError("License key is not configured. Application will run in read-only mode.");
            _applicationStateService.SetLicenseStatus(LicenseStatus.ReadOnly);
            return;
        }

        try
        {
            var validationResult = await _licenseApiClient.ValidateLicenseAsync(licenseKey);

            switch (validationResult)
            {
                case LicenseValidationResult.Valid:
                    _logger.LogInformation("License is valid. Application is fully functional.");
                    await ClearGracePeriodAsync();
                    _applicationStateService.SetLicenseStatus(LicenseStatus.Licensed);
                    break;

                case LicenseValidationResult.Invalid:
                    _logger.LogWarning("License is invalid. Application will run in read-only mode.");
                    await ClearGracePeriodAsync();
                    _applicationStateService.SetLicenseStatus(LicenseStatus.ReadOnly);
                    break;
                
                // This case handles network failures, timeouts, etc.
                case LicenseValidationResult.ApiUnreachable:
                    _logger.LogWarning("License validation API is unreachable. Checking grace period status.");
                    await HandleGracePeriodAsync();
                    break;
                
                default:
                    _logger.LogError("Unknown license validation result: {Result}. Defaulting to read-only mode.", validationResult);
                    _applicationStateService.SetLicenseStatus(LicenseStatus.ReadOnly);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during license validation. Checking grace period status.");
            await HandleGracePeriodAsync();
        }
    }

    private async Task HandleGracePeriodAsync()
    {
        string? gracePeriodStartString = await _localSettingsRepository.GetSettingAsync(GracePeriodStartedAtSettingKey);

        if (string.IsNullOrEmpty(gracePeriodStartString) || !DateTime.TryParse(gracePeriodStartString, out var gracePeriodStartedAt))
        {
            // Grace period has not started yet. Start it now.
            var now = DateTime.UtcNow;
            await _localSettingsRepository.SetSettingAsync(GracePeriodStartedAtSettingKey, now.ToString("o"));
            _logger.LogInformation("Starting 72-hour license grace period, ending at {GracePeriodEnd}.", now.Add(GracePeriodDuration));
            _applicationStateService.SetLicenseStatus(LicenseStatus.GracePeriod);
        }
        else
        {
            var gracePeriodEndsAt = gracePeriodStartedAt.Add(GracePeriodDuration);
            if (DateTime.UtcNow > gracePeriodEndsAt)
            {
                // Grace period has expired.
                _logger.LogError("License grace period started at {GracePeriodStart} has expired. Application will run in read-only mode.", gracePeriodStartedAt);
                _applicationStateService.SetLicenseStatus(LicenseStatus.ReadOnly);
            }
            else
            {
                // Still within the grace period.
                _logger.LogWarning("Application is running in a grace period. Full functionality is available until {GracePeriodEnd}.", gracePeriodEndsAt);
                _applicationStateService.SetLicenseStatus(LicenseStatus.GracePeriod);
            }
        }
    }
    
    private async Task ClearGracePeriodAsync()
    {
        await _localSettingsRepository.RemoveSettingAsync(GracePeriodStartedAtSettingKey);
        _logger.LogDebug("Cleared any existing license grace period data.");
    }
}