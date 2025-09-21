using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels;
using DMPS.Client.Presentation.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace DMPS.Client.Presentation.Services
{
    /// <summary>
    /// Provides a concrete implementation for navigating between different ViewModels.
    /// This service acts as a mediator between the main application shell and the individual page ViewModels.
    /// </summary>
    public sealed class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private ViewModelBase? _currentViewModel;

        /// <inheritdoc />
        public event PropertyChangedEventHandler? CurrentViewModelChanged;

        /// <inheritdoc />
        public ViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                if (Equals(_currentViewModel, value)) return;
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider to resolve ViewModel instances.</param>
        /// <param name="mainWindowViewModel">The main window's ViewModel which hosts the current page.</param>
        public NavigationService(IServiceProvider serviceProvider, MainWindowViewModel mainWindowViewModel)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        }

        /// <inheritdoc />
        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            try
            {
                var newViewModel = _serviceProvider.GetRequiredService<TViewModel>();

                if (CurrentViewModel != null && CurrentViewModel is IDisposable disposableViewModel)
                {
                    disposableViewModel.Dispose();
                }

                CurrentViewModel = newViewModel;
                _mainWindowViewModel.CurrentViewModel = newViewModel;
            }
            catch (InvalidOperationException ex)
            {
                // This typically means the ViewModel is not registered in the DI container.
                // This is a development-time error, so it's appropriate to throw a more informative exception.
                Debug.WriteLine($"Failed to navigate to ViewModel {typeof(TViewModel).Name}. Is it registered in DI container? Error: {ex.Message}");
                throw new InvalidOperationException($"The ViewModel '{typeof(TViewModel).FullName}' is not registered with the dependency injection container. Please register it in App.xaml.cs.", ex);
            }
        }
    }
}