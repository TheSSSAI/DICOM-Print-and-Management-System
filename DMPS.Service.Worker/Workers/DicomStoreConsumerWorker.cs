using DMPS.Infrastructure.Communication.Abstractions;
using DMPS.Service.Worker.Handlers;
using DMPS.Service.Worker.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DMPS.Service.Worker.Workers
{
    /// <summary>
    /// A background worker that consumes messages from the DICOM storage queue.
    /// It subscribes to the queue and delegates the processing of each message to a dedicated handler.
    /// </summary>
    public sealed class DicomStoreConsumerWorker : BackgroundService
    {
        private readonly ILogger<DicomStoreConsumerWorker> _logger;
        private readonly IMessageConsumer _messageConsumer;
        private readonly DicomStoreMessageHandler _handler;
        private readonly MessageQueueSettings _settings;

        public DicomStoreConsumerWorker(
            ILogger<DicomStoreConsumerWorker> logger,
            IOptions<MessageQueueSettings> settings,
            IMessageConsumer messageConsumer,
            DicomStoreMessageHandler handler)
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
            _logger.LogInformation("DICOM Store Consumer Worker is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("DICOM Store Consumer Worker is stopping."));

            try
            {
                _logger.LogInformation("Starting to consume messages from queue: {QueueName}", _settings.DicomStoreQueueName);

                await _messageConsumer.StartConsumingAsync(
                    _settings.DicomStoreQueueName,
                    _handler.HandleMessageAsync,
                    stoppingToken);

                _logger.LogWarning("Message consumer for queue {QueueName} has stopped.", _settings.DicomStoreQueueName);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("DICOM Store Consumer Worker was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A fatal error occurred in the DICOM Store Consumer Worker for queue {QueueName}, which is now stopping.", _settings.DicomStoreQueueName);
            }
            finally
            {
                _logger.LogInformation("DICOM Store Consumer Worker has shut down.");
            }
        }
    }
}