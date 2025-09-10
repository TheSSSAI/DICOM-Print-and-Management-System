using System.Threading.Tasks;

namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Represents the possible results of a dialog interaction.
    /// </summary>
    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }

    /// <summary>
    /// Defines a contract for a service that displays dialogs to the user.
    /// This allows ViewModels to show dialogs without a direct dependency on UI-specific components,
    /// which is crucial for testability and adherence to the MVVM pattern.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a modal message box with a title, message, and specified buttons.
        /// </summary>
        /// <param name="title">The title of the message box window.</param>
        /// <param name="message">The message to be displayed to the user.</param>
        /// <returns>A task that resolves to a <see cref="DialogResult"/> indicating which button the user pressed.</returns>
        Task<DialogResult> ShowMessageBoxAsync(string title, string message);

        /// <summary>
        /// Shows a modal message box with a title, message, and specified buttons for confirmation.
        /// </summary>
        /// <param name="title">The title of the confirmation dialog window.</param>
        /// <param name="message">The confirmation message to be displayed.</param>
        /// <returns>A task that resolves to true if the user confirmed (Yes/OK), and false otherwise.</returns>
        Task<bool> ShowConfirmationAsync(string title, string message);

        /// <summary>
        /// Shows a custom dialog associated with a specific ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the ViewModel for the custom dialog.</typeparam>
        /// <param name="viewModel">The instance of the ViewModel to be used as the DataContext for the dialog view.</param>
        /// <returns>A task that resolves to an object representing the result of the dialog interaction.</returns>
        Task<object?> ShowDialogAsync<TViewModel>(TViewModel viewModel) where TViewModel : class;
    }
}