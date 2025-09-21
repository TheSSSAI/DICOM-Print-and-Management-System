// DMPS.Client.Presentation/ViewModels/Base/ViewModelBase.cs

using CommunityToolkit.Mvvm.ComponentModel;

namespace DMPS.Client.Presentation.ViewModels.Base
{
    /// <summary>
    /// A base class for all ViewModels in the application.
    /// It inherits from ObservableObject from the CommunityToolkit.Mvvm library,
    /// which provides a modern and efficient implementation of INotifyPropertyChanged.
    /// This ensures that all ViewModels can participate in WPF data binding.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject
    {
        // This class is intentionally left blank. 
        // Its purpose is to serve as a common base type for all ViewModels 
        // and to provide the property change notification functionality from its own base class, ObservableObject.
        // Future cross-cutting ViewModel concerns (e.g., IsBusy property, Load/Unload methods) could be added here.
    }
}