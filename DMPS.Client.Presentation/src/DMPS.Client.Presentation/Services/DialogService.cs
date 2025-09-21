using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels;
using DMPS.Client.Presentation.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DMPS.Client.Presentation.Services
{
    /// <summary>
    /// A service for displaying modal dialogs from ViewModels without a direct dependency on the View layer.
    /// This implementation uses the Material Design in XAML Toolkit's DialogHost.
    /// </summary>
    public sealed class DialogService : IDialogService
    {
        private const string MainDialogHostIdentifier = "MainDialogHost";

        /// <inheritdoc />
        public async Task ShowMessageAsync(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            await ExecuteOnUIThread(async () =>
            {
                var view = new MessageDialogView
                {
                    DataContext = new MessageDialogViewModel
                    {
                        Title = title,
                        Message = message
                    }
                };

                await DialogHost.Show(view, MainDialogHostIdentifier);
            });
        }

        /// <inheritdoc />
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            var result = await ExecuteOnUIThread(async () =>
            {
                var viewModel = new ConfirmationDialogViewModel
                {
                    Title = title,
                    Message = message
                };

                var view = new ConfirmationDialogView
                {
                    DataContext = viewModel
                };

                await DialogHost.Show(view, MainDialogHostIdentifier);

                return viewModel.Result;
            });

            return result;
        }

        /// <inheritdoc />
        public async Task ShowErrorAsync(string title, string message, string details)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            await ExecuteOnUIThread(async () =>
            {
                var view = new ErrorDialogView
                {
                    DataContext = new ErrorDialogViewModel
                    {
                        Title = title,
                        Message = message,
                        Details = details
                    }
                };

                await DialogHost.Show(view, MainDialogHostIdentifier);
            });
        }

        /// <summary>
        /// Executes a function on the UI thread.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="func">The function to execute.</param>
        /// <returns>The result of the function.</returns>
        private static async Task<T> ExecuteOnUIThread<T>(Func<Task<T>> func)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                return await func();
            }
            return await Application.Current.Dispatcher.InvokeAsync(func).Task.Unwrap();
        }

        /// <summary>
        /// Executes an action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private static async Task ExecuteOnUIThread(Func<Task> action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                await action();
            }
            else
            {
                await Application.Current.Dispatcher.InvokeAsync(action).Task.Unwrap();
            }
        }
    }
}