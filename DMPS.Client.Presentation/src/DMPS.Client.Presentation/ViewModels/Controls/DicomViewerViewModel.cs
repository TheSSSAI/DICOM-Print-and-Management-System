using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMPS.Client.Application.Interfaces;
using DMPS.Core.Business.Entities;
using DMPS.Core.Business.ValueObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DMPS.Client.Presentation.ViewModels.Controls
{
    public sealed partial class DicomViewerViewModel : ObservableObject
    {
        private readonly IDicomImageService _dicomImageService;

        [ObservableProperty]
        private DicomImage? _currentImage;

        [ObservableProperty]
        private double _zoom = 1.0;

        [ObservableProperty]
        private double _panX;

        [ObservableProperty]
        private double _panY;

        [ObservableProperty]
        private double _windowWidth;

        [ObservableProperty]
        private double _windowCenter;

        [ObservableProperty]
        private bool _isBusy;

        public ObservableCollection<WindowLevelPreset> Presets { get; } = new();

        public DicomViewerViewModel(IDicomImageService dicomImageService)
        {
            _dicomImageService = dicomImageService;
            // In a real app, presets would be loaded from a service
            Presets.Add(new WindowLevelPreset("Default", 2000, 0));
            Presets.Add(new WindowLevelPreset("Lung", 1500, -600));
            Presets.Add(new WindowLevelPreset("Bone", 2500, 480));
            Presets.Add(new WindowLevelPreset("Brain", 80, 40));
        }

        public async Task LoadImageAsync(string imageSopInstanceUid)
        {
            IsBusy = true;
            try
            {
                var image = await _dicomImageService.GetDicomImageAsync(imageSopInstanceUid);
                CurrentImage = image;
                if (image != null)
                {
                    WindowWidth = image.DefaultWindowWidth;
                    WindowCenter = image.DefaultWindowCenter;
                    ResetView();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ResetView()
        {
            Zoom = 1.0;
            PanX = 0;
            PanY = 0;
            if (CurrentImage != null)
            {
                WindowWidth = CurrentImage.DefaultWindowWidth;
                WindowCenter = CurrentImage.DefaultWindowCenter;
            }
        }

        [RelayCommand]
        private void ApplyPreset(WindowLevelPreset preset)
        {
            if (preset != null)
            {
                WindowWidth = preset.Width;
                WindowCenter = preset.Center;
            }
        }

        [RelayCommand]
        private void Rotate(int degrees)
        {
            // Logic to update rotation property would go here
        }
        
        [RelayCommand]
        private void Flip(string direction)
        {
            // Logic to update flip property (horizontal/vertical) would go here
        }
    }
}