namespace DMPS.Client.Application.Exceptions
{
    /// <summary>
    /// Represents an error that occurs during user authentication. This exception should be
    /// used to wrap any specific authentication failure (e.g., user not found, incorrect password,
    /// disabled account) to provide a single, generic exception type to the presentation layer,
    /// preventing user enumeration attacks.
    /// </summary>
    [Serializable]
    public class AuthenticationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFailedException"/> class.
        /// </summary>
        public AuthenticationFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFailedException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AuthenticationFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFailedException"/> class 
        /// with a specified error message and a reference to the inner exception that is the 
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public AuthenticationFailedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}