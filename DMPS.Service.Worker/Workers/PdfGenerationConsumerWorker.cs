using DMPS.Infrastructure.Communication.Abstractions;
using DMPS.Service.Worker.Handlers;
using DMPS.Service.Worker.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DMPS.Service.Worker.Workers
{
    /// <summary>
    /// A background worker that consumes messages from the PDF generation queue.
    /// It subscribes to the queue and delegates the processing of each PDF generation request to a dedicated handler.
    /// </summary>
    public sealed class PdfGenerationConsumerWorker : BackgroundService
    {
        private readonly ILogger<PdfGenerationConsumerWorker> _logger;
        private readonly IMessageConsumer _messageConsumer;
        private readonly PdfGenerationMessageHandler _handler;
        private readonly MessageQueueSettings _settings;

        public PdfGenerationConsumerWorker(
            ILogger<PdfGenerationConsumerWorker> logger,
            IOptions<MessageQueueSettings> settings,
            IMessageConsumer messageConsumer,
            PdfGenerationMessageHandler handler)
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
            _logger.LogInformation("PDF Generation Consumer Worker is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("PDF Generation Consumer Worker is stopping."));
            
            try
            {
                _logger.LogInformation("Starting to consume messages from queue: {QueueName}", _settings.PdfGenerationQueueName);

                await _messageConsumer.StartConsumingAsync(
                    _settings.PdfGenerationQueueName,
                    _handler.HandleMessageAsync,
                    stoppingToken);

                _logger.LogWarning("Message consumer for queue {QueueName} has stopped.", _settings.PdfGenerationQueueName);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PDF Generation Consumer Worker was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A fatal error occurred in the PDF Generation Consumer Worker for queue {QueueName}, which is now stopping.", _settings.PdfGenerationQueueName);
            }
            finally
            {
                _logger.LogInformation("PDF Generation Consumer Worker has shut down.");
            }
        }
    }
}