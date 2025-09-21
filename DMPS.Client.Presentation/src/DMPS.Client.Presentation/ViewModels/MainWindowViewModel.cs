using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Interfaces;
using DMPS.Client.Application.DTOs;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Client.Presentation.ViewModels.Pages;
using System;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels
{
    /// <summary>
    /// The main shell ViewModel for the application. It orchestrates the main content area,
    /// manages the user session state (logged in/out, role), and controls global UI states
    /// such as the session lock overlay.
    /// </summary>
    public sealed partial class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private const string BASE_APPLICATION_TITLE = "DICOM Management and Printing System";
        private readonly INavigationService _navigationService;
        private readonly ISessionLockService _sessionLockService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ViewModelBase? _currentViewModel;

        [ObservableProperty]
        private bool _isScreenLocked;

        [ObservableProperty]
        private bool _isLoggedIn;

        [ObservableProperty]
        private bool _isAdmin;

        [ObservableProperty]
        private string? _currentUserDisplayName;

        [ObservableProperty]
        private string _applicationTitle = BASE_APPLICATION_TITLE;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service for changing the main content view.</param>
        /// <param name="sessionLockService">The service for managing automatic session locking.</param>
        /// <param name="authenticationService">The service for managing user authentication and session state.</param>
        /// <param name="dialogService">The service for displaying dialogs to the user.</param>
        public MainWindowViewModel(
            INavigationService navigationService,
            ISessionLockService sessionLockService,
            IAuthenticationService authenticationService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _sessionLockService = sessionLockService;
            _authenticationService = authenticationService;
            _dialogService = dialogService;

            SubscribeToEvents();

            // Initial navigation to the login screen
            _navigationService.NavigateTo<LoginViewModel>();
        }

        private void SubscribeToEvents()
        {
            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _sessionLockService.SessionLockTriggered += OnSessionLockTriggered;
            _sessionLockService.SessionUnlocked += OnSessionUnlocked;
            _authenticationService.UserSessionChanged += OnUserSessionChanged;
        }

        private void UnsubscribeFromEvents()
        {
            _navigationService.CurrentViewModelChanged -= OnCurrentViewModelChanged;
            _sessionLockService.SessionLockTriggered -= OnSessionLockTriggered;
            _sessionLockService.SessionUnlocked -= OnSessionUnlocked;
            _authenticationService.UserSessionChanged -= OnUserSessionChanged;
        }

        private void OnUserSessionChanged(object? sender, UserSession? session)
        {
            if (session is not null)
            {
                IsLoggedIn = true;
                IsAdmin = session.Role == UserRole.Administrator;
                CurrentUserDisplayName = session.DisplayName;
                ApplicationTitle = $"{BASE_APPLICATION_TITLE} - {session.DisplayName}";
            }
            else
            {
                IsLoggedIn = false;
                IsAdmin = false;
                CurrentUserDisplayName = null;
                ApplicationTitle = BASE_APPLICATION_TITLE;
            }
        }

        private void OnSessionUnlocked(object? sender, EventArgs e)
        {
            IsScreenLocked = false;
        }

        private void OnSessionLockTriggered(object? sender, EventArgs e)
        {
            IsScreenLocked = true;
        }

        private void OnCurrentViewModelChanged(object? sender, ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }

        private bool CanLogout() => IsLoggedIn;

        [RelayCommand(CanExecute = nameof(CanLogout))]
        private async Task LogoutAsync()
        {
            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Confirm Logout",
                "Are you sure you want to log out? Any unsaved changes will be lost.");

            if (!confirmed)
            {
                return;
            }

            try
            {
                _authenticationService.Logout();
                _navigationService.NavigateTo<LoginViewModel>();
            }
            catch (Exception ex)
            {
                // In a real application, we would log this unexpected exception.
                await _dialogService.ShowErrorAsync("Logout Error", "An unexpected error occurred during logout. Please try again.");
            }
        }

        /// <summary>
        /// Cleans up resources by unsubscribing from events.
        /// </summary>
        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}