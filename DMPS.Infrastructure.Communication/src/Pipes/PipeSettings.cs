using System.ComponentModel.DataAnnotations;

namespace DMPS.Infrastructure.Communication.Pipes;

/// <summary>
/// Provides strongly-typed configuration for Named Pipe communication,
/// intended for use with the .NET IOptions pattern.
/// </summary>
public sealed class PipeSettings
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Communication:Pipes";

    /// <summary>
    /// Gets the unique name for the Named Pipe. This must be identical in both the
    /// client and server applications to establish communication.
    /// </summary>
    [Required]
    public string PipeName { get; init; } = "DMPStatusPipe";

    /// <summary>
    /// Gets the timeout in milliseconds for the client to wait for a connection
    /// to the server pipe.
    /// </summary>
    [Range(100, 10000)]
    public int ConnectionTimeoutMs { get; init; } = 2000;
}