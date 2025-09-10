using DMPS.Infrastructure.Communication.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.Communication.Pipes
{
    /// <summary>
    /// Implements a client for synchronous request/reply communication over a .NET Named Pipe.
    /// This is intended for low-latency status checks.
    /// </summary>
    public sealed class NamedPipeClient : INamedPipeClient
    {
        private readonly PipeSettings _settings;
        private readonly ILogger<NamedPipeClient> _logger;

        public NamedPipeClient(IOptions<PipeSettings> settings, ILogger<NamedPipeClient> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_settings.PipeName))
            {
                throw new ArgumentException("PipeName cannot be null or whitespace.", nameof(settings));
            }
        }

        /// <inheritdoc />
        public async Task<string?> SendRequestAsync(string request, CancellationToken token)
        {
            try
            {
                await using var pipeClient = new NamedPipeClientStream(".", _settings.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                
                _logger.LogDebug("Attempting to connect to named pipe '{PipeName}' with a timeout of {Timeout}ms.", _settings.PipeName, _settings.ConnectionTimeoutMs);
                
                await pipeClient.ConnectAsync(_settings.ConnectionTimeoutMs, token).ConfigureAwait(false);

                _logger.LogDebug("Successfully connected to named pipe '{PipeName}'.", _settings.PipeName);

                await using var writer = new StreamWriter(pipeClient) { AutoFlush = true };
                using var reader = new StreamReader(pipeClient);
                
                await writer.WriteLineAsync(request.AsMemory(), token).ConfigureAwait(false);
                _logger.LogDebug("Sent request to pipe '{PipeName}': {Request}", _settings.PipeName, request);
                
                var response = await reader.ReadLineAsync(token).ConfigureAwait(false);
                _logger.LogDebug("Received response from pipe '{PipeName}': {Response}", _settings.PipeName, response);
                
                return response;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Connection to named pipe '{PipeName}' timed out. The server service may not be running.", _settings.PipeName);
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Named pipe request was canceled.");
                return null;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "An I/O error occurred while communicating with the named pipe '{PipeName}'.", _settings.PipeName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred in NamedPipeClient for pipe '{PipeName}'.", _settings.PipeName);
                return null;
            }
        }
    }
}