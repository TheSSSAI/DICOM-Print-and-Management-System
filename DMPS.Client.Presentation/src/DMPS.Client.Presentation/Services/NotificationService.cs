using DMPS.Client.Presentation.Services.Interfaces;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DMPS.Client.Presentation.Services
{
    /// <summary>
    /// A service for displaying non-blocking notifications (snackbars) to the user.
    /// This implementation uses the Material Design in XAML Toolkit's Snackbar component.
    /// </summary>
    public sealed class NotificationService : INotificationService
    {
        private readonly SnackbarMessageQueue _messageQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="messageQueue">The message queue associated with the main window's snackbar.</param>
        public NotificationService(SnackbarMessageQueue messageQueue)
        {
            _messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
        }

        /// <inheritdoc />
        public Task ShowSuccessAsync(string message) => ShowNotificationAsync(message, "Success");

        /// <inheritdoc />
        public Task ShowInfoAsync(string message) => ShowNotificationAsync(message, "Info");

        /// <inheritdoc />
        public Task ShowWarningAsync(string message) => ShowNotificationAsync(message, "Warning");

        /// <inheritdoc />
        public Task ShowErrorAsync(string message) => ShowNotificationAsync(message, "Error");

        private Task ShowNotificationAsync(string message, string category)
        {
            var duration = category == "Error" ? TimeSpan.FromSeconds(10) : TimeSpan.FromSeconds(5);
            
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _messageQueue.Enqueue(message, neverConsiderToBeDuplicate: true, promote: true, durationOverride: duration);
                return Task.CompletedTask;
            }
            else
            {
                return Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _messageQueue.Enqueue(message, neverConsiderToBeDuplicate: true, promote: true, durationOverride: duration);
                }).Task;
            }
        }
    }
}