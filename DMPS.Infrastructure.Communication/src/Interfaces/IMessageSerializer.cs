namespace DMPS.Infrastructure.Communication.Interfaces;

/// <summary>
/// Defines a contract for serializing and deserializing message payloads.
/// This allows for interchangeable serialization strategies (e.g., JSON, MessagePack).
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Serializes an object into a byte array.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A byte array representing the serialized object.</returns>
    byte[] Serialize<T>(T obj) where T : class;

    /// <summary>
    /// Deserializes a byte array into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize into.</typeparam>
    /// <param name="data">The byte array containing the serialized data.</param>
    /// <returns>The deserialized object.</returns>
    T? Deserialize<T>(byte[] data) where T : class;
}