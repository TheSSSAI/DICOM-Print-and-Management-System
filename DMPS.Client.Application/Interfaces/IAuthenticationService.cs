using DMPS.Client.Application.DTOs;

namespace DMPS.Client.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for user authentication and session management.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Event that is raised after a user has successfully logged in.
        /// </summary>
        event EventHandler<Events.UserLoggedInEventArgs>? UserLoggedIn;

        /// <summary>
        /// Event that is raised after a user has logged out.
        /// </summary>
        event EventHandler? UserLoggedOut;

        /// <summary>
        /// Attempts to authenticate a user with the provided credentials.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's plaintext password.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a 
        /// <see cref="LoginResult"/> object indicating the outcome of the authentication attempt.
        /// </returns>
        /// <exception cref="Exceptions.AuthenticationFailedException">
        /// Thrown for any authentication failure, such as invalid credentials or a disabled account.
        /// The exception message should be generic to prevent user enumeration.
        /// </exception>
        Task<LoginResult> LoginAsync(string username, string password);

        /// <summary>
        /// Clears the current user's session state, effectively logging them out.
        /// </summary>
        void Logout();
    }
}