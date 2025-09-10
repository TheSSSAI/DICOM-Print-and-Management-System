using DMPS.Infrastructure.Communication.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Threading;

namespace DMPS.Infrastructure.Communication.RabbitMQ
{
    /// <summary>
    /// Manages a single, persistent, and resilient connection to the RabbitMQ broker.
    /// Designed to be registered as a singleton to share the connection across the application.
    /// </summary>
    public sealed class RabbitMqConnectionManager : IRabbitMqConnectionManager, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqConnectionManager> _logger;
        private readonly RabbitMqSettings _settings;
        private readonly object _syncRoot = new();
        private IConnection? _connection;
        private bool _disposed;
        private readonly ResiliencePipeline _retryPipeline;

        public RabbitMqConnectionManager(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqConnectionManager> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _connectionFactory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _retryPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<BrokerUnreachableException>().Handle<IOException>(),
                    Delay = TimeSpan.FromSeconds(5),
                    MaxRetryAttempts = _settings.RetryCount > 0 ? _settings.RetryCount : 3,
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = args =>
                    {
                        _logger.LogWarning("Failed to connect to RabbitMQ. Retrying in {Delay}s... Attempt {AttemptNumber}/{MaxAttempts}.",
                            args.RetryDelay.TotalSeconds, args.AttemptNumber + 1, _settings.RetryCount);
                        return default;
                    }
                })
                .Build();
        }

        public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                _logger.LogError("RabbitMQ connection is not open. Cannot create a channel.");
                throw new InvalidOperationException("No RabbitMQ connection is available to create a channel.");
            }
            return _connection!.CreateModel();
        }

        public void TryConnect()
        {
            if (IsConnected) return;

            lock (_syncRoot)
            {
                if (IsConnected) return;

                _logger.LogInformation("Attempting to connect to RabbitMQ at {HostName}:{Port}...", _settings.HostName, _settings.Port);

                try
                {
                    _retryPipeline.Execute(() =>
                    {
                        _connection = _connectionFactory.CreateConnection();
                    });

                    if (IsConnected)
                    {
                        _connection!.ConnectionShutdown += OnConnectionShutdown;
                        _connection.CallbackException += OnCallbackException;
                        _connection.ConnectionBlocked += OnConnectionBlocked;
                        _logger.LogInformation("Successfully connected to RabbitMQ at {HostName} and established event handlers.", _connection.Endpoint.HostName);
                    }
                    else
                    {
                        _logger.LogCritical("Failed to connect to RabbitMQ after all retries.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "A critical, unhandled exception occurred while trying to connect to RabbitMQ.");
                }
            }
        }

        private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            _logger.LogWarning("RabbitMQ connection is blocked. Reason: {Reason}", e.Reason);
        }

        private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, "A RabbitMQ callback exception occurred.");
        }

        private void OnConnectionShutdown(object? sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;
            
            _logger.LogWarning("RabbitMQ connection was shut down. Reason: {Reason}. Will attempt to reconnect automatically.", reason.ReplyText);
            
            // The client library's automatic recovery will handle reconnection.
            // A manual retry could be added here if more control is needed.
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try
            {
                if (_connection != null)
                {
                    _connection.ConnectionShutdown -= OnConnectionShutdown;
                    _connection.CallbackException -= OnCallbackException;
                    _connection.ConnectionBlocked -= OnConnectionBlocked;
                    
                    if (_connection.IsOpen)
                    {
                         _connection.Close();
                    }
                    _connection.Dispose();
                }
                _logger.LogInformation("RabbitMQ connection manager disposed and connection closed.");
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex, "An I/O exception occurred during RabbitMQ connection disposal.");
            }
        }
    }
}