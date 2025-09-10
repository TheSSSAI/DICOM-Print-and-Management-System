using DMPS.Application.Interfaces;
using DMPS.CrossCutting.Security.Services;
using DMPS.Infrastructure.IO.Interfaces;
using DMPS.Infrastructure.IO.License.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.IO.License
{
    /// <summary>
    /// A typed HttpClient for communicating with the external Odoo REST API for license validation.
    /// This implementation is designed to be resilient to transient network failures.
    /// </summary>
    public sealed class OdooApiClient(
        HttpClient httpClient,
        ISecureCredentialStore credentialStore,
        IOptions<OdooApiSettings> settings,
        ILogger<OdooApiClient> logger) : ILicenseApiClient
    {
        private readonly OdooApiSettings _settings = settings.Value;
        
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <inheritdoc />
        public async Task<LicenseStatus> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                logger.LogWarning("License validation attempted with a null or empty license key.");
                return LicenseStatus.InvalidKey;
            }

            if (string.IsNullOrWhiteSpace(_settings.BaseUrl) || string.IsNullOrWhiteSpace(_settings.ApiKeySecretName))
            {
                logger.LogCritical("Odoo API settings (BaseUrl or ApiKeySecretName) are not configured. License validation cannot proceed.");
                return LicenseStatus.ApiError;
            }

            logger.LogInformation("Initiating license validation for key ending in '...{Last4Chars}'", licenseKey.Length > 4 ? licenseKey[^4..] : licenseKey);

            try
            {
                var apiKey = await credentialStore.RetrieveSecretAsync(_settings.ApiKeySecretName);
                if (string.IsNullOrEmpty(apiKey))
                {
                    logger.LogCritical("Failed to retrieve Odoo API key from secure credential store using secret name '{SecretName}'.", _settings.ApiKeySecretName);
                    return LicenseStatus.ApiError;
                }

                var request = new OdooLicenseRequest { LicenseKey = licenseKey };
                
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/license/validate");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                requestMessage.Content = JsonContent.Create(request);

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Received successful (HTTP {StatusCode}) response from Odoo API.", (int)response.StatusCode);
                    var odooResponse = await response.Content.ReadFromJsonAsync<OdooLicenseResponse>(_jsonOptions, cancellationToken);
                    
                    return MapResponseToStatus(odooResponse);
                }
                
                // Handle non-success status codes
                logger.LogWarning("Odoo API returned a non-success status code: {StatusCode}. Reason: {ReasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);
                return MapErrorStatusCodeToStatus(response.StatusCode);

            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested) // This indicates a timeout from Polly
            {
                logger.LogError(ex, "The license validation request to Odoo API timed out.");
                return LicenseStatus.ApiUnreachable;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An HTTP request exception occurred during license validation. The API may be unreachable.");
                return LicenseStatus.ApiUnreachable;
            }
            catch (SocketException ex)
            {
                logger.LogError(ex, "A socket exception occurred during license validation. Check network connectivity to Odoo API.");
                return LicenseStatus.ApiUnreachable;
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to deserialize the response from Odoo API. The API contract may have changed.");
                return LicenseStatus.ApiError;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An unexpected critical error occurred during license validation.");
                return LicenseStatus.ApiError;
            }
        }
        
        private LicenseStatus MapResponseToStatus(OdooLicenseResponse? response)
        {
            if (response is null)
            {
                logger.LogWarning("Odoo API response body was null or could not be parsed.");
                return LicenseStatus.ApiError;
            }

            return response.Status.ToLowerInvariant() switch
            {
                "valid" => LicenseStatus.Valid,
                "inactive" => LicenseStatus.InvalidKey,
                "expired" => LicenseStatus.InvalidKey,
                "not_found" => LicenseStatus.InvalidKey,
                _ => LicenseStatus.InvalidKey,
            };
        }

        private static LicenseStatus MapErrorStatusCodeToStatus(System.Net.HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden => LicenseStatus.InvalidKey,
                >= System.Net.HttpStatusCode.InternalServerError => LicenseStatus.ApiError,
                _ => LicenseStatus.ApiError,
            };
        }
    }
}