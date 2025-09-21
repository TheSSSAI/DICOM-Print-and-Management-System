using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.DTO;
using DMPS.Client.Application.Interfaces;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Pages.Admin
{
    public sealed partial class UserManagementViewModel : ViewModelBase
    {
        private readonly IAdminService _adminService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<UserDto> Users { get; } = new();

        public UserManagementViewModel(
            IAdminService adminService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _adminService = adminService;
            _dialogService = dialogService;
            _notificationService = notificationService;
        }

        [AsyncRelayCommand]
        private async Task LoadUsersAsync()
        {
            IsLoading = true;
            try
            {
                Users.Clear();
                var users = await _adminService.GetAllUsersAsync();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                // log ex
                await _dialogService.ShowMessageAsync("Error", "Failed to load users.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [AsyncRelayCommand]
        private async Task AddUserAsync()
        {
            // In a real app, this would open a dialog to get new user details
            // For now, we simulate it
            var newUser = new CreateUserDto { Username = $"newuser{DateTime.Now:mmss}", Role = "Technician" };
            
            var result = await _adminService.CreateUserAsync(newUser);
            if (result.IsSuccess)
            {
                await _dialogService.ShowMessageAsync("User Created", $"User '{newUser.Username}' created. Temporary Password: {result.TemporaryPassword}");
                await LoadUsersAsync();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Error", result.ErrorMessage ?? "Failed to create user.");
            }
        }

        [AsyncRelayCommand(CanExecute = nameof(CanModifyUser))]
        private async Task ResetPasswordAsync(UserDto user)
        {
            var confirmed = await _dialogService.ShowConfirmationAsync("Reset Password", $"Are you sure you want to reset the password for {user.Username}?");
            if (!confirmed) return;

            var result = await _adminService.ResetUserPasswordAsync(user.UserId);
            if (result.IsSuccess)
            {
                await _dialogService.ShowMessageAsync("Password Reset", $"Password for '{user.Username}' has been reset. Temporary Password: {result.TemporaryPassword}");
            }
            else
            {
                await _dialogService.ShowMessageAsync("Error", result.ErrorMessage ?? "Failed to reset password.");
            }
        }

        [AsyncRelayCommand(CanExecute = nameof(CanModifyUser))]
        private async Task DeleteUserAsync(UserDto user)
        {
            var confirmed = await _dialogService.ShowConfirmationAsync("Delete User", $"Are you sure you want to permanently delete user {user.Username}?");
            if (!confirmed) return;
            
            var result = await _adminService.DeleteUserAsync(user.UserId);
            if (result.IsSuccess)
            {
                _notificationService.Show("Success", $"User {user.Username} deleted.");
                Users.Remove(user);
            }
            else
            {
                await _dialogService.ShowMessageAsync("Error", result.ErrorMessage ?? "Failed to delete user.");
            }
        }

        private bool CanModifyUser(UserDto? user)
        {
            return user != null;
        }

        public async Task OnNavigatedToAsync()
        {
            await LoadUsersAsync();
        }
    }
}