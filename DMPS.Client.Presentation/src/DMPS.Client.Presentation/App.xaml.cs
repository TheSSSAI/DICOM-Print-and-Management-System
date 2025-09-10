using DMPS.Client.Application;
using DMPS.Client.Presentation.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels;
using DMPS.Client.Presentation.ViewModels.Controls;
using DMPS.Client.Presentation.ViewModels.Pages;
using DMPS.Client.Presentation.ViewModels.Pages.Admin;
using DMPS.Client.Presentation.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace DMPS.Client.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? _host;

        public static IHost Host => _host ?? throw new InvalidOperationException("Host has not been initialized.");

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = Host.CreateDefaultBuilder(e.Args);

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });

            builder.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: "Logs\\log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.EventLog(
                    source: "DMPS Client",
                    logName: "Application",
                    manageEventSource: true)
            );

            builder.ConfigureServices((context, services) =>
            {
                // Register Application Layer Services
                services.AddApplicationServices(context.Configuration);

                // Register Presentation Layer Services
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<INotificationService, NotificationService>();
                services.AddSingleton<ISessionLockService, SessionLockService>();
                // IRenderingService is transient as each viewer instance needs its own renderer
                // services.AddTransient<IRenderingService, VorticeRenderingService>();

                // Register ViewModels
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<LockScreenViewModel>();
                services.AddTransient<DicomViewerViewModel>();
                services.AddTransient<StudyBrowserViewModel>();
                services.AddTransient<PrintPreviewViewModel>();
                services.AddTransient<QueryRetrieveViewModel>();
                services.AddTransient<UserManagementViewModel>();
                services.AddTransient<AuditTrailViewModel>();
                services.AddTransient<SystemHealthViewModel>();

                // Register Views (as Singletons or Transients based on use case)
                services.AddSingleton<MainWindow>();
                services.AddTransient<LoginView>();
            });

            _host = builder.Build();

            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            base.OnExit(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "An unhandled exception occurred in the dispatcher thread.");
            ShowFatalErrorDialog(e.Exception);
            e.Handled = true;
            Shutdown(-1);
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Error(e.Exception, "An unobserved task exception occurred.");
            e.SetObserved(); // Prevents the process from terminating
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.ExceptionObject as Exception, "A fatal unhandled exception occurred.");
            if (e.IsTerminating)
            {
                ShowFatalErrorDialog(e.ExceptionObject as Exception);
            }
        }

        private void ShowFatalErrorDialog(Exception? ex)
        {
            var message = $"A critical, unrecoverable error has occurred and the application must close.\n\n" +
                          $"Error: {ex?.Message}\n\n" +
                          $"Please contact support and provide the log files located in the 'Logs' directory.";

            MessageBox.Show(message, "Fatal Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}