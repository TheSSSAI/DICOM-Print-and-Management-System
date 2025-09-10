using DMPS.Client.Application.Interfaces;
using DMPS.Client.Application.Events;
using DMPS.Shared.Core.Domain;
using Microsoft.Extensions.Logging;

namespace DMPS.Client.Application.Services
{
    /// <summary>
    /// Manages the global state of the client application, such as the currently authenticated user.
    /// This service acts as a centralized store for session-specific information.
    /// </summary>
    public sealed class ApplicationStateService : IApplicationStateService
    {
        private readonly ILogger<ApplicationStateService> _logger;
        private User? _currentUser;

        /// <inheritdoc />
        public User? CurrentUser
        {
            get => _currentUser;
            private set
            {
                if (_currentUser == value) return;
                _currentUser = value;
                OnCurrentUserChanged(new UserLoggedInEventArgs(_currentUser));
            }
        }

        /// <inheritdoc />
        public event EventHandler<UserLoggedInEventArgs>? CurrentUserChanged;

        public ApplicationStateService(ILogger<ApplicationStateService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public void SetCurrentUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (CurrentUser?.UserId == user.UserId)
            {
                _logger.LogWarning("Attempted to set the same user ({UserId}) as the current user again.", user.UserId);
                return;
            }

            _logger.LogInformation("Setting current user to {Username} (ID: {UserId}).", user.Username, user.UserId);
            CurrentUser = user;
        }

        /// <inheritdoc />
        public void ClearCurrentUser()
        {
            if (CurrentUser is null)
            {
                _logger.LogInformation("No current user to clear. Application state is already clean.");
                return;
            }

            _logger.LogInformation("Clearing current user {Username} (ID: {UserId}) from application state.", CurrentUser.Username, CurrentUser.UserId);
            CurrentUser = null;
        }

        /// <summary>
        /// Raises the CurrentUserChanged event in a thread-safe manner.
        /// </summary>
        /// <param name="e">The event arguments containing the current user information.</param>
        private void OnCurrentUserChanged(UserLoggedInEventArgs e)
        {
            _logger.LogDebug("Raising CurrentUserChanged event. User is now {Username}.", e.User?.Username ?? "null");
            CurrentUserChanged?.Invoke(this, e);
        }
    }
}