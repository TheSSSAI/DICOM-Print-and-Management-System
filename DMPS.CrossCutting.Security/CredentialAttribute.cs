//-----------------------------------------------------------------------
// <copyright file="CredentialAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DMPS.CrossCutting.Security;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// A managed representation of the native Windows CREDENTIAL_ATTRIBUTE structure.
/// This structure is used for P/Invoke calls to the Windows Credential Manager API.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct CredentialAttribute
{
    /// <summary>
    /// The keyword for the attribute.
    /// </summary>
    public string Keyword;

    /// <summary>
    /// Attribute flags.
    /// </summary>
    public int Flags;

    /// <summary>
    /// The size of the value in bytes.
    /// </summary>
    public int ValueSize;

    /// <summary>
    /// A pointer to the attribute value.
    /// </summary>
    public IntPtr Value;
}