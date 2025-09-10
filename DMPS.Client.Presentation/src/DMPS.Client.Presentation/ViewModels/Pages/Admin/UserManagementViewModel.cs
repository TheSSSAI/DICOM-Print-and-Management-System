using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Shared.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Pages.Admin
{
    public sealed partial class UserManagementViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly ISessionStateService _sessionStateService;

        [ObservableProperty]
        private ObservableCollection<User> _users = new();

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private bool _isBusy;
        
        public UserManagementViewModel(
            IUserService userService, 
            IDialogService dialogService,
            INotificationService notificationService,
            ISessionStateService sessionStateService)
        {
            _userService = userService;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _sessionStateService = sessionStateService;
        }

        [RelayCommand]
        private async Task LoadUsersAsync()
        {
            IsBusy = true;
            try
            {
                var result = await _userService.GetAllUsersAsync();
                if (result.IsSuccess && result.Value is not null)
                {
                    // Add IsCurrentUser property for UI logic
                    var currentUser = _sessionStateService.CurrentUser;
                    var userVms = result.Value.Select(u => {
                        u.IsCurrentUser = u.Id == currentUser?.Id;
                        return u;
                    });
                    Users = new ObservableCollection<User>(userVms);
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Error", result.Error ?? "Failed to load users.");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync("Error", $"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand(CanExecute = nameof(CanModifyUser))]
        private async Task ResetPasswordAsync()
        {
            var confirm = await _dialogService.ShowMessageBoxAsync("Confirm Reset", $"Are you sure you want to reset the password for {SelectedUser!.Username}?", DialogButton.YesNo);
            if (confirm != DialogResult.Yes) return;
            
            IsBusy = true;
            try
            {
                var result = await _userService.ResetPasswordAsync(SelectedUser!.Id);
                if(result.IsSuccess)
                {
                    await _dialogService.ShowMessageBoxAsync("Password Reset", $"The temporary password for {SelectedUser.Username} is:\n\n{result.Value}\n\nPlease provide this to the user securely.");
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Error", result.Error ?? "Failed to reset password.");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand(CanExecute = nameof(CanModifyUser))]
        private async Task ToggleUserStatusAsync()
        {
            string action = SelectedUser!.IsActive ? "disable" : "enable";
            var confirm = await _dialogService.ShowMessageBoxAsync("Confirm Action", $"Are you sure you want to {action} the account for {SelectedUser!.Username}?", DialogButton.YesNo);
            if (confirm != DialogResult.Yes) return;

            IsBusy = true;
            try
            {
                var result = await _userService.SetUserStatusAsync(SelectedUser!.Id, !SelectedUser.IsActive);
                if (result.IsSuccess)
                {
                    _notificationService.ShowSuccess("Success", $"User {SelectedUser.Username} has been {action}d.");
                    await LoadUsersAsync(); // Refresh the list
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Error", result.Error ?? $"Failed to {action} user.");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand(CanExecute = nameof(CanModifyUser))]
        private async Task DeleteUserAsync()
        {
            var confirm = await _dialogService.ShowMessageBoxAsync("Confirm Deletion", $"This will permanently delete the user {SelectedUser!.Username}. This action cannot be undone. Are you sure?", DialogButton.YesNo);
            if (confirm != DialogResult.Yes) return;
            
            IsBusy = true;
            try
            {
                var result = await _userService.DeleteUserAsync(SelectedUser!.Id);
                if(result.IsSuccess)
                {
                     _notificationService.ShowSuccess("Success", $"User {SelectedUser.Username} has been deleted.");
                     await LoadUsersAsync();
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Error", result.Error ?? "Failed to delete user.");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanModifyUser()
        {
            if (SelectedUser is null || IsBusy) return false;
            // Prevent self-modification for critical actions
            return !SelectedUser.IsCurrentUser;
        }

        // AddUser and EditUser commands would typically navigate to another view/dialog
        // This is a simplified implementation for brevity.
        [RelayCommand]
        private async Task AddUserAsync()
        {
            // This would normally open a dialog to get new user details
            await _dialogService.ShowMessageBoxAsync("Add User", "This would open a dialog to create a new user.");
        }
    }
}