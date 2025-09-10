using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels.Dialogs;
using DMPS.Client.Presentation.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DMPS.Client.Presentation.Services;

/// <summary>
/// A service for displaying dialogs in an MVVM-friendly way.
/// It uses the MaterialDesignInXamlToolkit's DialogHost for a consistent look and feel.
/// </summary>
public sealed class DialogService : IDialogService
{
    private const string MainDialogHostIdentifier = "MainDialogHost";
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
    
    /// <inheritdoc />
    public async Task<DialogResult> ShowMessageBoxAsync(string title, string message, DialogButton buttons)
    {
        return await Application.Current.Dispatcher.Invoke(async () =>
        {
            var viewModel = _serviceProvider.GetRequiredService<MessageBoxViewModel>();
            viewModel.Title = title;
            viewModel.Message = message;
            viewModel.ConfigureButtons(buttons);

            var view = new MessageBoxView
            {
                DataContext = viewModel
            };

            var result = await DialogHost.Show(view, MainDialogHostIdentifier);

            return result is DialogResult dialogResult ? dialogResult : DialogResult.None;
        });
    }

    /// <inheritdoc />
    public async Task<TResult?> ShowDialogAsync<TViewModel, TResult>(object? parameter = null) where TViewModel : IDialogViewModel<TResult>
    {
        return await Application.Current.Dispatcher.Invoke(async () =>
        {
            // This is a more advanced version that could be used for custom dialogs.
            // It requires a mapping from ViewModel to View, similar to NavigationService.
            // For now, this is a placeholder for future extension.
            // A full implementation would resolve the view by convention and show it.
            throw new NotImplementedException("Custom dialogs are not yet implemented in this version.");
        });
    }
}