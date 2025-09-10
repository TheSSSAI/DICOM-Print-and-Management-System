using DMPS.Client.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace DMPS.Client.Application.Services
{
    /// <summary>
    /// Manages the automatic session lock based on user inactivity.
    /// This service should be registered as a singleton to ensure a single
    /// timer exists for the application's lifetime.
    /// </summary>
    public sealed class SessionLockService : ISessionLockService, IDisposable
    {
        private readonly ILogger<SessionLockService> _logger;
        private readonly IApplicationStateService _applicationStateService;
        private Timer? _inactivityTimer;
        private TimeSpan _inactivityTimeout;
        private bool _isStarted = false;
        private readonly object _lock = new();

        /// <inheritdoc />
        public event EventHandler? SessionLockTriggered;

        public SessionLockService(ILogger<SessionLockService> logger, IApplicationStateService applicationStateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService));

            _applicationStateService.CurrentUserChanged += OnCurrentUserChanged;
        }

        private void OnCurrentUserChanged(object? sender, Events.UserLoggedInEventArgs e)
        {
            if (e.User != null)
            {
                // A user has logged in, start monitoring if we have a timeout set
                if (_isStarted)
                {
                    _logger.LogInformation("User logged in. Resetting inactivity timer.");
                    ResetInactivityTimer();
                }
            }
            else
            {
                // User has logged out, stop monitoring
                _logger.LogInformation("User logged out. Stopping inactivity timer.");
                Stop();
            }
        }
        
        /// <inheritdoc />
        public void Start(TimeSpan inactivityTimeout)
        {
            lock (_lock)
            {
                if (_isStarted)
                {
                    _logger.LogWarning("SessionLockService is already started. Ignoring call to Start.");
                    return;
                }

                if (inactivityTimeout <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(inactivityTimeout), "Inactivity timeout must be a positive time span.");
                }

                _inactivityTimeout = inactivityTimeout;
                _logger.LogInformation("Starting session lock service with a timeout of {Timeout} minutes.", _inactivityTimeout.TotalMinutes);

                // Start the timer only if a user is already logged in
                if (_applicationStateService.CurrentUser != null)
                {
                    _inactivityTimer = new Timer(OnTimerElapsed, null, _inactivityTimeout, Timeout.InfiniteTimeSpan);
                }
                
                _isStarted = true;
            }
        }

        /// <inheritdoc />
        public void ResetInactivityTimer()
        {
            lock (_lock)
            {
                if (!_isStarted || _applicationStateService.CurrentUser == null)
                {
                    return; // Do nothing if not started or no user is logged in
                }

                _logger.LogTrace("User activity detected. Resetting inactivity timer.");
                _inactivityTimer?.Change(_inactivityTimeout, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// Stops the inactivity monitoring.
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                if (!_isStarted) return;

                _logger.LogInformation("Stopping session lock service.");
                _inactivityTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                _isStarted = false;
            }
        }

        private void OnTimerElapsed(object? state)
        {
            _logger.LogInformation("Inactivity timeout of {Timeout} minutes reached. Triggering session lock.", _inactivityTimeout.TotalMinutes);

            // Stop the timer to prevent it from firing again until reset.
            Stop();

            // Raise the event for the UI to handle
            SessionLockTriggered?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _logger.LogDebug("Disposing SessionLockService.");
            _applicationStateService.CurrentUserChanged -= OnCurrentUserChanged;
            _inactivityTimer?.Dispose();
        }
    }
}