namespace DMPS.Client.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that orchestrates the application's license validation process.
    /// </summary>
    public interface ILicenseValidationService
    {
        /// <summary>
        /// Validates the application's license, typically on startup. This method encapsulates the logic
        /// for contacting the licensing server and handling grace periods in case of network failures.
        /// </summary>
        /// <returns>A task that represents the asynchronous validation operation.</returns>
        Task ValidateLicenseOnStartupAsync();
    }
}