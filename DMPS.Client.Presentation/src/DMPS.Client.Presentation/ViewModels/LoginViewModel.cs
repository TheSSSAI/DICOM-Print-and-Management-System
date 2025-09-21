using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Interfaces;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System;
using System.Security;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels
{
    public sealed partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string? _username;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private SecureString? _password;
        
        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public LoginViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        [AsyncRelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync()
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                var result = await _authenticationService.LoginAsync(Username!, Password!);

                if (result.IsSuccess)
                {
                    if (result.IsTemporaryPassword)
                    {
                        // To be implemented: Navigate to a dedicated Change Password view
                        // _navigationService.NavigateTo<ChangePasswordViewModel>(); 
                        await _dialogService.ShowMessageAsync("Login Successful", "You are required to change your temporary password.");
                    }
                    else
                    {
                        _navigationService.NavigateTo<StudyBrowserViewModel>();
                    }
                }
                else
                {
                    ErrorMessage = result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                // In a real app, log this exception
                ErrorMessage = "An unexpected error occurred. Please try again later.";
                await _dialogService.ShowMessageAsync("Login Error", ErrorMessage);
            }
            finally
            {
                Password?.Clear();
                IsBusy = false;
            }
        }
        
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) 
                && Password is not null 
                && Password.Length > 0 
                && !IsBusy;
        }
    }
}