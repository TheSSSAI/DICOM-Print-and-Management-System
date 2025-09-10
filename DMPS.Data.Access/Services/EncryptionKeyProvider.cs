using DMPS.Shared.Core.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace DMPS.Data.Access.Services;

/// <summary>
/// Provides the database encryption key from a secure configuration source.
/// This service is registered as a singleton to ensure the key is loaded only once on application startup.
/// </summary>
/// <remarks>
/// This implementation directly supports REQ-NFR-004 by externalizing sensitive credentials/keys.
/// It is a critical component for REQ-1-083, which mandates data-at-rest encryption for PHI.
/// </remarks>
public sealed class EncryptionKeyProvider : IEncryptionKeyProvider
{
    /// <summary>
    /// The configuration key used to retrieve the encryption key from IConfiguration.
    /// </summary>
    public const string EncryptionKeyConfigurationPath = "Database:EncryptionKey";

    private readonly string _encryptionKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptionKeyProvider"/> class.
    /// </summary>
    /// <param name="configuration">The application's configuration provider.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the encryption key is not found or is empty in the application's configuration.
    /// This is a fail-fast mechanism to prevent the application from starting in an insecure or non-functional state.
    /// </exception>
    public EncryptionKeyProvider(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var key = configuration[EncryptionKeyConfigurationPath];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                $"Database encryption key is missing or empty. " +
                $"Ensure that a value is provided for the '{EncryptionKeyConfigurationPath}' configuration key in a secure configuration source (e.g., user secrets, Azure Key Vault).");
        }

        _encryptionKey = key;
    }

    /// <summary>
    /// Gets the configured database encryption key.
    /// </summary>
    /// <returns>The database encryption key as a string.</returns>
    [SuppressMessage("Microsoft.Security", "CA5394", Justification = "This method provides a key for a symmetric algorithm, but the key itself is managed securely.")]
    public string GetKey() => _encryptionKey;
}