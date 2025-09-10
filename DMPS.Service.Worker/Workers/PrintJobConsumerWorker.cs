using DMPS.Infrastructure.Communication.Abstractions;
using DMPS.Service.Worker.Handlers;
using DMPS.Service.Worker.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DMPS.Service.Worker.Workers
{
    /// <summary>
    /// A background worker that consumes messages from the print job queue.
    /// It subscribes to the queue and delegates the processing of each print job to a dedicated handler.
    /// </summary>
    public sealed class PrintJobConsumerWorker : BackgroundService
    {
        private readonly ILogger<PrintJobConsumerWorker> _logger;
        private readonly IMessageConsumer _messageConsumer;
        private readonly PrintJobMessageHandler _handler;
        private readonly MessageQueueSettings _settings;

        public PrintJobConsumerWorker(
            ILogger<PrintJobConsumerWorker> logger,
            IOptions<MessageQueueSettings> settings,
            IMessageConsumer messageConsumer,
            PrintJobMessageHandler handler)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(messageConsumer);
            ArgumentNullException.ThrowIfNull(handler);

            _logger = logger;
            _settings = settings.Value;
            _messageConsumer = messageConsumer;
            _handler = handler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Print Job Consumer Worker is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("Print Job Consumer Worker is stopping."));

            try
            {
                _logger.LogInformation("Starting to consume messages from queue: {QueueName}", _settings.PrintJobQueueName);

                await _messageConsumer.StartConsumingAsync(
                    _settings.PrintJobQueueName,
                    _handler.HandleMessageAsync,
                    stoppingToken);

                _logger.LogWarning("Message consumer for queue {QueueName} has stopped.", _settings.PrintJobQueueName);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Print Job Consumer Worker was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A fatal error occurred in the Print Job Consumer Worker for queue {QueueName}, which is now stopping.", _settings.PrintJobQueueName);
            }
            finally
            {
                _logger.LogInformation("Print Job Consumer Worker has shut down.");
            }
        }
    }
}