using System;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that manages navigation between different views/viewmodels.
    /// This abstraction allows ViewModels to request navigation without being coupled to the UI framework's specific navigation mechanism.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the view associated with the specified ViewModel type.
        /// </summary>
        /// <param name="viewModelType">The type of the ViewModel to navigate to.</param>
        /// <returns>A task that represents the asynchronous navigation operation.</returns>
        Task NavigateToAsync(Type viewModelType);

        /// <summary>
        /// Navigates to the view associated with the specified ViewModel type, passing a parameter.
        /// </summary>
        /// <param name="viewModelType">The type of the ViewModel to navigate to.</param>
        /// <param name="parameter">The parameter to pass to the target ViewModel after navigation.</param>
        /// <returns>A task that represents the asynchronous navigation operation.</returns>
        Task NavigateToAsync(Type viewModelType, object parameter);

        /// <summary>
        /// Navigates back to the previous view in the navigation stack, if available.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Gets a value indicating whether it is possible to navigate back.
        /// </summary>
        bool CanGoBack { get; }
    }
}