namespace DMPS.Infrastructure.Communication.Interfaces;

/// <summary>
/// Defines a contract for a client performing synchronous-style request/reply communication over a Named Pipe.
/// </summary>
public interface INamedPipeClient
{
    /// <summary>
    /// Asynchronously sends a request string to the Named Pipe server and waits for a response string.
    /// </summary>
    /// <param name="request">The request string to send to the server.</param>
    /// <param name="token">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the server's response string,
    /// or null if no response was received.
    /// </returns>
    /// <exception cref="TimeoutException">
    /// Thrown by the implementation if a connection to the server pipe cannot be established within the configured timeout period,
    /// indicating that the server is likely unavailable.
    /// </exception>
    Task<string?> SendRequestAsync(string request, CancellationToken token);
}