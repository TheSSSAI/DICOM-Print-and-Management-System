using DMPS.Client.Presentation.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace DMPS.Client.Presentation.Services;

/// <summary>
/// A service to monitor user inactivity and request a session lock.
/// Implements REQ-1-041 (Automatic Session Lock).
/// </summary>
public sealed class SessionLockService : ISessionLockService, IDisposable
{
    private readonly ILogger<SessionLockService> _logger;
    private readonly TimeSpan _inactivityTimeout;
    private readonly Timer _timer;
    private bool _isLocked;
    private bool _isDisposed;

    /// <inheritdoc />
    public event Action? SessionLockRequested;

    public SessionLockService(IConfiguration configuration, ILogger<SessionLockService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        if (!int.TryParse(configuration["ApplicationSettings:InactivityTimeoutMinutes"], out var timeoutMinutes))
        {
            timeoutMinutes = 15; // Default value as per REQ-1-041
            _logger.LogWarning("InactivityTimeoutMinutes not found or invalid in configuration. Using default value of {DefaultMinutes} minutes.", timeoutMinutes);
        }
        _inactivityTimeout = TimeSpan.FromMinutes(timeoutMinutes);
        
        // Use a timer that checks periodically instead of hooking global events, which is less intrusive.
        _timer = new Timer(CheckInactivity, null, Timeout.Infinite, Timeout.Infinite);
        _logger.LogInformation("SessionLockService created with a timeout of {Timeout} minutes.", timeoutMinutes);
    }

    /// <inheritdoc />
    public void Start()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(SessionLockService));
        
        _logger.LogInformation("Session lock monitoring started.");
        _isLocked = false;
        // Check every 5 seconds.
        _timer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    /// <inheritdoc />
    public void Stop()
    {
        if (_isDisposed) return;

        _logger.LogInformation("Session lock monitoring stopped.");
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    /// <inheritdoc />
    public void Unlock()
    {
        if (_isDisposed) return;

        _logger.LogInformation("Session unlocked by user. Resuming inactivity monitoring.");
        _isLocked = false;
        // The timer continues running, so no need to restart it, it will just start passing the check again.
    }

    private void CheckInactivity(object? state)
    {
        if (_isLocked || _isDisposed)
        {
            return;
        }

        try
        {
            var idleTime = GetIdleTime();
            if (idleTime >= _inactivityTimeout)
            {
                _logger.LogInformation("User has been idle for {IdleTime}. Requesting session lock.", idleTime);
                _isLocked = true; // Prevent multiple lock requests
                SessionLockRequested?.Invoke();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking user inactivity.");
            Stop(); // Stop the timer on error to prevent log spam.
        }
    }

    private static TimeSpan GetIdleTime()
    {
        var lastInputInfo = new LASTINPUTINFO();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
        
        if (!GetLastInputInfo(ref lastInputInfo))
        {
            throw new InvalidOperationException($"Win32 GetLastInputInfo failed with error code {Marshal.GetLastWin32Error()}");
        }

        uint ticks = (uint)Environment.TickCount;
        uint lastInputTicks = lastInputInfo.dwTime;

        uint idleTicks = ticks - lastInputTicks;

        return TimeSpan.FromMilliseconds(idleTicks);
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _timer.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    #region P/Invoke for Win32 GetLastInputInfo

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    #endregion
}