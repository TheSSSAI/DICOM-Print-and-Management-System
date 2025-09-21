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
    public sealed partial class AuditTrailViewModel : ViewModelBase
    {
        private readonly IAuditTrailService _auditTrailService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private string? _userFilter;

        [ObservableProperty]
        private DateTime? _startDateFilter = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime? _endDateFilter = DateTime.Today;

        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<AuditLogEntryDto> AuditLogs { get; } = new();

        public AuditTrailViewModel(IAuditTrailService auditTrailService, IDialogService dialogService)
        {
            _auditTrailService = auditTrailService;
            _dialogService = dialogService;
        }

        [AsyncRelayCommand]
        private async Task SearchAsync()
        {
            IsLoading = true;
            try
            {
                AuditLogs.Clear();
                var filter = new AuditLogFilter
                {
                    Username = UserFilter,
                    StartDate = _startDateFilter,
                    EndDate = _endDateFilter?.AddDays(1).AddTicks(-1) // Include the whole end day
                };
                var logs = await _auditTrailService.GetAuditLogsAsync(filter);
                foreach (var log in logs)
                {
                    AuditLogs.Add(log);
                }
            }
            catch (Exception ex)
            {
                // log ex
                await _dialogService.ShowMessageAsync("Error", "Failed to retrieve audit logs.");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [AsyncRelayCommand]
        private async Task ExportAsync()
        {
            // In a real app, this would use a file save dialog and an export service
            await _dialogService.ShowMessageAsync("Not Implemented", "CSV export functionality is not yet implemented.");
        }
        
        public async Task OnNavigatedToAsync()
        {
            await SearchAsync();
        }
    }
}