namespace DMPS.Client.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for the user inactivity session lock service.
    /// This service monitors user activity and triggers a lock event when a timeout is reached.
    /// </summary>
    public interface ISessionLockService
    {
        /// <summary>
        /// Event raised when the session lock timeout is reached.
        /// The presentation layer should subscribe to this event to display the lock screen.
        /// </summary>
        event EventHandler? SessionLockTriggered;

        /// <summary>
        /// Starts the inactivity monitoring.
        /// </summary>
        /// <param name="inactivityTimeout">The duration of inactivity that will trigger the lock.</param>
        void Start(TimeSpan inactivityTimeout);

        /// <summary>
        /// Resets the inactivity timer. This should be called by the presentation layer
        /// in response to any user keyboard or mouse input.
        /// </summary>
        void ResetInactivityTimer();
    }
}