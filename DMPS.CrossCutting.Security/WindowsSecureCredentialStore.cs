using DMPS.CrossCutting.Security.Abstractions;
using DMPS.CrossCutting.Security.Implementations.CredentialManagement.Exceptions;
using DMPS.CrossCutting.Security.Implementations.CredentialManagement.Native;
using DMPS.CrossCutting.Security.Implementations.CredentialManagement.Native.Structs;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace DMPS.CrossCutting.Security.Implementations.CredentialManagement
{
    /// <summary>
    /// Provides a secure way to store and retrieve secrets using the native Windows Credential Manager.
    /// This implementation is platform-specific and will only work on Windows operating systems.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class WindowsSecureCredentialStore : ISecureCredentialStore
    {
        private readonly ILogger<WindowsSecureCredentialStore> _logger;
        private const int ERROR_NOT_FOUND = 1168;

        public WindowsSecureCredentialStore(ILogger<WindowsSecureCredentialStore> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public string GetSecret(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Credential key cannot be null or whitespace.", nameof(key));
            }

            IntPtr credentialPointer = IntPtr.Zero;
            try
            {
                // Attempt to read the credential from the Windows Credential Manager.
                bool success = NativeMethods.CredRead(key, CredentialType.Generic, 0, out credentialPointer);

                if (!success)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    if (lastError == ERROR_NOT_FOUND)
                    {
                        _logger.LogWarning("Secret with key '{Key}' not found in Windows Credential Manager.", key);
                        throw new SecretNotFoundException($"Secret with key '{key}' was not found in the Windows Credential Manager.");
                    }

                    _logger.LogError("Failed to read secret with key '{Key}' from Windows Credential Manager. Win32 Error Code: {ErrorCode}", key, lastError);
                    throw new CredentialStoreException($"Failed to read secret. Win32 Error Code: {lastError}", new Win32Exception(lastError));
                }

                // Marshal the native structure to a managed one.
                var credential = Marshal.PtrToStructure<Credential>(credentialPointer);

                if (credential.CredentialBlob == IntPtr.Zero || credential.CredentialBlobSize == 0)
                {
                    _logger.LogWarning("Secret with key '{Key}' was found but contains an empty value.", key);
                    return string.Empty;
                }

                // Marshal the credential blob (the secret) to a managed string.
                return Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2)!;
            }
            finally
            {
                // CRITICAL: Always free the unmanaged memory allocated by CredRead.
                if (credentialPointer != IntPtr.Zero)
                {
                    NativeMethods.CredFree(credentialPointer);
                }
            }
        }

        /// <inheritdoc />
        public void SetSecret(string key, string secret)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Credential key cannot be null or whitespace.", nameof(key));
            }

            if (secret is null)
            {
                throw new ArgumentNullException(nameof(secret), "Secret value cannot be null.");
            }

            var credential = new Credential
            {
                Type = CredentialType.Generic,
                TargetName = key,
                Persist = CredentialPersist.LocalMachine,
                UserName = Environment.UserName,
                Attributes = IntPtr.Zero,
                AttributeCount = 0
            };
            
            IntPtr secretPointer = IntPtr.Zero;
            try
            {
                // Marshal the managed secret string to an unmanaged memory block.
                secretPointer = Marshal.StringToCoTaskMemUni(secret);
                credential.CredentialBlob = secretPointer;
                credential.CredentialBlobSize = (uint)(secret.Length * 2); // Size in bytes for Unicode string.

                // Write the credential to the Windows Credential Manager.
                bool success = NativeMethods.CredWrite(ref credential, 0);

                if (!success)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    _logger.LogError("Failed to write secret with key '{Key}' to Windows Credential Manager. Win32 Error Code: {ErrorCode}", key, lastError);
                    throw new CredentialStoreException($"Failed to write secret. Win32 Error Code: {lastError}", new Win32Exception(lastError));
                }
                
                _logger.LogInformation("Successfully set secret with key '{Key}' in Windows Credential Manager.", key);
            }
            finally
            {
                // Free the unmanaged memory allocated for the secret.
                if (secretPointer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(secretPointer);
                }
            }
        }
    }
}