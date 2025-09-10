//-----------------------------------------------------------------------
// <copyright file="SecretNotFoundException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DMPS.CrossCutting.Security;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Represents an error that occurs when a requested secret (credential) is not found in the secure store.
/// This exception is used to distinguish between a general storage failure and a specific 'not found' condition.
/// </summary>
[Serializable]
public class SecretNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecretNotFoundException"/> class.
    /// </summary>
    public SecretNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SecretNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretNotFoundException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public SecretNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretNotFoundException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected SecretNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}