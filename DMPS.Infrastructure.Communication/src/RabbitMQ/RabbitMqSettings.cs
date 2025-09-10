using System.ComponentModel.DataAnnotations;

namespace DMPS.Infrastructure.Communication.RabbitMQ;

/// <summary>
/// Provides strongly-typed configuration for connecting to the RabbitMQ broker,
/// intended for use with the .NET IOptions pattern.
/// </summary>
public sealed class RabbitMqSettings
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Communication:RabbitMq";

    /// <summary>
    /// Gets the hostname or IP address of the RabbitMQ server.
    /// </summary>
    [Required]
    public string HostName { get; init; } = "localhost";

    /// <summary>
    /// Gets the port number for the RabbitMQ server.
    /// </summary>
    [Range(1, 65535)]
    public int Port { get; init; } = 5672;

    /// <summary>
    /// Gets the username for authentication.
    /// </summary>
    [Required]
    public string UserName { get; init; } = "guest";

    /// <summary>
    /// Gets the password for authentication.
    /// It is strongly recommended to store this securely (e.g., user secrets, Azure Key Vault)
    /// and not in plaintext configuration files for production environments.
    /// </summary>
    [Required]
    public string Password { get; init; } = "guest";
    
    /// <summary>
    /// Gets the number of times to retry establishing a connection before failing.
    /// </summary>
    [Range(0, 10)]
    public int RetryCount { get; init; } = 5;
}