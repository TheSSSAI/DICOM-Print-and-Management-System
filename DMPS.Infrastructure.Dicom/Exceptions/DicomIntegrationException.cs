using System.Runtime.Serialization;

namespace DMPS.Infrastructure.Dicom.Exceptions;

/// <summary>
/// Represents a base class for exceptions that occur during DICOM integration operations,
/// such as protocol errors or configuration issues.
/// </summary>
[Serializable]
public class DicomIntegrationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DicomIntegrationException"/> class.
    /// </summary>
    public DicomIntegrationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DicomIntegrationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DicomIntegrationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DicomIntegrationException"/> class with a specified error message 
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DicomIntegrationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DicomIntegrationException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected DicomIntegrationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}