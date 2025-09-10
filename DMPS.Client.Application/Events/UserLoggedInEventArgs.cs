using DMPS.Shared.Core.Domain;
using System.Diagnostics.CodeAnalysis;

namespace DMPS.Client.Application.Events
{
    /// <summary>
    /// Provides data for the UserLoggedIn event.
    /// </summary>
    public sealed class UserLoggedInEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the user who has logged in.
        /// </summary>
        public User AuthenticatedUser { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoggedInEventArgs"/> class.
        /// </summary>
        /// <param name="authenticatedUser">The user who has logged in.</param>
        public UserLoggedInEventArgs([NotNull] User authenticatedUser)
        {
            ArgumentNullException.ThrowIfNull(authenticatedUser);
            AuthenticatedUser = authenticatedUser;
        }
    }
}