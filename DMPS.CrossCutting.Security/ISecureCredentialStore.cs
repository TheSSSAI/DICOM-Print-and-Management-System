namespace DMPS.CrossCutting.Security.Abstractions
{
    /// <summary>
    /// Defines the contract for a key-value store that provides secure storage and retrieval of sensitive secrets,
    /// such as database connection strings or API keys.
    /// Implementations are expected to be platform-specific (e.g., Windows Credential Manager, macOS Keychain).
    /// </summary>
    public interface ISecureCredentialStore
    {
        /// <summary>
        /// Retrieves a secret from the secure store.
        /// </summary>
        /// <param name="key">The unique key (e.g., target name) identifying the secret to retrieve. Must not be null or empty.</param>
        /// <returns>The retrieved secret value as a string.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the key is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the key is empty or whitespace.</exception>
        /// <exception cref="DMPS.CrossCutting.Security.Exceptions.SecretNotFoundException">Thrown if no secret is found for the given key.</exception>
        /// <exception cref="DMPS.CrossCutting.Security.Exceptions.CredentialStoreException">Thrown for other underlying storage errors.</exception>
        string GetSecret(string key);

        /// <summary>
        /// Stores or updates a secret in the secure store. The operation should be idempotent.
        /// </summary>
        /// <param name="key">The unique key (e.g., target name) under which to store the secret. Must not be null or empty.</param>
        /// <param name="secret">The sensitive value to be stored. Must not be null or empty.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the key or secret is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the key or secret is empty or whitespace.</exception>
        /// <exception cref="DMPS.CrossCutting.Security.Exceptions.CredentialStoreException">Thrown if the store operation fails for any reason.</exception>
        void SetSecret(string key, string secret);
    }
}