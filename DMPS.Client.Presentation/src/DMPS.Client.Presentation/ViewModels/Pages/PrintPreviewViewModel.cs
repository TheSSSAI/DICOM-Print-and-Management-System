using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.DTO;
using DMPS.Client.Application.Interfaces;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Pages
{
    public sealed partial class PrintPreviewViewModel : ViewModelBase
    {
        private readonly IPrintJobService _printJobService;
        private readonly ISystemStatusService _systemStatusService;
        private readonly INotificationService _notificationService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<string> LayoutTemplates { get; } = new();
        public ObservableCollection<string> Printers { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitPrintJobCommand))]
        private bool _isPrintServiceAvailable;

        [ObservableProperty]
        private string? _selectedLayout;

        [ObservableProperty]
        private string? _selectedPrinter;
        
        [ObservableProperty]
        private bool _isBusy;

        // In a real implementation, this would be a more complex object representing the images and annotations.
        public object? PrintContent { get; set; }

        public PrintPreviewViewModel(
            IPrintJobService printJobService,
            ISystemStatusService systemStatusService,
            INotificationService notificationService,
            IDialogService dialogService)
        {
            _printJobService = printJobService;
            _systemStatusService = systemStatusService;
            _notificationService = notificationService;
            _dialogService = dialogService;

            LoadLayouts();
        }

        private void LoadLayouts()
        {
            LayoutTemplates.Add("Single image on A4");
            LayoutTemplates.Add("2x2 grid on A4");
            LayoutTemplates.Add("1+3 comparison on A3");
            SelectedLayout = LayoutTemplates[0];
        }

        [AsyncRelayCommand]
        private async Task LoadPrintersAndCheckStatusAsync()
        {
            IsBusy = true;
            try
            {
                IsPrintServiceAvailable = await _systemStatusService.IsBackgroundServiceRunningAsync();

                if (IsPrintServiceAvailable)
                {
                    Printers.Clear();
                    var printers = await _printJobService.GetAvailablePrintersAsync();
                    foreach (var printer in printers)
                    {
                        Printers.Add(printer);
                    }
                    if (Printers.Count > 0)
                        SelectedPrinter = Printers[0];
                }
                else
                {
                    Printers.Clear();
                    await _dialogService.ShowMessageAsync("Print Service Unavailable", "The background print service is not running. Printing is disabled.");
                }
            }
            catch (Exception ex)
            {
                // Log exception
                await _dialogService.ShowMessageAsync("Error", "Failed to retrieve printer list.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [AsyncRelayCommand(CanExecute = nameof(CanSubmitPrintJob))]
        private async Task SubmitPrintJobAsync()
        {
            IsBusy = true;
            try
            {
                var printJobData = new PrintJobData
                {
                    PrinterName = SelectedPrinter,
                    LayoutTemplate = SelectedLayout,
                    // Map other properties from the viewmodel to the DTO
                };

                await _printJobService.SubmitPrintJobAsync(printJobData);
                _notificationService.Show("Success", "Print job has been queued successfully.");
            }
            catch (Exception ex)
            {
                // Log exception
                await _dialogService.ShowMessageAsync("Error", "Failed to submit print job.");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        private bool CanSubmitPrintJob()
        {
            return IsPrintServiceAvailable && !IsBusy && !string.IsNullOrEmpty(SelectedPrinter) && !string.IsNullOrEmpty(SelectedLayout);
        }

        public async Task OnNavigatedToAsync()
        {
            await LoadPrintersAndCheckStatusAsync();
        }
    }
}