using System;

namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that monitors user activity and triggers
    /// a session lock after a period of inactivity, as required by REQ-1-041.
    /// </summary>
    public interface ISessionLockService
    {
        /// <summary>
        /// Event that is raised when the inactivity timeout is reached and the session should be locked.
        /// </summary>
        event EventHandler? SessionLockTriggered;

        /// <summary>
        /// Starts the service, beginning the monitoring of user activity.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the service, ending the monitoring of user activity.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Manually resets the inactivity timer. This should be called in response to user input.
        /// </summary>
        void ResetTimer();

        /// <summary>
        /// Gets a value indicating whether the service is currently running.
        /// </summary>
        bool IsRunning { get; }
    }
}