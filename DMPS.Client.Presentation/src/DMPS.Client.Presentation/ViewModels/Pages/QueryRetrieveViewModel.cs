using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Shared.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Pages
{
    public sealed partial class QueryRetrieveViewModel : ViewModelBase
    {
        private readonly IPacsService _pacsService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<PacsConfiguration> _pacsNodes = new();
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(QueryCommand))]
        [NotifyCanExecuteChangedFor(nameof(RetrieveCommand))]
        private PacsConfiguration? _selectedPacsNode;
        
        [ObservableProperty]
        private string? _patientId;
        
        [ObservableProperty]
        private DateTime? _studyDate = DateTime.Today;
        
        [ObservableProperty]
        private ObservableCollection<PacsStudyResult> _queryResults = new();
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RetrieveCommand))]
        private PacsStudyResult? _selectedStudyResult;
        
        [ObservableProperty]
        private bool _isQuerying;

        [ObservableProperty]
        private bool _isRetrieving;
        
        public QueryRetrieveViewModel(
            IPacsService pacsService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _pacsService = pacsService;
            _dialogService = dialogService;
            _notificationService = notificationService;
        }

        [RelayCommand]
        private async Task LoadPacsNodesAsync()
        {
            try
            {
                var result = await _pacsService.GetPacsConfigurationsAsync();
                if (result.IsSuccess && result.Value is not null)
                {
                    PacsNodes = new ObservableCollection<PacsConfiguration>(result.Value);
                    SelectedPacsNode = PacsNodes.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync("Error", $"Failed to load PACS configurations: {ex.Message}");
            }
        }
        
        [RelayCommand(CanExecute = nameof(CanQuery))]
        private async Task QueryAsync()
        {
            IsQuerying = true;
            QueryResults.Clear();
            try
            {
                var query = new PacsQueryParameters
                {
                    PatientId = PatientId,
                    StudyDate = StudyDate
                };

                var result = await _pacsService.QueryStudiesAsync(SelectedPacsNode!.Id, query);
                if (result.IsSuccess && result.Value is not null)
                {
                    QueryResults = new ObservableCollection<PacsStudyResult>(result.Value);
                    if (!QueryResults.Any())
                    {
                        _notificationService.ShowInformation("No Results", "Your query returned no matching studies.");
                    }
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Query Failed", result.Error ?? "An unknown error occurred during the C-FIND operation.");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync("Query Error", $"A critical error occurred: {ex.Message}");
            }
            finally
            {
                IsQuerying = false;
            }
        }

        private bool CanQuery() => SelectedPacsNode is not null && !IsQuerying;

        [RelayCommand(CanExecute = nameof(CanRetrieve))]
        private async Task RetrieveAsync()
        {
            IsRetrieving = true;
            try
            {
                var result = await _pacsService.RetrieveStudyAsync(SelectedPacsNode!.Id, SelectedStudyResult!.StudyInstanceUid);
                if (result.IsSuccess)
                {
                    _notificationService.ShowSuccess("Retrieve Started", "The selected study is being retrieved to local storage. You will be notified on completion.");
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Retrieve Failed", result.Error ?? "An unknown error occurred during the C-MOVE operation.");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync("Retrieve Error", $"A critical error occurred: {ex.Message}");
            }
            finally
            {
                IsRetrieving = false;
            }
        }
        
        private bool CanRetrieve() => SelectedPacsNode is not null && SelectedStudyResult is not null && !IsRetrieving;
    }
}