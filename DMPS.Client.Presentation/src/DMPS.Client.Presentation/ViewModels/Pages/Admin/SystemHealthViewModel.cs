using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Shared.Core.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DMPS.Client.Presentation.ViewModels.Pages.Admin
{
    public sealed partial class SystemHealthViewModel : ViewModelBase, IDisposable
    {
        private readonly ISystemHealthService _systemHealthService;
        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private SystemHealthReport? _healthReport;

        [ObservableProperty]
        private DateTime _lastUpdated;
        
        [ObservableProperty]
        private bool _isBusy;

        public SystemHealthViewModel(ISystemHealthService systemHealthService, IDialogService dialogService)
        {
            _systemHealthService = systemHealthService;
            _dialogService = dialogService;
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _timer.Tick += async (s, e) => await RefreshHealthAsync();
        }

        public void OnNavigatedTo()
        {
            _timer.Start();
            // Fire immediately on navigation
            Task.Run(RefreshHealthAsync);
        }

        public void OnNavigatedFrom()
        {
            _timer.Stop();
        }

        [RelayCommand]
        private async Task RefreshHealthAsync()
        {
            IsBusy = true;
            try
            {
                var result = await _systemHealthService.GetSystemHealthAsync();
                if (result.IsSuccess && result.Value is not null)
                {
                    HealthReport = result.Value;
                    LastUpdated = DateTime.Now;
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Error", result.Error ?? "Failed to retrieve system health status.");
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync("Critical Error", $"An error occurred while fetching system health: {ex.Message}");
                // Stop timer on critical failure to avoid spamming errors
                _timer.Stop();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Dispose()
        {
            _timer.Stop();
        }
    }
}