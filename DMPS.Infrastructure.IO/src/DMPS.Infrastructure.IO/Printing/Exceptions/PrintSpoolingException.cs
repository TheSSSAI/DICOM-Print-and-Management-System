using System;
using System.Runtime.Serialization;

namespace DMPS.Infrastructure.IO.Printing.Exceptions;

/// <summary>
/// Represents errors that occur during the interaction with the operating system's print spooler.
/// This exception is typically thrown for issues like an invalid printer name or a failure in the printing subsystem.
/// </summary>
[Serializable]
public class PrintSpoolingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrintSpoolingException"/> class.
    /// </summary>
    public PrintSpoolingException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintSpoolingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PrintSpoolingException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintSpoolingException"/> class with a specified error message 
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public PrintSpoolingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintSpoolingException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected PrintSpoolingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}