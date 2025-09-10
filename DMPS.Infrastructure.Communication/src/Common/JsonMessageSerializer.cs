using DMPS.Infrastructure.Communication.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMPS.Infrastructure.Communication.Common
{
    /// <summary>
    /// Implements message serialization and deserialization using System.Text.Json.
    /// </summary>
    public sealed class JsonMessageSerializer : IMessageSerializer
    {
        private readonly ILogger<JsonMessageSerializer> _logger;
        private readonly JsonSerializerOptions _options;

        public JsonMessageSerializer(ILogger<JsonMessageSerializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <inheritdoc />
        public T? Deserialize<T>(byte[] data) where T : class
        {
            if (data == null || data.Length == 0)
            {
                _logger.LogWarning("Attempted to deserialize null or empty byte array to type {TypeName}.", typeof(T).Name);
                return default;
            }

            try
            {
                var jsonString = Encoding.UTF8.GetString(data);
                return JsonSerializer.Deserialize<T>(jsonString, _options);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON to type {TypeName}. Invalid JSON format.", typeof(T).Name);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during deserialization to type {TypeName}.", typeof(T).Name);
                return default;
            }
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T message) where T : class
        {
            if (message == null)
            {
                _logger.LogWarning("Attempted to serialize a null message of type {TypeName}.", typeof(T).Name);
                return Array.Empty<byte>();
            }

            try
            {
                var jsonString = JsonSerializer.Serialize(message, _options);
                return Encoding.UTF8.GetBytes(jsonString);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to serialize message of type {TypeName} to JSON.", typeof(T).Name);
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during serialization of type {TypeName}.", typeof(T).Name);
                return Array.Empty<byte>();
            }
        }
    }
}