using DMPS.Infrastructure.Communication.Common;
using DMPS.Infrastructure.Communication.Interfaces;
using DMPS.Infrastructure.Communication.Pipes;
using DMPS.Infrastructure.Communication.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DMPS.Infrastructure.Communication.Extensions
{
    /// <summary>
    /// Provides extension methods for IServiceCollection to register communication infrastructure services.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all necessary services for the communication infrastructure layer, including RabbitMQ and Named Pipes.
        /// This method encapsulates the configuration and registration of all communication components,
        /// simplifying the setup process in the consuming application's composition root.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configuration">The IConfiguration instance for retrieving settings.</param>
        /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown if services or configuration is null.</exception>
        public static IServiceCollection AddCommunicationInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            // Configure strongly-typed settings objects from the IConfiguration provider.
            // This leverages the Options pattern for clean, decoupled configuration.
            services.Configure<RabbitMqSettings>(configuration.GetSection("Communication:RabbitMq"));
            services.Configure<PipeSettings>(configuration.GetSection("Communication:Pipes"));

            // Register the RabbitMQ connection manager as a singleton.
            // A single, shared, resilient connection to the broker is managed for the application's lifetime,
            // which is critical for performance and resource management.
            services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();

            // Register the message producer and consumer.
            // Scoped lifetime is a safe default, ensuring a new channel is used per logical operation scope,
            // which aligns with RabbitMQ.Client's recommendation for channel thread-safety.
            services.AddScoped<IMessageProducer, RabbitMqProducer>();
            services.AddScoped<IMessageConsumer, RabbitMqConsumer>();
            
            // Register the Named Pipe server as a singleton.
            // Only one server instance can listen on a specific pipe name at any given time,
            // making the singleton lifetime mandatory.
            services.AddSingleton<INamedPipeServer, NamedPipeServer>();

            // Register the Named Pipe client as transient.
            // Each call requires a new NamedPipeClientStream instance which is disposable.
            // A transient lifetime ensures a new client is created for each request, which is the correct usage pattern.
            services.AddTransient<INamedPipeClient, NamedPipeClient>();

            // Register the message serializer as a singleton.
            // Since the serializer is stateless, a single instance can be shared throughout the application
            // for optimal performance and memory usage.
            services.AddSingleton<IMessageSerializer, JsonMessageSerializer>();
            
            return services;
        }
    }
}