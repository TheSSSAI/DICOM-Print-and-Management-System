namespace DMPS.CrossCutting.Security.Abstractions
{
    /// <summary>
    /// Defines the contract for a service that provides one-way password hashing and verification.
    /// Implementations of this interface should be thread-safe and stateless.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Creates a secure, salted hash of the provided plaintext password.
        /// </summary>
        /// <param name="password">The plaintext password to hash. Must not be null or empty.</param>
        /// <returns>A string containing the generated password hash, including the salt and algorithm identifiers.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the provided password is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the provided password is empty or whitespace.</exception>
        string HashPassword(string password);

        /// <summary>
        /// Verifies that a plaintext password matches a stored password hash.
        /// </summary>
        /// <param name="password">The plaintext password to verify. Must not be null or empty.</param>
        /// <param name="hashedPassword">The stored hash to compare against. Must not be null or empty.</param>
        /// <returns><c>true</c> if the password matches the hash; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Implementations of this method MUST use a constant-time comparison algorithm to mitigate timing attacks.
        /// The method should return false for any exceptions during verification (e.g., malformed hash) rather than
        /// re-throwing, to prevent information leakage about the hash format.
        /// </remarks>
        bool VerifyPassword(string password, string hashedPassword);
    }
}