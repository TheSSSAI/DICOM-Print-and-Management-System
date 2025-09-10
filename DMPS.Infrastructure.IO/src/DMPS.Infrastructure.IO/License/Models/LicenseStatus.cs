namespace DMPS.Infrastructure.IO.License.Models;

/// <summary>
/// Defines the possible outcomes of a license validation check against the external licensing API.
/// </summary>
public enum LicenseStatus
{
    /// <summary>
    /// Indicates the license key is valid and the application is fully functional.
    /// </summary>
    Valid = 0,

    /// <summary>
    /// Indicates the license key provided is not recognized, has expired, or is otherwise invalid.
    /// </summary>
    InvalidKey = 1,

    /// <summary>
    /// Indicates the license API returned a server-side error (e.g., HTTP 5xx) during the validation attempt.
    /// </summary>
    ApiError = 2,

    /// <summary>
    /// Indicates a connection to the license API could not be established after all configured retries.
    /// This typically signifies a network connectivity issue or that the API endpoint is down.
    /// </summary>
    ApiUnreachable = 3
}