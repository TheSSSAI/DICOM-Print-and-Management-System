using CommunityToolkit.Mvvm.ComponentModel;

namespace DMPS.Client.Presentation.ViewModels.Base
{
    /// <summary>
    /// An abstract base class for all ViewModels in the application.
    /// It inherits from ObservableObject to provide INotifyPropertyChanged implementation
    /// and serves as a common type for ViewModel-related logic and extension.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject
    {
        private bool _isBusy;
        private string _title = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is currently busy with a long-running operation.
        /// This property can be used to show loading indicators in the View.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        
        /// <summary>
        /// Gets or sets the title for the view associated with this ViewModel.
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// A virtual method that can be overridden in derived ViewModels to handle initialization logic.
        /// This is often called when the ViewModel is first loaded or navigated to.
        /// </summary>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}