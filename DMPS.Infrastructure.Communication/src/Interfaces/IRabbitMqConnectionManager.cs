using RabbitMQ.Client;
using System;

namespace DMPS.Infrastructure.Communication.Interfaces;

/// <summary>
/// Manages the lifecycle of a single, shared, and resilient connection to a RabbitMQ broker.
/// This interface should be registered as a Singleton in the dependency injection container.
/// </summary>
public interface IRabbitMqConnectionManager : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the connection to the RabbitMQ broker is currently established.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Creates a new channel (IModel) from the single, managed connection.
    /// This method will attempt to establish a connection if one is not already present.
    /// </summary>
    /// <returns>A new RabbitMQ channel (IModel).</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a connection cannot be established after configured retries.
    /// </exception>
    IModel CreateModel();
}