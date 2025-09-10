using DMPS.Infrastructure.Dicom.Exceptions;
using DMPS.Infrastructure.Dicom.Interfaces;
using DMPS.Shared.Core.Configuration;
using FellowOakDicom;
using FellowOakDicom.Network;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DMPS.Infrastructure.Dicom.Services;

/// <summary>
/// Service for performing DICOM SCU (Service Class User) operations like C-ECHO, C-FIND, and C-MOVE.
/// </summary>
public sealed class DicomScuService : IDicomScuService
{
    private readonly ILogger<DicomScuService> _logger;

    public DicomScuService(ILogger<DicomScuService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> VerifyPacsConnectionAsync(PacsConfiguration config, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient(config);
            var request = new DicomCEchoRequest();

            request.OnResponseReceived += (req, res) =>
            {
                _logger.LogInformation("C-ECHO response received from {AETitle} with status {Status}", config.AeTitle, res.Status);
            };

            await client.AddRequestAsync(request);
            await client.SendAsync(cancellationToken);

            return request.Status == DicomStatus.Success;
        }
        catch (Exception ex) when (ex is DicomNetworkException or DicomAssociationRejectedException or DicomAssociationAbortedException or TimeoutException)
        {
            _logger.LogWarning(ex, "Failed to verify connection to PACS {AETitle} at {Host}:{Port}.", config.AeTitle, config.Hostname, config.Port);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during C-ECHO verification for PACS {AETitle}.", config.AeTitle);
            throw new PacsConnectionException($"An unexpected error occurred while verifying PACS '{config.AeTitle}'. See inner exception for details.", ex);
        }
    }

    public async Task<List<DicomDataset>> QueryStudiesAsync(PacsConfiguration config, DicomDataset queryParameters, CancellationToken cancellationToken = default)
    {
        var results = new List<DicomDataset>();
        try
        {
            var client = CreateClient(config);
            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study)
            {
                Dataset = queryParameters
            };

            request.OnResponseReceived += (req, res) =>
            {
                if (res.Status.State == DicomState.Pending)
                {
                    results.Add(res.Dataset);
                }
                else if (res.Status.State == DicomState.Success)
                {
                    _logger.LogInformation("C-FIND completed successfully for PACS {AETitle}. Found {Count} studies.", config.AeTitle, results.Count);
                }
                else
                {
                    _logger.LogWarning("C-FIND response from {AETitle} with non-success status: {Status}", config.AeTitle, res.Status);
                }
            };

            await client.AddRequestAsync(request);
            await client.SendAsync(cancellationToken);
            
            if (request.Status != DicomStatus.Success)
            {
                 throw new DicomIntegrationException($"C-FIND query failed with status: {request.Status}");
            }

            return results;
        }
        catch (Exception ex) when (ex is DicomNetworkException or DicomAssociationRejectedException or DicomAssociationAbortedException or TimeoutException)
        {
            _logger.LogError(ex, "Failed to query studies from PACS {AETitle} at {Host}:{Port}.", config.AeTitle, config.Hostname, config.Port);
            throw new PacsConnectionException($"Failed to query PACS '{config.AeTitle}' due to a network or association error.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during C-FIND query for PACS {AETitle}.", config.AeTitle);
            throw new DicomIntegrationException($"An unexpected error occurred while querying PACS '{config.AeTitle}'.", ex);
        }
    }

    public async Task<DicomCMoveResponse> MoveStudyAsync(PacsConfiguration config, string studyInstanceUid, string destinationAet, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = CreateClient(config);
            var request = new DicomCMoveRequest(destinationAet, studyInstanceUid);

            DicomCMoveResponse? finalResponse = null;
            var responseBuilder = new StringBuilder();
            
            request.OnResponseReceived += (req, res) =>
            {
                responseBuilder.AppendLine($"C-MOVE response from {config.AeTitle}: Status={res.Status}, Remaining={res.Remaining}, Completed={res.Completed}, Failed={res.Failed}, Warning={res.Warning}");
                finalResponse = res;
            };

            await client.AddRequestAsync(request);
            await client.SendAsync(cancellationToken);
            
            _logger.LogInformation("C-MOVE responses from {AETitle} for study {StudyUID}: \n{Responses}", config.AeTitle, studyInstanceUid, responseBuilder.ToString());

            if (finalResponse is null)
            {
                throw new DicomIntegrationException("C-MOVE operation did not receive a final response from the PACS.");
            }

            if (finalResponse.Status != DicomStatus.Success)
            {
                 _logger.LogWarning("C-MOVE operation for study {StudyUID} failed with final status: {Status}", studyInstanceUid, finalResponse.Status);
            }
            
            return finalResponse;
        }
        catch (Exception ex) when (ex is DicomNetworkException or DicomAssociationRejectedException or DicomAssociationAbortedException or TimeoutException)
        {
            _logger.LogError(ex, "Failed to initiate C-MOVE for study {StudyUID} from PACS {AETitle}.", studyInstanceUid, config.AeTitle);
            throw new PacsConnectionException($"Failed to move study from PACS '{config.AeTitle}' due to a network or association error.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during C-MOVE initiation for study {StudyUID} from PACS {AETitle}.", studyInstanceUid, config.AeTitle);
            throw new DicomIntegrationException($"An unexpected error occurred while moving study from PACS '{config.AeTitle}'.", ex);
        }
    }

    private IDicomClient CreateClient(PacsConfiguration config)
    {
        var client = DicomClientFactory.Create(config.Hostname, config.Port, config.UseTls, config.CallingAeTitle, config.AeTitle);
        client.NegotiateAsyncOps();
        if (config.UseTls && config.TlsOptions != null)
        {
            client.Options.SecurityOptions = new SecurityOptions
            {
                CipherSuites = new DicomTlsCipherSuite[]
                {
                    DicomTlsCipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
                    DicomTlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384
                },
                CertificateValidation = (sender, certificate, chain, errors) => config.TlsOptions.IgnoreCertificateErrors,
                ClientCertificates = string.IsNullOrWhiteSpace(config.TlsOptions.ClientCertificatePath)
                    ? new X509CertificateCollection()
                    : new X509CertificateCollection(new[] { new X509Certificate2(config.TlsOptions.ClientCertificatePath) })
            };
        }
        
        // Setting longer timeouts for potentially slow PACS responses
        client.Options.AssociationRequestTimeout = 15000; // 15 seconds
        client.Options.AssociationLingerTimeout = 1000;
        
        return client;
    }
}