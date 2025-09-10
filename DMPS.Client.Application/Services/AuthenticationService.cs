using DMPS.Client.Application.DTOs;
using DMPS.Client.Application.Exceptions;
using DMPS.Client.Application.Interfaces;
using DMPS.CrossCutting.Security.Services;
using DMPS.Shared.Core.Domain;
using DMPS.Shared.Core.Repositories;
using Microsoft.Extensions.Logging;
using System.Security;

namespace DMPS.Client.Application.Services;

/// <summary>
/// Orchestrates user authentication, logout, and manages the current user's session state.
/// This service implements the core logic for the authentication use cases.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IApplicationStateService _applicationStateService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IApplicationStateService applicationStateService,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<LoginResult> LoginAsync(string username, SecureString password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNull(password);

        _logger.LogInformation("Login attempt for user: {Username}", username);

        try
        {
            User? user = await _userRepository.GetUserByUsernameAsync(username);

            if (user is null)
            {
                _logger.LogWarning("Authentication failed: User '{Username}' not found.", username);
                throw new AuthenticationFailedException("Invalid username or password.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Authentication failed: User '{Username}' account is disabled.", username);
                throw new AuthenticationFailedException("Your account has been disabled. Please contact an administrator.");
            }

            bool isPasswordVerified = _passwordHasher.VerifyPassword(password, user.PasswordHash);

            if (!isPasswordVerified)
            {
                _logger.LogWarning("Authentication failed: Invalid password for user '{Username}'.", username);
                throw new AuthenticationFailedException("Invalid username or password.");
            }

            _applicationStateService.SetCurrentUser(user);

            _logger.LogInformation("User '{Username}' authenticated successfully. Role: {Role}", user.Username, user.Role?.RoleName);

            return new LoginResult(true, user);
        }
        catch (AuthenticationFailedException)
        {
            // Re-throw specific authentication failures to be handled by the UI.
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during login for user {Username}", username);
            // Abstract underlying exceptions (e.g., database connection issues) into a generic failure.
            throw new AuthenticationFailedException("An unexpected error occurred during login. Please try again later.");
        }
    }

    /// <inheritdoc />
    public void Logout()
    {
        var currentUser = _applicationStateService.CurrentUser;
        if (currentUser is not null)
        {
            _logger.LogInformation("User '{Username}' is logging out.", currentUser.Username);
            _applicationStateService.ClearCurrentUser();
        }
    }

    /// <inheritdoc />
    public async Task<bool> UnlockSessionAsync(SecureString password)
    {
        var currentUser = _applicationStateService.CurrentUser;
        if (currentUser is null)
        {
            _logger.LogWarning("Session unlock attempt failed: No active user session found.");
            return false;
        }
        
        ArgumentNullException.ThrowIfNull(password);

        try
        {
            bool isPasswordVerified = _passwordHasher.VerifyPassword(password, currentUser.PasswordHash);

            if (isPasswordVerified)
            {
                _logger.LogInformation("Session for user '{Username}' unlocked successfully.", currentUser.Username);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed session unlock attempt for user '{Username}'.", currentUser.Username);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during session unlock for user {Username}", currentUser.Username);
            return false;
        }
    }
}