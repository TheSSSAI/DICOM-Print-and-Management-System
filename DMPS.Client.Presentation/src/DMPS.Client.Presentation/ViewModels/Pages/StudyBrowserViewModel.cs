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
    public sealed partial class StudyBrowserViewModel : ViewModelBase
    {
        private readonly IStudyQueryService _studyQueryService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private string? _patientNameFilter;

        [ObservableProperty]
        private DateTime? _studyDateFilter;

        [ObservableProperty]
        private string? _modalityFilter;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private StudySummaryDto? _selectedStudy;
        
        public ObservableCollection<StudySummaryDto> Studies { get; } = new();

        public StudyBrowserViewModel(
            IStudyQueryService studyQueryService, 
            INavigationService navigationService, 
            IDialogService dialogService)
        {
            _studyQueryService = studyQueryService;
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        [AsyncRelayCommand]
        private async Task LoadStudiesAsync()
        {
            IsLoading = true;
            try
            {
                Studies.Clear();
                var queryCriteria = new StudyQueryCriteria
                {
                    PatientName = PatientNameFilter,
                    StudyDate = StudyDateFilter,
                    Modality = ModalityFilter
                };
                
                var studies = await _studyQueryService.GetLocalStudiesAsync(queryCriteria);
                foreach (var study in studies)
                {
                    Studies.Add(study);
                }
            }
            catch (Exception ex)
            {
                // Log exception
                await _dialogService.ShowMessageAsync("Error", "Failed to load studies.");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private void ClearFilters()
        {
            PatientNameFilter = null;
            StudyDateFilter = null;
            ModalityFilter = null;
            _ = LoadStudiesAsync();
        }

        [RelayCommand]
        private void OpenStudy(StudySummaryDto? study)
        {
            var studyToOpen = study ?? SelectedStudy;
            if (studyToOpen is not null)
            {
                // This would navigate to a more complex viewer page that hosts the DicomViewerViewModel
                //_navigationService.NavigateTo<SeriesViewerViewModel>(studyToOpen.StudyInstanceUid);
            }
        }
        
        // This method would be called by the view when the ViewModel is activated
        public async Task OnNavigatedToAsync()
        {
            await LoadStudiesAsync();
        }
    }
}