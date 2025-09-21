using DMPS.Client.Application.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace DMPS.Client.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISessionLockService _sessionLockService;

        public MainWindow(object dataContext, ISessionLockService sessionLockService)
        {
            InitializeComponent();
            DataContext = dataContext;
            _sessionLockService = sessionLockService;

            // Hook into events to detect user activity for the session lock timer.
            PreviewMouseMove += MainWindow_ActivityDetected;
            PreviewKeyDown += MainWindow_ActivityDetected;
            PreviewMouseDown += MainWindow_ActivityDetected;
        }

        private void MainWindow_ActivityDetected(object sender, InputEventArgs e)
        {
            // Reset the inactivity timer on any detected user input.
            _sessionLockService?.ResetTimer();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unhook events to prevent memory leaks when the window is closed.
            PreviewMouseMove -= MainWindow_ActivityDetected;
            PreviewKeyDown -= MainWindow_ActivityDetected;
            PreviewMouseDown -= MainWindow_ActivityDetected;
            base.OnClosed(e);
        }
    }
}