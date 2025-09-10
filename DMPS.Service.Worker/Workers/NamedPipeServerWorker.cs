using DMPS.Infrastructure.Communication.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DMPS.Service.Worker.Workers
{
    /// <summary>
    /// A background worker responsible for hosting a Named Pipe server.
    /// This provides a low-latency, synchronous Inter-Process Communication (IPC) channel
    /// for the WPF client to perform real-time status checks.
    /// </summary>
    public sealed class NamedPipeServerWorker : BackgroundService
    {
        private readonly ILogger<NamedPipeServerWorker> _logger;
        private readonly INamedPipeServer _namedPipeServer;

        public NamedPipeServerWorker(
            ILogger<NamedPipeServerWorker> logger,
            INamedPipeServer namedPipeServer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(namedPipeServer);

            _logger = logger;
            _namedPipeServer = namedPipeServer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Named Pipe Server Worker is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("Named Pipe Server Worker is stopping."));

            try
            {
                await _namedPipeServer.StartListeningAsync(HandlePipeRequest, stoppingToken);
                
                _logger.LogInformation("Named Pipe Server is listening.");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Named Pipe Server Worker was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A fatal error occurred in the Named Pipe Server Worker, which is now stopping.");
            }
            finally
            {
                _logger.LogInformation("Named Pipe Server Worker has gracefully shut down.");
            }
        }

        /// <summary>
        /// Handles incoming requests from the named pipe client.
        /// This implementation provides a simple PING/PONG mechanism for service health checks.
        /// </summary>
        /// <param name="request">The request string received from the client.</param>
        /// <returns>The response string to be sent back to the client.</returns>
        private Task<string> HandlePipeRequest(string request)
        {
            _logger.LogDebug("Named Pipe server received request: '{Request}'", request);

            // This can be extended to handle more complex synchronous queries from the client,
            // such as checking for duplicate Study UIDs before a file import.
            // For now, it only handles a simple health check.
            var response = request.ToUpperInvariant() switch
            {
                "PING" => "PONG",
                _ => $"UNKNOWN_COMMAND: {request}"
            };
            
            _logger.LogDebug("Named Pipe server sending response: '{Response}'", response);
            return Task.FromResult(response);
        }
    }
}