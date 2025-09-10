//-----------------------------------------------------------------------
// <copyright file="BCryptPasswordHasher.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DMPS.CrossCutting.Security;

using System;
using BCrypt.Net;
using DMPS.CrossCutting.Security.Abstractions;
using DMPS.CrossCutting.Security.Configuration;
using Microsoft.Extensions.Options;

/// <summary>
/// Implements the <see cref="IPasswordHasher"/> interface using the BCrypt.Net-Next library.
/// This class provides secure, salted password hashing and verification.
/// </summary>
public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private readonly int _workFactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="BCryptPasswordHasher"/> class.
    /// </summary>
    /// <param name="options">The configuration options for BCrypt, specifying the work factor.</param>
    /// <exception cref="ArgumentNullException">Thrown if options or options.Value is null.</exception>
    public BCryptPasswordHasher(IOptions<BCryptSettings> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);

        this._workFactor = options.Value.WorkFactor;
    }

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
        }

        return BCrypt.HashPassword(password, this._workFactor);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        try
        {
            // BCrypt.Verify handles both hashing and comparison in a constant-time manner
            // to prevent timing attacks.
            return BCrypt.Verify(password, hashedPassword);
        }
        catch (SaltParseException)
        {
            // This exception is thrown if the hashedPassword is not in a valid BCrypt format.
            // We must catch this and return false to prevent information leakage about the
            // hash format or potential enumeration attacks. Do not re-throw.
            return false;
        }
        catch (Exception)
        {
            // Catch any other unexpected exceptions from the library for safety.
            return false;
        }
    }
}