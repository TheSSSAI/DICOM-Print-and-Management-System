using DMPS.Data.Access.Abstractions;
using DMPS.Shared.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DMPS.Service.Worker.Workers
{
    /// <summary>
    /// A scheduled background worker that enforces data retention policies and performs data integrity checks.
    /// Runs on a configurable schedule (e.g., daily) to maintain the data store.
    /// </summary>
    public sealed class DataRetentionWorker : BackgroundService
    {
        private readonly ILogger<DataRetentionWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly DataRetentionSettings _settings;
        private readonly PeriodicTimer _timer;

        public DataRetentionWorker(
            ILogger<DataRetentionWorker> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<DataRetentionSettings> settings)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(scopeFactory);
            ArgumentNullException.ThrowIfNull(settings);

            _logger = logger;
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
            
            // For a production system, the schedule would be more sophisticated, possibly using a Cron expression parser.
            // For this implementation, a simple 24-hour timer suffices.
            _timer = new PeriodicTimer(TimeSpan.FromHours(24)); 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Retention Worker is starting. Will run every 24 hours.");

            try
            {
                // Wait for the first tick. This prevents it from running immediately on startup.
                await _timer.WaitForNextTickAsync(stoppingToken);

                while (await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    _logger.LogInformation("Data Retention Worker is starting its scheduled run.");
                    await ProcessDataRetentionAndIntegrityAsync();
                    _logger.LogInformation("Data Retention Worker has completed its scheduled run.");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Data Retention Worker is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A fatal error occurred in the Data Retention Worker, which is now stopping.");
            }
            finally
            {
                _timer.Dispose();
                _logger.LogInformation("Data Retention Worker has shut down.");
            }
        }
        
        private async Task ProcessDataRetentionAndIntegrityAsync()
        {
            // Each run uses its own DI scope to ensure DbContext and other scoped services are disposed of correctly.
            // This is a critical pattern for long-running singleton services.
            using var scope = _scopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                var studyRepository = serviceProvider.GetRequiredService<IStudyRepository>();
                var auditLogRepository = serviceProvider.GetRequiredService<IAuditLogRepository>();

                // --- Part 1: Data Retention ---
                await EnforceDataRetentionPolicyAsync(studyRepository, auditLogRepository);

                // --- Part 2: Data Integrity Check ---
                await PerformDataIntegrityCheckAsync(studyRepository);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during the scheduled data retention and integrity check.");
            }
        }

        private async Task EnforceDataRetentionPolicyAsync(IStudyRepository studyRepository, IAuditLogRepository auditLogRepository)
        {
            _logger.LogInformation("Starting data retention policy enforcement. Deleting studies older than {Days} days.", _settings.RetentionPeriodDays);

            var cutoffDate = DateTime.UtcNow.AddDays(-_settings.RetentionPeriodDays);
            
            var deletedStudies = await studyRepository.PurgeStudiesOlderThanAsync(cutoffDate);

            if (deletedStudies.Any())
            {
                _logger.LogInformation("Successfully purged {Count} studies older than {CutoffDate}.", deletedStudies.Count, cutoffDate.Date);
                var auditEvent = new
                {
                    EventType = "SystemDataPurge",
                    Details = $"Purged {deletedStudies.Count} studies older than the {_settings.RetentionPeriodDays}-day retention period.",
                    PurgedStudyInstanceUIDs = deletedStudies
                };
                
                // User ID is null for system-initiated events.
                await auditLogRepository.LogSystemEventAsync("SystemDataPurge", "Study", auditEvent);
            }
            else
            {
                _logger.LogInformation("No studies found that meet the criteria for data retention purge.");
            }
        }

        private async Task PerformDataIntegrityCheckAsync(IStudyRepository studyRepository)
        {
            _logger.LogInformation("Starting data integrity check.");
            
            // This is a placeholder for a more comprehensive check. A real implementation might:
            // 1. Get a batch of active study records from the DB.
            // 2. For each record, check if the corresponding DICOM file(s) exist on the file system.
            // 3. Log any discrepancies (DB record exists but file is missing, or vice-versa).
            var discrepancies = await studyRepository.VerifyDataIntegrityAsync();

            if (discrepancies > 0)
            {
                _logger.LogWarning("Data integrity check found {Count} discrepancies. See detailed logs for more information.", discrepancies);
                // An alert should be triggered here for an administrator to investigate.
            }
            else
            {
                _logger.LogInformation("Data integrity check completed successfully with no discrepancies found.");
            }
        }
    }
}