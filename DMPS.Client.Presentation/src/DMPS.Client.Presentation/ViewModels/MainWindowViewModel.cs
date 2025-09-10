using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Pages;
using DMPS.Client.Presentation.ViewModels.Pages.Admin;
using DMPS.Shared.Core.Dtos;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels
{
    public sealed record NavigationItem(string Name, PackIconKind Icon, Type ViewModelType);

    public partial class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly IStateService _stateService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly ISessionLockService _sessionLockService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ViewModelBase? _currentViewModel;

        [ObservableProperty]
        private UserDto? _currentUser;

        [ObservableProperty]
        private bool _isLocked;

        public ObservableCollection<NavigationItem> MenuItems { get; } = new();
        public ObservableCollection<NavigationItem> FooterMenuItems { get; } = new();

        public MainWindowViewModel(
            IStateService stateService,
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            ISessionLockService sessionLockService,
            IDialogService dialogService)
        {
            _stateService = stateService;
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _sessionLockService = sessionLockService;
            _dialogService = dialogService;

            // Subscribe to global state changes
            _stateService.CurrentUserChanged += OnCurrentUserChanged;
            _sessionLockService.SessionLocked += OnSessionLocked;
            _sessionLockService.SessionUnlocked += OnSessionUnlocked;

            _navigationService.CurrentViewModelChanged += (sender, vm) => CurrentViewModel = vm;

            // Initialize with current state, in case the app starts with a logged-in user (e.g., from a persisted session)
            OnCurrentUserChanged(this, _stateService.CurrentUser);
        }

        private void OnCurrentUserChanged(object? sender, UserDto? user)
        {
            CurrentUser = user;
            BuildMenuItems();
            if (user is not null)
            {
                // Navigate to the default view after login
                _navigationService.NavigateTo(typeof(StudyBrowserViewModel));
            }
        }
        
        private void BuildMenuItems()
        {
            MenuItems.Clear();
            FooterMenuItems.Clear();

            if (CurrentUser is null)
            {
                // No user logged in, so no menu items
                return;
            }

            // Standard items available to all roles
            MenuItems.Add(new NavigationItem("Study Browser", PackIconKind.FolderImage, typeof(StudyBrowserViewModel)));
            MenuItems.Add(new NavigationItem("Print Preview", PackIconKind.Printer, typeof(PrintPreviewViewModel)));
            MenuItems.Add(new NavigationItem("Query/Retrieve", PackIconKind.CloudSearch, typeof(QueryRetrieveViewModel)));

            // Admin-specific items
            if (CurrentUser.Role == "Administrator")
            {
                var adminHeader = new NavigationItem("Administration", PackIconKind.ShieldAccount, typeof(UserManagementViewModel));
                MenuItems.Add(adminHeader); // Can be used as a group header or main navigation point
                
                // If you have a separate settings/admin page with sub-navigation, you might add them differently
                // For this example, we'll assume they are top-level for simplicity.
                FooterMenuItems.Add(new NavigationItem("User Management", PackIconKind.AccountGroup, typeof(UserManagementViewModel)));
                FooterMenuItems.Add(new NavigationItem("System Health", PackIconKind.HeartPulse, typeof(SystemHealthViewModel)));
                FooterMenuItems.Add(new NavigationItem("Audit Trail", PackIconKind.History, typeof(AuditTrailViewModel)));
            }
        }

        [RelayCommand]
        private void Navigate(Type viewModelType)
        {
            if (viewModelType is not null)
            {
                _navigationService.NavigateTo(viewModelType);
            }
        }

        [AsyncRelayCommand]
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
                IsBusy = true;
                await _authenticationService.LogoutAsync();
                // The CurrentUserChanged event will handle UI updates, including navigating away or clearing data.
                _navigationService.NavigateTo(typeof(LoginViewModel));
            }
            catch (Exception ex)
            {
                // In a real app, inject a logger and log the exception.
                await _dialogService.ShowMessageBoxAsync("Logout Failed", "An unexpected error occurred during logout. Please try again.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnSessionLocked(object? sender, EventArgs e)
        {
            IsLocked = true;
        }

        private void OnSessionUnlocked(object? sender, EventArgs e)
        {
            IsLocked = false;
        }

        public void Dispose()
        {
            // Unsubscribe from events to prevent memory leaks
            _stateService.CurrentUserChanged -= OnCurrentUserChanged;
            _sessionLockService.SessionLocked -= OnSessionLocked;
            _sessionLockService.SessionUnlocked -= OnSessionUnlocked;
            _navigationService.CurrentViewModelChanged -= (sender, vm) => CurrentViewModel = vm;
        }
    }
}