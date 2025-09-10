using DMPS.Infrastructure.Communication.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.Communication.Pipes
{
    /// <summary>
    /// Implements a server-side listener for Named Pipe communication, designed to be run within a background service.
    /// </summary>
    public sealed class NamedPipeServer : INamedPipeServer, IDisposable
    {
        private readonly PipeSettings _settings;
        private readonly ILogger<NamedPipeServer> _logger;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _listeningTask;
        private Func<string, Task<string>>? _onRequestReceived;

        public NamedPipeServer(IOptions<PipeSettings> settings, ILogger<NamedPipeServer> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_settings.PipeName))
            {
                throw new ArgumentException("PipeName cannot be null or whitespace.", nameof(settings));
            }
        }

        /// <inheritdoc />
        public void StartListening(Func<string, Task<string>> onRequestReceived)
        {
            if (_listeningTask != null && !_listeningTask.IsCompleted)
            {
                _logger.LogWarning("Named pipe server is already running for pipe '{PipeName}'.", _settings.PipeName);
                return;
            }

            _onRequestReceived = onRequestReceived ?? throw new ArgumentNullException(nameof(onRequestReceived));
            _cancellationTokenSource = new CancellationTokenSource();
            _listeningTask = Task.Run(() => ListenForConnections(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            _logger.LogInformation("Named pipe server started and listening on '{PipeName}'.", _settings.PipeName);
        }

        /// <inheritdoc />
        public void StopListening()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            _logger.LogInformation("Stopping named pipe server for pipe '{PipeName}'.", _settings.PipeName);
            _cancellationTokenSource.Cancel();
            _listeningTask?.Wait(TimeSpan.FromSeconds(5));
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _logger.LogInformation("Named pipe server stopped for pipe '{PipeName}'.", _settings.PipeName);
        }

        private async Task ListenForConnections(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                NamedPipeServerStream? pipeServer = null;
                try
                {
                    pipeServer = CreatePipeServer();
                    _logger.LogDebug("Waiting for a client connection on pipe '{PipeName}'.", _settings.PipeName);
                    
                    await pipeServer.WaitForConnectionAsync(token).ConfigureAwait(false);
                    
                    _logger.LogInformation("Client connected to named pipe '{PipeName}'.", _settings.PipeName);
                    
                    // Do not await this; let it run in the background to handle the current client
                    // while the loop continues to wait for the next connection.
                    _ = Task.Run(() => HandleConnectionAsync(pipeServer, token), token);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Listening operation was canceled for pipe '{PipeName}'.", _settings.PipeName);
                    pipeServer?.Dispose();
                    break; 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the named pipe listening loop for pipe '{PipeName}'. The loop will continue.", _settings.PipeName);
                    pipeServer?.Dispose();
                    // Brief delay to prevent tight loop on persistent error.
                    await Task.Delay(1000, token).ConfigureAwait(false);
                }
            }
        }

        private async Task HandleConnectionAsync(NamedPipeServerStream pipeServer, CancellationToken token)
        {
            try
            {
                await using (pipeServer.ConfigureAwait(false))
                await using (var writer = new StreamWriter(pipeServer) { AutoFlush = true }.ConfigureAwait(false))
                using (var reader = new StreamReader(pipeServer))
                {
                    var request = await reader.ReadLineAsync(token).ConfigureAwait(false);
                    if (request != null && _onRequestReceived != null)
                    {
                        _logger.LogDebug("Received request on pipe '{PipeName}': {Request}", _settings.PipeName, request);
                        var response = await _onRequestReceived(request).ConfigureAwait(false);
                        await writer.BaseStream.WriteLineAsync(response.AsMemory(), token).ConfigureAwait(false);
                        _logger.LogDebug("Sent response on pipe '{PipeName}': {Response}", _settings.PipeName, response);
                    }
                }
            }
            catch (IOException ex)
            {
                 _logger.LogWarning(ex, "An I/O error occurred during client communication on pipe '{PipeName}'. Client may have disconnected.", _settings.PipeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while handling a client connection on pipe '{PipeName}'.", _settings.PipeName);
            }
        }
        
        private NamedPipeServerStream CreatePipeServer()
        {
            if (OperatingSystem.IsWindows())
            {
                var pipeSecurity = new PipeSecurity();
                var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
                pipeSecurity.AddAccessRule(new PipeAccessRule(sid, PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
                
                return NamedPipeServerStreamAcl.Create(
                    _settings.PipeName, 
                    PipeDirection.InOut, 
                    NamedPipeServerStream.MaxAllowedServerInstances, 
                    PipeTransmissionMode.Byte, 
                    PipeOptions.Asynchronous, 
                    0, 
                    0, 
                    pipeSecurity);
            }

            // For non-Windows platforms, ACLs are not supported in the same way.
            // Behavior might differ, but this provides a basic cross-platform implementation.
            return new NamedPipeServerStream(
                _settings.PipeName, 
                PipeDirection.InOut, 
                NamedPipeServerStream.MaxAllowedServerInstances, 
                PipeTransmissionMode.Byte, 
                PipeOptions.Asynchronous);
        }

        public void Dispose()
        {
            StopListening();
        }
    }
}