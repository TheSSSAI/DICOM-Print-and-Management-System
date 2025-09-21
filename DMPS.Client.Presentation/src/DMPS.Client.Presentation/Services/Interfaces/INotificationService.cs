using System;

namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Defines a service for displaying non-blocking notifications (toasts) to the user.
    /// This is used for providing feedback on asynchronous background operations.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Displays a success notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Optional duration for the notification to be visible.</param>
        void ShowSuccess(string message, TimeSpan? duration = null);

        /// <summary>
        /// Displays an information notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Optional duration for the notification to be visible.</param>
        void ShowInformation(string message, TimeSpan? duration = null);

        /// <summary>
        /// Displays a warning notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Optional duration for the notification to be visible.</param>
        void ShowWarning(string message, TimeSpan? duration = null);

        /// <summary>
        /// Displays an error notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Optional duration for the notification to be visible.</param>
        void ShowError(string message, TimeSpan? duration = null);
    }
}