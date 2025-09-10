//-----------------------------------------------------------------------
// <copyright file="Credential.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DMPS.CrossCutting.Security;

using System;
using System.Runtime.InteropServices;
using DMPS.CrossCutting.Security.Abstractions;

/// <summary>
/// A managed representation of the native Windows CREDENTIAL structure.
/// This structure is used for P/Invoke calls to the Windows Credential Manager API.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct Credential
{
    /// <summary>
    /// Member flags.
    /// </summary>
    public int Flags;

    /// <summary>
    /// The type of the credential.
    /// </summary>
    public CredentialType Type;

    /// <summary>
    /// The name of the credential.
    /// </summary>
    public string TargetName;

    /// <summary>
    /// A string comment.
    /// </summary>
    public string Comment;

    /// <summary>
    /// The last written time.
    /// </summary>
    public long LastWritten;

    /// <summary>
    /// The size of the credential blob in bytes.
    /// </summary>
    public int CredentialBlobSize;

    /// <summary>
    /// A pointer to the credential data.
    /// </summary>
    public IntPtr CredentialBlob;

    /// <summary>
    /// Defines the persistence of this credential.
    /// </summary>
    public int Persist;

    /// <summary>
    /// The number of attributes.
    /// </summary>
    public int AttributeCount;

    /// <summary>
    /// A pointer to the credential attributes.
    /// </summary>
    public IntPtr Attributes;

    /// <summary>
    /// An alias for the target name.
    /// </summary>
    public string TargetAlias;

    /// <summary>
    _summary>
    /// The user name of the credential.
    /// </summary>
    public string UserName;
}