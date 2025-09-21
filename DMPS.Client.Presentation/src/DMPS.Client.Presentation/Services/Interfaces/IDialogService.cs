using System.Threading.Tasks;

namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Provides an abstraction for showing various types of dialogs from a ViewModel
    /// without a direct dependency on the View/UI framework.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Displays an informational message dialog to the user.
        /// </summary>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="message">The message to display to the user.</param>
        /// <returns>A task that completes when the dialog is closed.</returns>
        Task ShowMessageAsync(string title, string message);

        /// <summary>
        /// Displays an error message dialog to the user.
        /// </summary>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="message">The error message to display.</param>
        /// <returns>A task that completes when the dialog is closed.</returns>
        Task ShowErrorAsync(string title, string message);

        /// <summary>
        /// Displays a confirmation dialog with "Yes" and "No" options.
        /// </summary>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="message">The confirmation question to ask the user.</param>
        /// <returns>A task that resolves to <c>true</c> if the user clicks "Yes", and <c>false</c> otherwise.</returns>
        Task<bool> ShowConfirmationAsync(string title, string message);
    }
}