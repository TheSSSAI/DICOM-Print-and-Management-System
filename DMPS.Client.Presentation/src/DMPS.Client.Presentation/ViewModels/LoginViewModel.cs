using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels
{
    public sealed partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string? _username;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public LoginViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            INotificationService notificationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _notificationService = notificationService;
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync(string? password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(Username))
            {
                ErrorMessage = "Username and password are required.";
                return;
            }

            IsBusy = true;
            ErrorMessage = null;

            try
            {
                var loginResult = await _authenticationService.LoginAsync(Username, password);
                if (loginResult.IsSuccess)
                {
                    // Navigate to the main application window
                    _navigationService.NavigateTo<MainWindowViewModel>();
                    _navigationService.Close<LoginViewModel>();
                }
                else
                {
                    ErrorMessage = loginResult.Error;
                }
            }
            catch (System.Exception ex)
            {
                // In a real app, log this exception
                ErrorMessage = "An unexpected error occurred. Please contact support.";
                _notificationService.ShowError("Login Failed", "Could not connect to the authentication service.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLogin(string? password)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(password) && !IsBusy;
        }
    }
}