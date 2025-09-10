using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Controls;

namespace DMPS.Client.Presentation.Services;

/// <summary>
/// A service for managing navigation between different views/pages within the application shell.
/// This service is designed to be used in an MVVM architecture, navigating based on ViewModel types.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;
    private readonly Dictionary<Type, Type> _viewModelToViewMappings = [];
    private Frame? _mainFrame;
    private ViewModelBase? _currentViewModel;

    public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public void Initialize(Frame navigationFrame)
    {
        _mainFrame = navigationFrame ?? throw new ArgumentNullException(nameof(navigationFrame));
        _logger.LogInformation("NavigationService initialized with a navigation frame.");
    }

    /// <inheritdoc />
    public void Configure<TViewModel, TView>() where TViewModel : ViewModelBase where TView : FrameworkElement
    {
        var viewModelType = typeof(TViewModel);
        var viewType = typeof(TView);

        if (_viewModelToViewMappings.ContainsKey(viewModelType))
        {
            _logger.LogWarning("Mapping for ViewModel type {ViewModelType} is already configured. It will be overwritten.", viewModelType.FullName);
            _viewModelToViewMappings[viewModelType] = viewType;
        }
        else
        {
            _viewModelToViewMappings.Add(viewModelType, viewType);
            _logger.LogDebug("Configured navigation mapping: {ViewModelType} -> {ViewType}", viewModelType.FullName, viewType.FullName);
        }
    }

    /// <inheritdoc />
    public async Task NavigateToAsync<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var viewModelType = typeof(TViewModel);

            if (_mainFrame is null)
            {
                var ex = new InvalidOperationException("NavigationService has not been initialized. Call Initialize() before navigating.");
                _logger.LogError(ex, "Attempted to navigate before initialization.");
                throw ex;
            }

            if (!_viewModelToViewMappings.TryGetValue(viewModelType, out var viewType))
            {
                var ex = new InvalidOperationException($"No view is configured for the view model '{viewModelType.FullName}'.");
                _logger.LogError(ex, "Navigation failed due to missing view model mapping.");
                throw ex;
            }

            // Avoid re-navigating to the same view model type unless it's a different instance or parameterized
            if (_currentViewModel?.GetType() == viewModelType && parameter == null)
            {
                _logger.LogInformation("Navigation to {ViewModelType} skipped as it is already the current view.", viewModelType.FullName);
                return;
            }

            try
            {
                // Resolve dependencies for the new view and view model
                var view = _serviceProvider.GetRequiredService(viewType) as FrameworkElement;
                var viewModel = _serviceProvider.GetRequiredService(viewModelType) as TViewModel;
                
                if (view is null || viewModel is null)
                {
                    var ex = new InvalidOperationException($"Failed to resolve view or view model from DI container for type {viewModelType.FullName}.");
                    _logger.LogError(ex, "DI resolution failed during navigation.");
                    throw ex;
                }

                view.DataContext = viewModel;

                _logger.LogInformation("Navigating to {ViewType} with ViewModel {ViewModelType}.", viewType.FullName, viewModelType.FullName);

                // Perform cleanup on the old ViewModel
                if (_currentViewModel is not null && _currentViewModel is IDisposable disposableOldViewModel)
                {
                    disposableOldViewModel.Dispose();
                    _logger.LogDebug("Disposed previous ViewModel: {OldViewModelType}", _currentViewModel.GetType().FullName);
                }

                // Navigate the frame and update the current view model
                _mainFrame.Navigate(view);
                _currentViewModel = viewModel;

                // Call an initialization method on the new ViewModel if it exists
                if (viewModel is INavigationAware navigationAwareViewModel)
                {
                    await navigationAwareViewModel.OnNavigatedToAsync(parameter);
                    _logger.LogDebug("Executed OnNavigatedToAsync for {ViewModelType}.", viewModelType.FullName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during navigation to {ViewModelType}.", viewModelType.FullName);
                // Optionally, navigate to an error page or show a dialog
                throw; // Rethrow to allow higher-level error handling
            }
        });
    }
}