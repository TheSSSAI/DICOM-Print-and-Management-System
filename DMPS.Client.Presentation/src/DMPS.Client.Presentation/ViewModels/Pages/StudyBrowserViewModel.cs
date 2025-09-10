using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Shared.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace DMPS.Client.Presentation.ViewModels.Pages
{
    public sealed partial class StudyBrowserViewModel : ViewModelBase
    {
        private readonly IStudyService _studyService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string? _searchQuery;

        [ObservableProperty]
        private ObservableCollection<Study> _studies = new();

        [ObservableProperty]
        private Study? _selectedStudy;

        [ObservableProperty]
        private bool _isBusy;

        public StudyBrowserViewModel(
            IStudyService studyService,
            INavigationService navigationService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _studyService = studyService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _notificationService = notificationService;
        }

        [RelayCommand]
        private async Task LoadStudiesAsync()
        {
            IsBusy = true;
            try
            {
                var studiesResult = await _studyService.GetRecentStudiesAsync(100);
                if (studiesResult.IsSuccess && studiesResult.Value is not null)
                {
                    Studies = new ObservableCollection<Study>(studiesResult.Value);
                }
                else
                {
                    await _dialogService.ShowMessageBoxAsync("Error", "Failed to load recent studies.");
                }
            }
            catch (Exception ex)
            {
                // Log exception
                await _dialogService.ShowMessageBoxAsync("Load Error", $"An error occurred while loading studies: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await LoadStudiesAsync();
                return;
            }

            IsBusy = true;
            try
            {
                var searchResult = await _studyService.SearchStudiesAsync(SearchQuery);
                if (searchResult.IsSuccess && searchResult.Value is not null)
                {
                    Studies = new ObservableCollection<Study>(searchResult.Value);
                }
                else
                {
                    Studies.Clear();
                    _notificationService.ShowInformation("No Results", "No studies found matching your search criteria.");
                }
            }
            catch (Exception ex)
            {
                // Log exception
                await _dialogService.ShowMessageBoxAsync("Search Error", $"An error occurred during search: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanOpenStudy))]
        private void OpenStudy()
        {
            if (SelectedStudy is not null)
            {
                // Navigate to a detailed viewer view, passing the study instance UID
                // _navigationService.NavigateTo<DicomViewerPageViewModel>(SelectedStudy.StudyInstanceUid);
                _notificationService.ShowSuccess("Opening Study", $"Opening study for patient: {SelectedStudy.PatientName}");
            }
        }
        
        private bool CanOpenStudy() => SelectedStudy is not null && !IsBusy;
        
        [RelayCommand(CanExecute = nameof(CanDeleteStudy))]
        private async Task DeleteStudyAsync()
        {
            if (SelectedStudy is null) return;

            var result = await _dialogService.ShowMessageBoxAsync(
                "Confirm Delete", 
                $"Are you sure you want to permanently delete the study for '{SelectedStudy.PatientName}' dated '{SelectedStudy.StudyDate:d}'?", 
                DialogButton.YesNo);
            
            if (result == DialogResult.Yes)
            {
                IsBusy = true;
                try
                {
                    var deleteResult = await _studyService.DeleteStudyAsync(SelectedStudy.StudyInstanceUid);
                    if (deleteResult.IsSuccess)
                    {
                        Studies.Remove(SelectedStudy);
                        SelectedStudy = null;
                        _notificationService.ShowSuccess("Success", "Study has been deleted.");
                    }
                    else
                    {
                        await _dialogService.ShowMessageBoxAsync("Error", deleteResult.Error ?? "Failed to delete the study.");
                    }
                }
                catch (Exception ex)
                {
                    // Log ex
                    await _dialogService.ShowMessageBoxAsync("Error", $"An unexpected error occurred: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanDeleteStudy() => SelectedStudy is not null && !IsBusy;
    }
}