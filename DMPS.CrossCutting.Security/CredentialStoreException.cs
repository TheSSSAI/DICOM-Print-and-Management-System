using System.Runtime.Serialization;

namespace DMPS.CrossCutting.Security.Exceptions
{
    /// <summary>
    /// Represents an error that occurs during an operation with a secure credential store,
    /// such as the Windows Credential Manager.
    /// </summary>
    [Serializable]
    public class CredentialStoreException : Exception
    {
        /// <summary>
        /// Gets the underlying native error code (e.g., from a Win32 API call), if available.
        /// </summary>
        public int? NativeErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialStoreException"/> class.
        /// </summary>
        public CredentialStoreException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialStoreException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CredentialStoreException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialStoreException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CredentialStoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialStoreException"/> class with a specified error message
        /// and the native error code that caused the exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="nativeErrorCode">The native (e.g., Win32) error code associated with the failure.</param>
        public CredentialStoreException(string message, int nativeErrorCode) : base(message)
        {
            NativeErrorCode = nativeErrorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialStoreException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected CredentialStoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            NativeErrorCode = (int?)info.GetValue(nameof(NativeErrorCode), typeof(int?));
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(NativeErrorCode), NativeErrorCode, typeof(int?));
        }
    }
}