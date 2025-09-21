using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.DTO;
using DMPS.Client.Application.Interfaces;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DMPS.Client.Presentation.ViewModels.Pages.Admin
{
    public sealed partial class SystemHealthViewModel : ViewModelBase, IDisposable
    {
        private readonly ISystemHealthService _systemHealthService;
        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _refreshTimer;

        [ObservableProperty]
        private SystemHealthStatusDto? _healthStatus;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private DateTime _lastUpdated;

        public SystemHealthViewModel(ISystemHealthService systemHealthService, IDialogService dialogService)
        {
            _systemHealthService = systemHealthService;
            _dialogService = dialogService;
            
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _refreshTimer.Tick += async (s, e) => await RefreshHealthStatusCommand.ExecuteAsync(null);
        }

        [AsyncRelayCommand]
        private async Task RefreshHealthStatusAsync()
        {
            IsLoading = true;
            try
            {
                HealthStatus = await _systemHealthService.GetSystemHealthStatusAsync();
                LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                // log ex
                _refreshTimer.Stop();
                await _dialogService.ShowMessageAsync("Error", "Failed to retrieve system health status. Auto-refresh disabled.");
                HealthStatus = new SystemHealthStatusDto { IsHealthy = false, ServiceStatus = "Error" };
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task OnNavigatedToAsync()
        {
            await RefreshHealthStatusAsync();
            if (!_refreshTimer.IsEnabled)
            {
                _refreshTimer.Start();
            }
        }
        
        public void OnNavigatedFrom()
        {
            _refreshTimer.Stop();
        }

        public void Dispose()
        {
            _refreshTimer.Stop();
            GC.SuppressFinalize(this);
        }
    }
}