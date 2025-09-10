using DMPS.Shared.Core.CrossCutting.Exceptions;

namespace DMPS.Shared.Core.CrossCutting.Exceptions;

/// <summary>
/// The exception that is thrown when an attempt is made to create a user with a username that already exists.
/// </summary>
public sealed class DuplicateUsernameException : ValidationException
{
    /// <summary>
    /// Gets the username that caused the exception.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateUsernameException"/> class with a specified username.
    /// </summary>
    /// <param name="username">The username that already exists.</param>
    public DuplicateUsernameException(string username)
        : base($"A user with the username '{username}' already exists.")
    {
        Username = username;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateUsernameException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public DuplicateUsernameException(string message, Exception innerException)
        : base(message, innerException)
    {
        Username = string.Empty;
    }
}