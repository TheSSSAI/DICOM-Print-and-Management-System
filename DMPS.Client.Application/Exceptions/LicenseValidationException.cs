namespace DMPS.Client.Application.Exceptions
{
    /// <summary>
    /// Represents an error that occurs during the license validation process.
    /// </summary>
    [Serializable]
    public class LicenseValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseValidationException"/> class.
        /// </summary>
        public LicenseValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseValidationException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LicenseValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseValidationException"/> class 
        /// with a specified error message and a reference to the inner exception that is the 
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public LicenseValidationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}