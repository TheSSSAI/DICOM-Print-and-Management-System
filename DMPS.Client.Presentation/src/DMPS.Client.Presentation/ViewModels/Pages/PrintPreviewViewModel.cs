using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Shared.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace DMPS.Client.Presentation.ViewModels.Pages
{
    public sealed partial class PrintPreviewViewModel : ViewModelBase
    {
        private readonly IPrintJobService _printJobService;
        private readonly INotificationService _notificationService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private Study? _currentStudy;

        [ObservableProperty]
        private ObservableCollection<string> _availablePrinters = new();

        [ObservableProperty]
        private string? _selectedPrinter;
        
        [ObservableProperty]
        private ObservableCollection<string> _availableLayouts = new();

        [ObservableProperty]
        private string? _selectedLayout;

        [ObservableProperty]
        private bool _isBusy;

        public PrintPreviewViewModel(
            IPrintJobService printJobService,
            INotificationService notificationService,
            IDialogService dialogService)
        {
            _printJobService = printJobService;
            _notificationService = notificationService;
            _dialogService = dialogService;

            // These would typically be loaded from configuration
            AvailableLayouts.Add("Single image on A4");
            AvailableLayouts.Add("2x2 grid on A4");
            AvailableLayouts.Add("1+3 comparison on A3");
            SelectedLayout = AvailableLayouts.FirstOrDefault();
        }

        public async Task InitializeAsync(Study study)
        {
            CurrentStudy = study;
            await LoadPrintersAsync();
        }

        private async Task LoadPrintersAsync()
        {
            IsBusy = true;
            try
            {
                var printersResult = await _printJobService.GetAvailablePrintersAsync();
                if (printersResult.IsSuccess && printersResult.Value is not null)
                {
                    AvailablePrinters = new ObservableCollection<string>(printersResult.Value);
                    SelectedPrinter = AvailablePrinters.FirstOrDefault();
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Printer Error", "Could not retrieve list of available printers.");
                }
            }
            catch (Exception ex)
            {
                // Log ex
                await _dialogService.ShowMessageBoxAsync("Error", $"Failed to load printers: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand(CanExecute = nameof(CanPrint))]
        private async Task PrintAsync()
        {
            IsBusy = true;
            try
            {
                var printJob = new PrintJobCreationInfo
                {
                    StudyInstanceUid = CurrentStudy!.StudyInstanceUid,
                    PrinterName = SelectedPrinter!,
                    LayoutName = SelectedLayout!,
                    // Add other properties like image UIDs, overlays, etc.
                };

                var result = await _printJobService.SubmitPrintJobAsync(printJob);

                if (result.IsSuccess)
                {
                    _notificationService.ShowSuccess("Print Job Submitted", "Your print job has been successfully queued for processing.");
                    // Optionally navigate away or reset the view
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Print Error", result.Error ?? "Failed to submit the print job.");
                }
            }
            catch (Exception ex)
            {
                // Log ex
                await _dialogService.ShowMessageBoxAsync("Critical Error", $"An unexpected error occurred while submitting the print job: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanPrint() => CurrentStudy is not null && !string.IsNullOrEmpty(SelectedPrinter) && !string.IsNullOrEmpty(SelectedLayout) && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanExportPdf))]
        private async Task ExportPdfAsync()
        {
            // In a real app, this would use a file save dialog
            string outputPath = $"C:\\temp\\{CurrentStudy?.PatientName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            IsBusy = true;
            try
            {
                var pdfJob = new PdfExportCreationInfo
                {
                    StudyInstanceUid = CurrentStudy!.StudyInstanceUid,
                    LayoutName = SelectedLayout!,
                    OutputPath = outputPath
                };

                var result = await _printJobService.SubmitPdfExportJobAsync(pdfJob);

                if (result.IsSuccess)
                {
                    _notificationService.ShowSuccess("PDF Export Started", $"The PDF will be saved to: {outputPath}");
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("PDF Export Error", result.Error ?? "Failed to submit the PDF export job.");
                }
            }
            catch (Exception ex)
            {
                // Log ex
                await _dialogService.ShowMessageBoxAsync("Critical Error", $"An unexpected error occurred during PDF export: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanExportPdf() => CurrentStudy is not null && !string.IsNullOrEmpty(SelectedLayout) && !IsBusy;
    }
}