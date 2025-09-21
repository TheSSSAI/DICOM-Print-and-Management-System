using System;
using DMPS.Client.Presentation.ViewModels.Base;

namespace DMPS.Client.Presentation.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that manages navigation between different ViewModels
    /// within the application's main content area.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets the currently active ViewModel.
        /// </summary>
        ViewModelBase? CurrentViewModel { get; }

        /// <summary>
        /// An event that is raised when the current ViewModel changes.
        /// </summary>
        event Action<ViewModelBase>? CurrentViewModelChanged;

        /// <summary>
        /// Navigates to a new view associated with the specified ViewModel type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the ViewModel to navigate to. 
        /// It must derive from ViewModelBase and be registered in the DI container.</typeparam>
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    }
}