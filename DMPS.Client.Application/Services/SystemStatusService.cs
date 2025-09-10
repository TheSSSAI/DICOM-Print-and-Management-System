using DMPS.Client.Application.Interfaces;
using DMPS.Infrastructure.Communication.Abstractions;
using Microsoft.Extensions.Logging;
using System.IO.Pipes;

namespace DMPS.Client.Application.Services
{
    /// <summary>
    /// Provides real-time status checks of the background DICOM Windows Service
    /// using a low-latency Inter-Process Communication (IPC) mechanism.
    /// </summary>
    public sealed class SystemStatusService : ISystemStatusService
    {
        private readonly INamedPipeClient _namedPipeClient;
        private readonly ILogger<SystemStatusService> _logger;

        private const string StatusRequestMessage = "PING";
        private const string ExpectedResponseMessage = "PONG";

        public SystemStatusService(INamedPipeClient namedPipeClient, ILogger<SystemStatusService> logger)
        {
            _namedPipeClient = namedPipeClient ?? throw new ArgumentNullException(nameof(namedPipeClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<bool> IsBackgroundServiceRunningAsync()
        {
            _logger.LogDebug("Checking status of background service.");

            try
            {
                var response = await _namedPipeClient.SendRequestAsync(StatusRequestMessage);

                if (response == ExpectedResponseMessage)
                {
                    _logger.LogDebug("Background service responded successfully. Status: Running.");
                    return true;
                }
                else
                {
                    _logger.LogWarning("Background service responded with an unexpected message: '{Response}'. Assuming service is in an error state.", response);
                    return false;
                }
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("The request to the background service timed out. Assuming service is not running or is unresponsive.");
                return false;
            }
            catch (PipeBrokenException ex)
            {
                _logger.LogWarning(ex, "The named pipe connection to the background service was broken. Assuming service is not running.");
                return false;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "An I/O error occurred while communicating with the background service via named pipe.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while checking background service status.");
                return false;
            }
        }
    }
}