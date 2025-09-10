using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Services;
using DMPS.Client.Presentation.ViewModels.Base;
using DMPS.Shared.Core.Models;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Controls
{
    public sealed partial class DicomViewerViewModel : ViewModelBase
    {
        private readonly IDicomProcessingService _dicomProcessingService;
        
        [ObservableProperty]
        private DicomImage? _currentImage;

        [ObservableProperty]
        private double _zoomFactor = 1.0;
        
        [ObservableProperty]
        private double _panX = 0;

        [ObservableProperty]
        private double _panY = 0;

        [ObservableProperty]
        private int _windowWidth = 400;

        [ObservableProperty]
        private int _windowCenter = 40;
        
        public DicomViewerViewModel(IDicomProcessingService dicomProcessingService)
        {
            _dicomProcessingService = dicomProcessingService;
        }

        public async Task LoadImageAsync(DicomImage image)
        {
            CurrentImage = image;
            // In a real application, this might involve more complex logic,
            // like fetching pixel data or applying default presets.
            await ApplyDefaultPresetAsync();
        }

        [RelayCommand]
        private void ZoomIn() => ZoomFactor *= 1.1;

        [RelayCommand]
        private void ZoomOut() => ZoomFactor /= 1.1;
        
        [RelayCommand]
        private void ResetZoomAndPan()
        {
            ZoomFactor = 1.0;
            PanX = 0;
            PanY = 0;
        }

        [RelayCommand]
        private async Task ApplyDefaultPresetAsync()
        {
            if (CurrentImage is null) return;

            var preset = await _dicomProcessingService.GetDefaultWindowLevelAsync(CurrentImage.Modality);
            WindowWidth = preset.Width;
            WindowCenter = preset.Center;
        }
        
        [RelayCommand]
        private void ApplyPreset(WindowLevelPreset preset)
        {
            WindowWidth = preset.Width;
            WindowCenter = preset.Center;
        }

        // Methods to be called from the View's code-behind for mouse interactions
        public void Pan(double deltaX, double deltaY)
        {
            PanX += deltaX / ZoomFactor;
            PanY += deltaY / ZoomFactor;
        }

        public void Update WindowLevel(double deltaX, double deltaY)
        {
            // A common mapping: horizontal movement adjusts width, vertical adjusts center
            WindowWidth += (int)deltaX;
            WindowCenter += (int)deltaY;
        }
    }
}