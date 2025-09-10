using DMPS.Infrastructure.IO.License.Models;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.IO.Interfaces;

/// <summary>
/// Defines the contract for a client that validates a license key against an external API.
/// Implementations are expected to be resilient to network failures.
/// </summary>
public interface ILicenseApiClient
{
    /// <summary>
    /// Asynchronously sends a license key to an external service for validation.
    /// </summary>
    /// <param name="licenseKey">The license key to validate.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="LicenseStatus"/>
    /// enum value indicating the outcome of the validation. Implementations should handle exceptions
    /// gracefully and return an appropriate status (e.g., ApiUnreachable) rather than throwing.
    /// </returns>
    Task<LicenseStatus> ValidateLicenseAsync(string licenseKey);
}