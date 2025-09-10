// DMPS.Shared.Core/Domain/Services/IPasswordPolicyValidator.cs
using DMPS.Shared.Core.Common;

namespace DMPS.Shared.Core.Domain.Services
{
    /// <summary>
    /// Defines the contract for a domain service that validates passwords against the system's defined policies.
    /// </summary>
    public interface IPasswordPolicyValidator
    {
        /// <summary>
        /// Validates a password against the configured system-wide policies.
        /// </summary>
        /// <param name="password">The plaintext password to validate.</param>
        /// <param name="passwordHistoryHashes">A collection of the user's previous password hashes to check for reuse.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure. If failed, the Error will contain details of the policy violations.</returns>
        Result Validate(string password, IEnumerable<string> passwordHistoryHashes);
    }
}