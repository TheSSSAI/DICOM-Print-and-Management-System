using DMPS.Infrastructure.Communication.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace DMPS.Infrastructure.Communication.RabbitMQ;

/// <summary>
/// Provides a mechanism for publishing messages to a RabbitMQ broker in a reliable, persistent manner.
/// </summary>
public sealed class RabbitMqProducer(
    IRabbitMqConnectionManager connectionManager,
    IMessageSerializer messageSerializer,
    ILogger<RabbitMqProducer> logger) : IMessageProducer
{
    private readonly IRabbitMqConnectionManager _connectionManager = connectionManager;
    private readonly IMessageSerializer _messageSerializer = messageSerializer;
    private readonly ILogger<RabbitMqProducer> _logger = logger;

    // Cache topology declarations to avoid re-declaring on every publish.
    private readonly ConcurrentDictionary<string, bool> _declaredExchanges = new();

    /// <inheritdoc />
    public void Publish<T>(T message, string exchange, string routingKey, string? correlationId = null, IDictionary<string, object>? headers = null) where T : class
    {
        if (!_connectionManager.IsConnected)
        {
            _logger.LogError("Cannot publish message. RabbitMQ connection is not available.");
            // In a real-world scenario, you might throw a specific exception or queue this locally for later sending.
            // For this system, logging the error is sufficient as the caller will handle the exception from CreateModel.
            throw new InvalidOperationException("RabbitMQ connection is not available.");
        }

        using var channel = _connectionManager.CreateModel();

        try
        {
            using var scope = _logger.BeginScope("CorrelationId: {CorrelationId}", correlationId ?? "(new)");
            _logger.LogDebug("Publishing message to exchange '{Exchange}' with routing key '{RoutingKey}'", exchange, routingKey);

            // Ensure the exchange exists. This is idempotent.
            EnsureExchangeExists(channel, exchange);

            var body = _messageSerializer.Serialize(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Mark message as persistent (delivery_mode = 2)
            properties.ContentType = _messageSerializer.ContentType;

            properties.Headers = headers ?? new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                properties.CorrelationId = correlationId;
                // Add as a header as well for easier inspection and cross-platform compatibility
                properties.Headers["CorrelationId"] = correlationId;
            }

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Successfully published message of type {MessageType} to exchange '{Exchange}'", typeof(T).Name, exchange);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType} to exchange '{Exchange}' with routing key '{RoutingKey}'", typeof(T).Name, exchange, routingKey);
            throw; // Re-throw to allow the caller to handle the failure (e.g., retry or notify user)
        }
    }

    private void EnsureExchangeExists(IModel channel, string exchange)
    {
        // Only declare if we haven't successfully done so before in this instance.
        if (_declaredExchanges.ContainsKey(exchange)) return;
        
        try
        {
            _logger.LogDebug("Declaring direct exchange '{ExchangeName}' to ensure it exists.", exchange);
            // Assuming a direct exchange as per most common command patterns.
            // This could be parameterized if other exchange types are needed.
            channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true, autoDelete: false);
            _declaredExchanges.TryAdd(exchange, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to declare exchange '{ExchangeName}'. Subsequent publish attempts might fail.", exchange);
            // Do not re-throw, allow the publish to attempt and fail if the exchange truly does not exist.
        }
    }
}