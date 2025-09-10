using System;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.Communication.Interfaces;

/// <summary>
/// Defines a contract for a server that listens for and responds to requests on a Named Pipe.
/// </summary>
public interface INamedPipeServer
{
    /// <summary>
    /// Starts a background task to listen for incoming client connections on the configured named pipe.
    /// </summary>
    /// <param name="onRequestReceived">
    /// An asynchronous callback function that will be invoked with the client's request string.
    /// The function is responsible for processing the request and returning a response string.
    /// </param>
    void StartListening(Func<string, Task<string>> onRequestReceived);

    /// <summary>
    /// Stops the background listener task gracefully.
    /// </summary>
    void StopListening();
}