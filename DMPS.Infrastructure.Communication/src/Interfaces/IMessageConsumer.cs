namespace DMPS.Infrastructure.Communication.Interfaces;

/// <summary>
/// Defines a contract for a service that consumes and processes messages from a queue.
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Initiates a long-running listener on a specified queue to consume messages.
    /// </summary>
    /// <typeparam name="T">The type of the message object to deserialize.</typeparam>
    /// <param name="queueName">The name of the queue to consume from.</param>
    /// <param name="onMessageReceived">
    /// An asynchronous callback function that is invoked for each received message.
    /// The function should return a Task that resolves to 'true' if the message was processed successfully and should be acknowledged (ACK),
    /// or 'false' if processing failed and the message should be negatively acknowledged (NACK), potentially for retry or dead-lettering.
    /// </param>
    void StartConsuming<T>(string queueName, Func<T, Task<bool>> onMessageReceived) where T : class;
}