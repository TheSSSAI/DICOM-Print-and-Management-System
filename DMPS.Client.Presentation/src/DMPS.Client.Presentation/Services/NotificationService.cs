using DMPS.Client.Presentation.Services.Interfaces;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace DMPS.Client.Presentation.Services;

/// <summary>
/// A service to display non-blocking toast notifications (snackbars) to the user.
/// This implementation relies on the MaterialDesignInXamlToolkit's Snackbar component.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly ISnackbarMessageQueue _messageQueue;

    public NotificationService(ISnackbarMessageQueue messageQueue)
    {
        _messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
    }

    /// <inheritdoc />
    public void Show(string message, NotificationType type = NotificationType.Info)
    {
        if (Application.Current.Dispatcher.CheckAccess())
        {
            EnqueueMessage(message, type);
        }
        else
        {
            Application.Current.Dispatcher.Invoke(() => EnqueueMessage(message, type));
        }
    }

    private void EnqueueMessage(string message, NotificationType type)
    {
        // MaterialDesign Snackbar doesn't have built-in styling for different notification types.
        // We can prepend the message with a prefix or an icon in a real application.
        // For simplicity, we just show the message.
        // In a more complex scenario, we could use a custom template for the snackbar content.
        
        string fullMessage = type switch
        {
            NotificationType.Success => $"✅ {message}",
            NotificationType.Error => $"❌ {message}",
            NotificationType.Warning => $"⚠️ {message}",
            _ => $"ℹ️ {message}"
        };

        // The Enqueue method is thread-safe, but it's good practice to ensure it's called
        // from the UI thread context, as UI updates might be triggered.
        _messageQueue.Enqueue(fullMessage);
    }
}