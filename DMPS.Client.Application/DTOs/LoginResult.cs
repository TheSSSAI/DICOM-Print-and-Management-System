using DMPS.Shared.Core.Domain;

namespace DMPS.Client.Application.DTOs
{
    /// <summary>
    /// Represents the outcome of a user login attempt.
    /// </summary>
    /// <param name="IsSuccess">A value indicating whether the login was successful.</param>
    /// <param name="User">The authenticated user object if the login was successful; otherwise, null.</param>
    /// <param name="ErrorMessage">A descriptive error message if the login failed; otherwise, null.</param>
    public sealed record LoginResult(
        bool IsSuccess,
        User? User,
        string? ErrorMessage);
}