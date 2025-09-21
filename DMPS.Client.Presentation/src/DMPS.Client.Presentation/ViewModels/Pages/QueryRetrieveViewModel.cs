using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.DTO;
using DMPS.Client.Application.Interfaces;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Pages
{
    public sealed partial class QueryRetrieveViewModel : ViewModelBase
    {
        private readonly IDicomScuService _dicomScuService;
        private readonly IPacsConfigurationService _pacsConfigService;
        private readonly INotificationService _notificationService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<PacsConfigurationDto> PacsConfigurations { get; } = new();
        public ObservableCollection<DicomQueryResultDto> QueryResults { get; } = new();

        [ObservableProperty]
        private PacsConfigurationDto? _selectedPacs;

        [ObservableProperty]
        private string? _patientIdFilter;

        [ObservableProperty]
        private DateTime? _studyDateFilter;

        [ObservableProperty]
        private string? _modalityFilter;

        [ObservableProperty]
        private bool _isQuerying;

        [ObservableProperty]
        private bool _isRetrieving;

        public QueryRetrieveViewModel(
            IDicomScuService dicomScuService,
            IPacsConfigurationService pacsConfigService,
            INotificationService notificationService,
            IDialogService dialogService)
        {
            _dicomScuService = dicomScuService;
            _pacsConfigService = pacsConfigService;
            _notificationService = notificationService;
            _dialogService = dialogService;
        }

        [AsyncRelayCommand]
        private async Task LoadPacsConfigurationsAsync()
        {
            try
            {
                PacsConfigurations.Clear();
                var configs = await _pacsConfigService.GetPacsConfigurationsAsync();
                foreach (var config in configs)
                {
                    PacsConfigurations.Add(config);
                }
                SelectedPacs = PacsConfigurations.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // log
                await _dialogService.ShowMessageAsync("Error", "Could not load PACS configurations.");
            }
        }

        [AsyncRelayCommand(CanExecute = nameof(CanQuery))]
        private async Task QueryAsync()
        {
            IsQuerying = true;
            QueryResults.Clear();
            try
            {
                var criteria = new DicomQueryCriteria
                {
                    PatientId = PatientIdFilter,
                    StudyDate = StudyDateFilter,
                    Modality = ModalityFilter
                };

                var results = await _dicomScuService.QueryStudiesAsync(SelectedPacs!, criteria);
                foreach (var result in results)
                {
                    QueryResults.Add(result);
                }
                _notificationService.Show("Query Complete", $"{QueryResults.Count} studies found.");
            }
            catch (Exception ex)
            {
                // log
                await _dialogService.ShowMessageAsync("Query Failed", $"An error occurred while querying PACS: {ex.Message}");
            }
            finally
            {
                IsQuerying = false;
            }
        }

        private bool CanQuery()
        {
            return SelectedPacs != null && !IsQuerying && !IsRetrieving;
        }

        [AsyncRelayCommand(CanExecute = nameof(CanRetrieve))]
        private async Task RetrieveAsync(DicomQueryResultDto resultToRetrieve)
        {
            IsRetrieving = true;
            try
            {
                _notificationService.Show("Retrieve Started", $"Retrieving study {resultToRetrieve.StudyInstanceUid}...");
                var result = await _dicomScuService.MoveStudyAsync(SelectedPacs!, resultToRetrieve.StudyInstanceUid);

                if (result.IsSuccess)
                {
                    _notificationService.Show("Retrieve Complete", $"Study {resultToRetrieve.StudyInstanceUid} retrieved successfully.");
                }
                else
                {
                    await _dialogService.ShowMessageAsync("Retrieve Failed", $"Failed to retrieve study: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                // log
                await _dialogService.ShowMessageAsync("Retrieve Error", $"An unexpected error occurred during retrieve: {ex.Message}");
            }
            finally
            {
                IsRetrieving = false;
            }
        }
        
        private bool CanRetrieve(DicomQueryResultDto? result)
        {
            return result != null && SelectedPacs != null && !IsQuerying && !IsRetrieving;
        }

        public async Task OnNavigatedToAsync()
        {
            await LoadPacsConfigurationsAsync();
        }
    }
}