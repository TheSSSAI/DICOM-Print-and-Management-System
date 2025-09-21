using DMPS.Client.Application;
using DMPS.Client.Presentation.Services;
using DMPS.Client.Presentation.Services.Interfaces;
using DMPS.Client.Presentation.ViewModels;
using DMPS.Client.Presentation.ViewModels.Controls;
using DMPS.Client.Presentation.ViewModels.Pages;
using DMPS.Client.Presentation.ViewModels.Pages.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DMPS.Client.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public static IServiceProvider ServiceProvider { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var hostBuilder = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .ConfigureLogging((context, logging) =>
                {
                    // Configure logging providers here. For Serilog, this would be where .UseSerilog() is called.
                    // Example: logging.AddConsole();
                });

            _host = hostBuilder.Build();
            ServiceProvider = _host.Services;

            await _host.StartAsync();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            // Initialize and start services that need to run for the application's lifetime
            var sessionLockService = ServiceProvider.GetRequiredService<ISessionLockService>();
            var sessionLockTimeout = _host.Services.GetRequiredService<IConfiguration>()
                .GetValue<int>("ApplicationSettings:SessionLockTimeoutMinutes");
            sessionLockService.Start(TimeSpan.FromMinutes(sessionLockTimeout));

            base.OnStartup(e);
        }

        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            // Register Application Layer services
            // This extension method should be defined in the DMPS.Client.Application project
            services.AddApplicationServices(configuration);

            // Register Presentation Layer Services (UI-specific)
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ISessionLockService, SessionLockService>();

            // Register ViewModels
            // Main Window ViewModel should be a singleton as it represents the main application shell
            services.AddSingleton<MainWindowViewModel>();

            // Page/Feature ViewModels should be transient to ensure a clean state upon navigation
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LockScreenViewModel>();
            services.AddTransient<StudyBrowserViewModel>();
            services.AddTransient<PrintPreviewViewModel>();
            services.AddTransient<QueryRetrieveViewModel>();

            // Admin Page ViewModels
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<AuditTrailViewModel>();
            services.AddTransient<SystemHealthViewModel>();

            // Control ViewModels
            services.AddTransient<DicomViewerViewModel>();

            // Register Views (Windows)
            // The MainWindow is registered as a Singleton to ensure only one instance exists.
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            base.OnExit(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Prevent default WPF unhandled exception processing
            e.Handled = true;

            // Log the exception and show a friendly message
            var logger = ServiceProvider?.GetService<ILogger<App>>();
            var dialogService = ServiceProvider?.GetService<IDialogService>();

            logger?.LogCritical(e.Exception, "An unhandled exception occurred.");

            var errorMessage = $"An unexpected error occurred. The application may need to close. Please contact support and provide the following details:\n\nError: {e.Exception.Message}";
            
            // Using MainWindow directly if dialog service is not available yet
            if (dialogService != null)
            {
                dialogService.ShowMessageAsync("Critical Application Error", errorMessage);
            }
            else
            {
                MessageBox.Show(errorMessage, "Critical Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}