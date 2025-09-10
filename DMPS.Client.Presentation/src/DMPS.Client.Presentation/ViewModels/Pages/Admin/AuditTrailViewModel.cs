using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Shared.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Pages.Admin
{
    public sealed partial class AuditTrailViewModel : ViewModelBase
    {
        private readonly IAuditService _auditService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private DateTime _startDate = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        [ObservableProperty]
        private string? _userFilter;
        
        [ObservableProperty]
        private ObservableCollection<AuditLog> _auditLogs = new();

        [ObservableProperty]
        private bool _isBusy;
        
        public AuditTrailViewModel(IAuditService auditService, IDialogService dialogService)
        {
            _auditService = auditService;
            _dialogService = dialogService;
        }

        [RelayCommand]
        private async Task FilterAsync()
        {
            if (StartDate > EndDate)
            {
                await _dialogService.ShowMessageBoxAsync("Invalid Date Range", "The start date cannot be after the end date.");
                return;
            }
            
            IsBusy = true;
            try
            {
                var result = await _auditService.GetAuditLogsAsync(StartDate, EndDate, UserFilter);
                if (result.IsSuccess && result.Value is not null)
                {
                    AuditLogs = new ObservableCollection<AuditLog>(result.Value);
                }
                else
                {
                    AuditLogs.Clear();
                    await _dialogService.ShowMessageBoxAsync("Error", result.Error ?? "Failed to retrieve audit logs.");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync("Critical Error", $"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand]
        private async Task ExportAsync()
        {
            if (AuditLogs.Count == 0)
            {
                await _dialogService.ShowMessageBoxAsync("No Data", "There is no data to export.");
                return;
            }
            
            IsBusy = true;
            try
            {
                // In a real app, use a File Save Dialog
                var filePath = $"C:\\temp\\AuditLog_{DateTime.Now:yyyyMMddHHmmss}.csv";
                var result = await _auditService.ExportAuditLogsAsync(AuditLogs, filePath);
                
                if (result.IsSuccess)
                {
                    await _dialogService.ShowMessageBoxAsync("Export Successful", $"Audit log has been exported to:\n{filePath}");
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Export Failed", result.Error ?? "An error occurred during export.");
                }
            }
            catch (Exception ex)
            {
                 await _dialogService.ShowMessageBoxAsync("Critical Error", $"An unexpected error occurred during export: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}