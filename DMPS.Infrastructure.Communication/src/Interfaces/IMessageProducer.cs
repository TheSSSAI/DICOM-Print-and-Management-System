namespace DMPS.Infrastructure.Communication.Interfaces;

/// <summary>
/// Defines a technology-agnostic contract for publishing messages to a message broker.
/// </summary>
public interface IMessageProducer
{
    /// <summary>
    /// Serializes and publishes a message to a specific exchange and routing key.
    /// This operation is fire-and-forget from the caller's perspective. Reliability, such as persistence and durability,
    /// is handled by the concrete implementation.
    /// </summary>
    /// <typeparam name="T">The type of the message object.</typeparam>
    /// <param name="message">The message object to publish.</param>
    /// <param name="exchange">The name of the exchange to publish to.</param>
    /// <param name="routingKey">The routing key for the message.</param>
    /// <param name="correlationId">An optional correlation ID for tracing the message across services.</param>
    void Publish<T>(T message, string exchange, string routingKey, string? correlationId = null) where T : class;
}