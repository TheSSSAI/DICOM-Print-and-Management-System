using DMPS.Shared.Core.Domain;

namespace DMPS.Client.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that manages the global state of the client application,
    /// such as the currently authenticated user and license status.
    /// This service acts as a single source of truth for application-wide state.
    /// </summary>
    public interface IApplicationStateService
    {
        /// <summary>
        /// Event that is raised when the current user's state changes (login or logout).
        /// </summary>
        event EventHandler? CurrentUserChanged;

        /// <summary>
        /// Gets the currently authenticated user. Returns null if no user is logged in.
        /// </summary>
        User? CurrentUser { get; }

        /// <summary>
        /// Sets the currently authenticated user, typically after a successful login.
        /// </summary>
        /// <param name="user">The authenticated user object.</param>
        void SetCurrentUser(User user);

        /// <summary>
        /// Clears the current user, typically after a logout.
        /// </summary>
        void ClearCurrentUser();
    }
}