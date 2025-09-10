using DMPS.Infrastructure.Communication.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace DMPS.Infrastructure.Communication.RabbitMQ;

/// <summary>
/// A long-running service that consumes messages from a specified RabbitMQ queue,
/// processes them via a callback, and handles message acknowledgment (ACK/NACK).
/// </summary>
public sealed class RabbitMqConsumer(
    IRabbitMqConnectionManager connectionManager,
    IMessageSerializer messageSerializer,
    ILogger<RabbitMqConsumer> logger) : IMessageConsumer, IDisposable
{
    private readonly ILogger<RabbitMqConsumer> _logger = logger;
    private readonly IRabbitMqConnectionManager _connectionManager = connectionManager;
    private readonly IMessageSerializer _messageSerializer = messageSerializer;

    private IModel? _channel;
    private string? _consumerTag;
    private string? _queueName;

    /// <inheritdoc />
    public void StartConsuming<T>(string queueName, Func<T, IDictionary<string, object>, Task<bool>> onMessageReceived) where T : class
    {
        if (_channel != null)
        {
            _logger.LogWarning("Consumer is already running for queue: {QueueName}. Stop the consumer before starting again.", _queueName);
            return;
        }

        _queueName = queueName;
        _channel = _connectionManager.CreateModel();
        _channel.CallbackException += Channel_CallbackException;

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var correlationId = GetCorrelationId(ea.BasicProperties);
            using var scope = _logger.BeginScope("CorrelationId: {CorrelationId}", correlationId);

            try
            {
                var message = _messageSerializer.Deserialize<T>(ea.Body);
                if (message == null)
                {
                    _logger.LogError("Failed to deserialize message from queue {QueueName}. Message will be rejected and sent to DLQ if configured.", _queueName);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                _logger.LogInformation("Processing message from queue {QueueName} with delivery tag {DeliveryTag}.", _queueName, ea.DeliveryTag);

                // Invoke the application-layer callback to process the message.
                bool success = await onMessageReceived(message, ea.BasicProperties.Headers);

                if (success)
                {
                    _logger.LogDebug("Message with delivery tag {DeliveryTag} processed successfully. Acknowledging (ACK).", ea.DeliveryTag);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _logger.LogWarning("Message processing failed as indicated by callback for delivery tag {DeliveryTag}. Rejecting (NACK) without requeue.", ea.DeliveryTag);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing message with delivery tag {DeliveryTag} from queue {QueueName}. Rejecting (NACK) without requeue.", ea.DeliveryTag, _queueName);
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _consumerTag = _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("Consumer started for queue: {QueueName} with consumer tag: {ConsumerTag}", _queueName, _consumerTag);
    }

    private string GetCorrelationId(IBasicProperties basicProperties)
    {
        if (basicProperties?.Headers != null && basicProperties.Headers.TryGetValue("CorrelationId", out var correlationIdObj))
        {
            if (correlationIdObj is byte[] correlationIdBytes)
            {
                return System.Text.Encoding.UTF8.GetString(correlationIdBytes);
            }
        }
        return Guid.NewGuid().ToString(); // Fallback if not present
    }

    private void Channel_CallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.LogCritical(e.Exception, "A RabbitMQ callback exception occurred. This may affect consumer stability. Details: {Details}", e.Detail);
    }

    /// <summary>
    /// Disposes the consumer, canceling the subscription and closing the channel.
    /// </summary>
    public void Dispose()
    {
        _logger.LogInformation("Disposing RabbitMQ consumer for queue {QueueName}.", _queueName);
        if (_channel?.IsOpen == true && !string.IsNullOrEmpty(_consumerTag))
        {
            try
            {
                _channel.BasicCancel(_consumerTag);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while cancelling consumer with tag {ConsumerTag}", _consumerTag);
            }
        }
        
        _channel?.CallbackException -= Channel_CallbackException;
        _channel?.Close();
        _channel?.Dispose();
        _channel = null;
        _logger.LogInformation("RabbitMQ consumer for queue {QueueName} disposed.", _queueName);
    }
}