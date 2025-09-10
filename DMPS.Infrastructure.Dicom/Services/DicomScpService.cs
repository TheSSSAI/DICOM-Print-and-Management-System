using DMPS.Infrastructure.Dicom.Configuration;
using DMPS.Infrastructure.Dicom.Interfaces;
using FellowOakDicom;
using FellowOakDicom.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DMPS.Infrastructure.Dicom.Services;

/// <summary>
/// Service for managing the DICOM C-STORE SCP (Service Class Provider) listener.
/// This service is responsible for starting and stopping the DICOM server.
/// </summary>
public sealed class DicomScpService : IDicomScpService, IDisposable
{
    private readonly ILogger<DicomScpService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly DicomScpOptions _options;
    private IDicomServer? _server;

    public DicomScpService(ILogger<DicomScpService> logger, ILoggerFactory loggerFactory, IOptions<DicomScpOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Starts the DICOM SCP listener on the configured port.
    /// </summary>
    /// <param name="onFileReceived">A callback action that is invoked when a DICOM file is successfully received.</param>
    public void StartListening(Action<DicomFile> onFileReceived)
    {
        if (_server != null && _server.IsListening)
        {
            _logger.LogWarning("DICOM SCP service is already listening on port {Port}.", _options.Port);
            return;
        }

        try
        {
            _logger.LogInformation("Starting DICOM C-STORE SCP on port {Port}...", _options.Port);

            var userState = new ScpUserState(onFileReceived, _loggerFactory);
            
            _server = DicomServer.Create<InternalDicomStoreService>(_options.Port, userState: userState);

            _logger.LogInformation("DICOM C-STORE SCP started successfully. Listening on port {Port}.", _options.Port);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start DICOM SCP on port {Port}. Port may be in use or permissions may be insufficient.", _options.Port);
            _server?.Dispose();
            _server = null;
            throw; // Rethrow to indicate a critical startup failure
        }
    }

    /// <summary>
    /// Stops the DICOM SCP listener.
    /// </summary>
    public void StopListening()
    {
        if (_server == null || !_server.IsListening)
        {
            _logger.LogInformation("DICOM SCP service is not currently listening.");
            return;
        }

        _logger.LogInformation("Stopping DICOM SCP service...");
        _server.Stop();
        _logger.LogInformation("DICOM SCP service stopped.");
    }
    
    public bool IsListening => _server?.IsListening ?? false;

    public void Dispose()
    {
        StopListening();
        _server?.Dispose();
        _server = null;
    }

    /// <summary>
    /// Represents the user state passed to the fo-dicom service instance.
    /// </summary>
    /// <param name="OnFileReceived">The callback to invoke when a file is received.</param>
    /// <param name="LoggerFactory">The logger factory to create loggers within the fo-dicom service.</param>
    private record ScpUserState(Action<DicomFile> OnFileReceived, ILoggerFactory LoggerFactory);
    
    /// <summary>
    /// Internal class that implements the fo-dicom C-STORE provider logic.
    /// This class is instantiated by the DicomServer and is not managed by the application's DI container.
    /// </summary>
    private class InternalDicomStoreService : DicomService, IDicomServiceProvider, IDicomCStoreProvider
    {
        private readonly ILogger _logger;
        private readonly Action<DicomFile> _onFileReceived;

        public InternalDicomStoreService(INetworkStream stream, DicomServiceDependencies dependencies, ScpUserState userState) 
            : base(stream, dependencies)
        {
            _onFileReceived = userState.OnFileReceived ?? throw new ArgumentNullException(nameof(userState.OnFileReceived));
            _logger = userState.LoggerFactory.CreateLogger<InternalDicomStoreService>();
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            _logger.LogInformation("Received association request from AE: {CallingAE} on IP: {RemoteIP}", 
                association.CallingAE, Association.RemoteHost);

            foreach (var pc in association.PresentationContexts)
            {
                pc.SetResult(DicomPresentationContextResult.Accept);
            }

            return SendAssociationAcceptAsync(association);
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            _logger.LogWarning("Received abort from {Source} with reason {Reason}", source, reason);
        }

        public void OnConnectionClosed(Exception exception)
        {
            if (exception != null)
            {
                _logger.LogError(exception, "Connection closed with an exception.");
            }
        }

        public async Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
        {
            var studyInstanceUid = request.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "UnknownStudy");
            var sopInstanceUid = request.SOPInstanceUID.UID ?? "UnknownInstance";

            _logger.LogInformation("Received C-STORE request for StudyUID: {StudyUID}, SOPInstanceUID: {SOPInstanceUID}",
                studyInstanceUid, sopInstanceUid);

            try
            {
                _onFileReceived(request.File);
                
                _logger.LogInformation("Successfully processed C-STORE request for SOPInstanceUID: {SOPInstanceUID}", sopInstanceUid);
                return new DicomCStoreResponse(request, DicomStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing C-STORE request for SOPInstanceUID: {SOPInstanceUID}. The configured callback failed.", sopInstanceUid);
                return new DicomCStoreResponse(request, DicomStatus.ProcessingFailure);
            }
        }

        public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
        {
            _logger.LogError(e, "Exception processing C-STORE request for temporary file: {TempFileName}", tempFileName);
            return Task.CompletedTask;
        }
    }
}