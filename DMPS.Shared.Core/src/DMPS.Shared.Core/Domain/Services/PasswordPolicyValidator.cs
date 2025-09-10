using System.Text;
using System.Text.RegularExpressions;
using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Entities;

namespace DMPS.Shared.Core.Domain.Services;

/// <summary>
/// A domain service that validates passwords against a defined system policy.
/// This service is stateless and contains pure business logic.
/// </summary>
public sealed partial class PasswordPolicyValidator : IPasswordPolicyValidator
{
    // Regex for password complexity checks, compiled for performance.
    [GeneratedRegex("[A-Z]")]
    private static partial Regex HasUppercaseRegex();
    [GeneratedRegex("[a-z]")]
    private static partial Regex HasLowercaseRegex();
    [GeneratedRegex("[0-9]")]
    private static partial Regex HasNumberRegex();
    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex HasSymbolRegex();
    
    /// <inheritdoc />
    public Result Validate(string password, PasswordPolicySettings settings, User? userContext, IEnumerable<string> passwordHistoryHashes, Func<string, string, bool> verifyPasswordHash)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(password))
        {
            return Result.Failure("Password cannot be empty.");
        }
        
        ValidateLength(password, settings, errors);
        ValidateComplexity(password, settings, errors);
        ValidateNotContainsUsername(password, userContext, errors);
        ValidateHistory(password, passwordHistoryHashes, settings, verifyPasswordHash, errors);
        
        if (errors.Count > 0)
        {
            var errorMessage = new StringBuilder("Password does not meet policy requirements:");
            foreach (var error in errors)
            {
                errorMessage.Append($"\n- {error}");
            }
            return Result.Failure(errorMessage.ToString());
        }

        return Result.Success();
    }

    private static void ValidateLength(string password, PasswordPolicySettings settings, ICollection<string> errors)
    {
        if (password.Length < settings.MinimumLength)
        {
            errors.Add($"Password must be at least {settings.MinimumLength} characters long.");
        }
    }
    
    private static void ValidateComplexity(string password, PasswordPolicySettings settings, ICollection<string> errors)
    {
        if (settings.RequireUppercase && !HasUppercaseRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one uppercase letter.");
        }
        if (settings.RequireLowercase && !HasLowercaseRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one lowercase letter.");
        }
        if (settings.RequireNumber && !HasNumberRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one number.");
        }
        if (settings.RequireSymbol && !HasSymbolRegex().IsMatch(password))
        {
            errors.Add("Password must contain at least one symbol (e.g., !@#$%).");
        }
    }

    private static void ValidateNotContainsUsername(string password, User? userContext, ICollection<string> errors)
    {
        if (userContext is not null && !string.IsNullOrEmpty(userContext.Username))
        {
            if (password.Contains(userContext.Username, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Password cannot contain the username.");
            }
        }
    }

    private static void ValidateHistory(string password, IEnumerable<string> passwordHistoryHashes, PasswordPolicySettings settings, Func<string, string, bool> verifyPasswordHash, ICollection<string> errors)
    {
        if (settings.HistoryReusePreventionCount <= 0) return;

        var recentHashes = passwordHistoryHashes.Take(settings.HistoryReusePreventionCount);

        foreach (var hash in recentHashes)
        {
            if (verifyPasswordHash(password, hash))
            {
                errors.Add($"Password cannot be one of the last {settings.HistoryReusePreventionCount} passwords used.");
                // We can break after the first match.
                return;
            }
        }
    }
}