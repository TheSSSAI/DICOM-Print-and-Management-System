namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that displays non-blocking notifications (e.g., toasts) to the user.
    /// This service is used to provide feedback for asynchronous operations without interrupting the user's workflow.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Displays a success notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void ShowSuccess(string message);

        /// <summary>
        /// Displays an informational notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void ShowInfo(string message);

        /// <summary>
        /// Displays a warning notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void ShowWarning(string message);

        /// <summary>
        /// Displays an error notification.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void ShowError(string message);

        /// <summary>
        /// Displays a notification with a custom title and message.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to display.</param>
        void Show(string title, string message);
    }
}