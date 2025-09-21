using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Interfaces;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System.Security;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels
{
    public sealed partial class LockScreenViewModel : ViewModelBase
    {
        private const int MaxUnlockAttempts = 5;

        private readonly IAuthenticationService _authenticationService;
        private readonly ISessionStateService _sessionStateService;

        [ObservableProperty]
        private string? _username;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
        private SecureString? _password;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        private int _failedAttempts;

        public LockScreenViewModel(
            IAuthenticationService authenticationService, 
            ISessionStateService sessionStateService)
        {
            _authenticationService = authenticationService;
            _sessionStateService = sessionStateService;
            Username = _sessionStateService.CurrentUser?.Username;
        }
        
        [AsyncRelayCommand(CanExecute = nameof(CanUnlock))]
        private async Task UnlockAsync()
        {
            IsBusy = true;
            ErrorMessage = null;

            try
            {
                var result = await _authenticationService.VerifyPasswordAsync(Password!);
                if (result)
                {
                    _sessionStateService.UnlockSession();
                }
                else
                {
                    HandleFailedAttempt();
                }
            }
            catch
            {
                HandleFailedAttempt();
                ErrorMessage = "An error occurred during verification.";
            }
            finally
            {
                Password?.Clear();
                IsBusy = false;
            }
        }

        private void HandleFailedAttempt()
        {
            _failedAttempts++;
            if (_failedAttempts >= MaxUnlockAttempts)
            {
                ErrorMessage = $"Maximum unlock attempts ({MaxUnlockAttempts}) exceeded. Logging out.";
                // Give user a moment to see the message before logout
                Task.Delay(2000).ContinueWith(_ => _sessionStateService.Logout());
            }
            else
            {
                ErrorMessage = $"Invalid password. Please try again. ({MaxUnlockAttempts - _failedAttempts} attempts remaining)";
            }
        }

        private bool CanUnlock()
        {
            return Password is not null && Password.Length > 0 && !IsBusy && _failedAttempts < MaxUnlockAttempts;
        }

        [RelayCommand]
        private void Logout()
        {
            _sessionStateService.Logout();
        }
    }
}