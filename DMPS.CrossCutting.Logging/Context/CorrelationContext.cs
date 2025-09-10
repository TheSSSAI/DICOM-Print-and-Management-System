namespace DMPS.CrossCutting.Logging.Context
{
    /// <summary>
    /// Provides a static, thread-safe, and async-aware mechanism for storing and retrieving 
    /// the current operation's Correlation ID. This context flows with asynchronous calls.
    /// </summary>
    /// <remarks>
    /// This class utilizes <see cref="AsyncLocal{T}"/> to maintain an ambient context that is
    /// unique to a given asynchronous control flow. It is the foundation for fulfilling REQ-1-090,
    /// allowing for end-to-end tracing of operations.
    /// </remarks>
    public static class CorrelationContext
    {
        /// <summary>
        /// The underlying storage for the correlation ID that flows with the asynchronous execution context.
        /// </summary>
        private static readonly AsyncLocal<string?> _correlationId = new();

        /// <summary>
        /// Sets the correlation ID for the current asynchronous execution context.
        /// This should be called at the beginning of an operation (e.g., in middleware, a message consumer, or a service entry point).
        /// </summary>
        /// <param name="correlationId">The correlation ID to set for the current execution context. Should be a non-empty string, often a GUID.</param>
        public static void SetCorrelationId(string correlationId)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                // While we don't throw to avoid breaking the caller, we prevent setting a meaningless value.
                // An empty or null ID is effectively no ID.
                _correlationId.Value = null;
                return;
            }

            _correlationId.Value = correlationId;
        }

        /// <summary>
        /// Retrieves the correlation ID from the current asynchronous execution context.
        /// This is typically called by the CorrelationIdEnricher to add the ID to log events.
        /// </summary>
        /// <returns>The correlation ID for the current context, or null if it has not been set.</returns>
        public static string? GetCorrelationId()
        {
            return _correlationId.Value;
        }

        /// <summary>
        /// Clears the correlation ID for the current asynchronous execution context.
        /// Useful for cleanup in scenarios where execution contexts might be reused.
        /// </summary>
        public static void Clear()
        {
            _correlationId.Value = null;
        }
    }
}