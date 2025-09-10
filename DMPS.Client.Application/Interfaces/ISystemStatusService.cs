namespace DMPS.Client.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that provides real-time status information
    /// about critical system components, such as the background Windows Service.
    /// </summary>
    public interface ISystemStatusService
    {
        /// <summary>
        /// Asynchronously checks if the background Windows Service is running and responsive.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is true if the service
        /// is running and responsive; otherwise, false.
        /// </returns>
        Task<bool> IsBackgroundServiceRunningAsync();
    }
}