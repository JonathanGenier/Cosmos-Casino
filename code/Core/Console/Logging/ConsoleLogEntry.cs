namespace CosmosCasino.Core.Console.Logging
{
    /// <summary>
    /// Represents a single immutable, structured log entry
    /// produced by the DevLog logging system.
    /// </summary>
    public readonly struct ConsoleLogEntry
    {
        #region FIELDS

        /// <summary>
        /// Timestamp in milliseconds since application start.
        /// </summary>
        public readonly long TimestampMs;

        /// <summary>
        /// Severity level of the log entry.
        /// </summary>
        public readonly ConsoleLogLevel Level;

        /// <summary>
        /// Indicates whether the log entry is allowed to appear
        /// in production environments.
        /// </summary>
        public readonly ConsoleLogSafety Safety;

        /// <summary>
        /// Semantic classification of the log entry
        /// </summary>
        public readonly ConsoleLogKind Kind;

        /// <summary>
        /// Logical subsystem or feature that produced the log
        /// </summary>
        public readonly string Category;

        /// <summary>
        /// Human-readable diagnostic message.
        /// </summary>
        public readonly string Message;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new structured log entry.
        /// Null category and message values are normalized to
        /// safe default strings.
        /// </summary>
        /// <param name="timestampMs">
        /// Timestamp in milliseconds since application start.
        /// </param>
        /// <param name="level">
        /// Severity level of the log entry.
        /// </param>
        /// <param name="safety">
        /// Controls whether the entry is allowed in production.
        /// </param>
        /// <param name="kind">
        /// Semantic classification of the log entry.
        /// </param>
        /// <param name="category">
        /// Logical subsystem producing the log.
        /// </param>
        /// <param name="message">
        /// Human-readable log message.
        /// </param>
        public ConsoleLogEntry(
            long timestampMs,
            ConsoleLogLevel level,
            ConsoleLogSafety safety,
            ConsoleLogKind kind,
            string category,
            string message)
        {
            TimestampMs = timestampMs;
            Level = level;
            Safety = safety;
            Kind = kind;
            Category = category ?? "Undefined";
            Message = message ?? string.Empty;
        }

        #endregion
    }
}
