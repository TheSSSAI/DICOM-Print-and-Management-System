using DMPS.Client.Presentation.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace DMPS.Client.Presentation.Services
{
    /// <summary>
    /// Manages the user inactivity timer and controls the locked state of the UI.
    /// This service uses P/Invoke to query the system for the last user input time.
    /// </summary>
    public sealed class SessionLockService : ISessionLockService, IDisposable
    {
        private readonly ILogger<SessionLockService> _logger;
        private DispatcherTimer? _inactivityTimer;
        private TimeSpan _inactivityTimeout;

        /// <inheritdoc />
        public event EventHandler? SessionLockTriggered;

        public SessionLockService(ILogger<SessionLockService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public void Start(TimeSpan inactivityTimeout)
        {
            if (inactivityTimeout <= TimeSpan.Zero)
            {
                _logger.LogWarning("Session lock timeout is zero or negative. Session locking will be disabled.");
                return;
            }

            _inactivityTimeout = inactivityTimeout;

            if (_inactivityTimer is not null && _inactivityTimer.IsEnabled)
            {
                _inactivityTimer.Stop();
            }

            _inactivityTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(1), // Check every second
                DispatcherPriority.ApplicationIdle,
                OnTimerTick,
                Dispatcher.CurrentDispatcher);
            
            _logger.LogInformation("Session lock service started with a timeout of {TimeoutMinutes} minutes.", _inactivityTimeout.TotalMinutes);
            _inactivityTimer.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (_inactivityTimer is not null)
            {
                _inactivityTimer.Stop();
                _inactivityTimer.Tick -= OnTimerTick;
                _inactivityTimer = null;
                _logger.LogInformation("Session lock service stopped.");
            }
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            try
            {
                var idleTime = GetIdleTime();
                if (idleTime >= _inactivityTimeout)
                {
                    Stop();
                    _logger.LogInformation("User inactivity detected. Triggering session lock.");
                    SessionLockTriggered?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the session lock timer tick.");
                Stop(); // Stop the timer to prevent repeated errors.
            }
        }

        private static TimeSpan GetIdleTime()
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            if (!GetLastInputInfo(ref lastInputInfo))
            {
                // Could throw a Win32Exception but for this purpose, we can just return zero.
                return TimeSpan.Zero;
            }

            uint ticks = (uint)Environment.TickCount;
            uint idleTime = ticks - lastInputInfo.dwTime;

            return TimeSpan.FromMilliseconds(idleTime);
        }

        #region P/Invoke
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        #endregion

        public void Dispose()
        {
            Stop();
        }
    }
}