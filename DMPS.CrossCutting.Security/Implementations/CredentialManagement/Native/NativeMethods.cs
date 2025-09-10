//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1121 // Use built-in type alias

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using DMPS.CrossCutting.Security.Implementations.CredentialManagement.Native.Structs;

namespace DMPS.CrossCutting.Security.Implementations.CredentialManagement.Native;

/// <summary>
/// Contains P/Invoke method signatures, constants, and structures required to interact with the
/// native Windows Credential Manager functions in advapi32.dll.
/// This class is internal to encapsulate the implementation details of the P/Invoke calls.
/// </summary>
[SupportedOSPlatform("windows")]
internal static partial class NativeMethods
{
    /// <summary>
    /// The credential will be stored with the session. It will be deleted when the user logs off.
    /// </summary>
    internal const int CRED_PERSIST_SESSION = 1;

    /// <summary>
    /// The credential will be stored with the machine. It will be available to all sessions on that machine.
    /// </summary>
    internal const int CRED_PERSIST_LOCAL_MACHINE = 2;

    /// <summary>
    /// The credential will be stored per user. It roams with the user's profile.
    /// </summary>
    internal const int CRED_PERSIST_ENTERPRISE = 3;

    /// <summary>
    /// Win32 error code for "Element not found."
    /// This is returned by CredRead when the specified credential does not exist.
    /// </summary>
    internal const int ERROR_NOT_FOUND = 1168;

    /// <summary>
    /// The CredRead function reads a credential from the user's credential set.
    /// </summary>
    /// <param name="target">The name of the credential to read.</param>
    /// <param name="type">The type of the credential to read. See <see cref="CredentialType"/>.</param>
    /// <param name="flags">Reserved. Must be zero.</param>
    /// <param name="credential">A pointer to a single block of memory that contains the credential information.
    /// This buffer must be freed by calling <see cref="CredFree"/>.</param>
    /// <returns>True if the function succeeds, false otherwise.</returns>
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CredRead(
        [In] string target,
        [In] CredentialType type,
        [In] int flags,
        [Out] out IntPtr credential);

    /// <summary>
    /// The CredWrite function creates a new credential or modifies an existing credential in the Credential Manager.
    /// </summary>
    /// <param name="credential">A <see cref="Credential"/> structure that contains the credential to write.</param>
    /// <param name="flags">Reserved. Must be zero.</param>
    /// <returns>True if the function succeeds, false otherwise.</returns>
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CredWrite(
        [In] ref Credential credential,
        [In] int flags);

    /// <summary>
    /// The CredFree function frees a buffer that was allocated by one of the credential management functions, such as CredRead.
    /// </summary>
    /// <param name="buffer">The buffer to be freed.</param>
    [DllImport("advapi32.dll")]
    internal static extern void CredFree(
        [In] IntPtr buffer);
}