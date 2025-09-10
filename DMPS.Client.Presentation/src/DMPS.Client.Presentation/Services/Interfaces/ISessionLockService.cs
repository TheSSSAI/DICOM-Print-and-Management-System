using System;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that manages automatic session locking based on user inactivity.
    /// This is a critical security feature for HIPAA compliance.
    /// </summary>
    public interface ISessionLockService
    {
        /// <summary>
        /// Occurs when the inactivity timer has elapsed and the session should be locked.
        /// The main window or shell should subscribe to this event to display the lock screen.
        /// </summary>
        event EventHandler? SessionLockRequested;

        /// <summary>
        /// Starts the inactivity monitoring service.
        /// </summary>
        /// <param name="timeout">The duration of inactivity before the session is locked.</param>
        void Start(TimeSpan timeout);

        /// <summary>
        /// Stops the inactivity monitoring service.
        /// </summary>
        void Stop();

        /// <summary>
        /// Resets the inactivity timer. This method should be called whenever user activity is detected.
        /// </summary>
        void ResetTimer();

        /// <summary>
        /// Attempts to unlock the current session by verifying the provided password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <returns>A task that resolves to true if the password is correct and the session is unlocked; otherwise, false.</returns>
        Task<bool> UnlockSessionAsync(string password);
    }
}