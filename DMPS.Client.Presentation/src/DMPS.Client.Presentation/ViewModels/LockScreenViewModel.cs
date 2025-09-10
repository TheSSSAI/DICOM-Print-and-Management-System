using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels
{
    public sealed partial class LockScreenViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISessionLockService _sessionLockService;
        private readonly ISessionStateService _sessionStateService;

        private int _failedAttemptCount = 0;
        private const int MaxFailedAttempts = 5;

        [ObservableProperty]
        private string? _username;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public LockScreenViewModel(
            IAuthenticationService authenticationService,
            ISessionLockService sessionLockService,
            ISessionStateService sessionStateService)
        {
            _authenticationService = authenticationService;
            _sessionLockService = sessionLockService;
            _sessionStateService = sessionStateService;

            Username = _sessionStateService.CurrentUser?.Username ?? "Unknown User";
        }

        [RelayCommand(CanExecute = nameof(CanUnlock))]
        private async Task UnlockAsync(string? password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Password is required.";
                return;
            }
            
            IsBusy = true;
            ErrorMessage = null;

            try
            {
                var unlockResult = await _authenticationService.UnlockSessionAsync(password);
                if (unlockResult)
                {
                    _sessionLockService.Unlock();
                }
                else
                {
                    HandleFailedAttempt();
                }
            }
            catch (System.Exception)
            {
                // Log exception
                ErrorMessage = "An error occurred during unlock. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        private bool CanUnlock(string? password)
        {
            return !string.IsNullOrWhiteSpace(password) && !IsBusy;
        }

        private void HandleFailedAttempt()
        {
            _failedAttemptCount++;
            if (_failedAttemptCount >= MaxFailedAttempts)
            {
                ErrorMessage = $"Too many failed attempts. Logging out.";
                // Force logout after a brief delay to show message
                Task.Delay(1500).ContinueWith(_ => LogoutAsync());
            }
            else
            {
                ErrorMessage = $"Invalid password. Please try again. ({MaxFailedAttempts - _failedAttemptCount} attempts remaining)";
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _authenticationService.LogoutAsync();
            _sessionLockService.ForceLogout();
        }
    }
}