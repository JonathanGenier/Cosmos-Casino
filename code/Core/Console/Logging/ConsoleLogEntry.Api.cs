namespace CosmosCasino.Core.Console.Logging
{
    /// <summary>
    /// Represents a single immutable, structured log entry exposed 
    /// to the console.
    /// This type provides read-only access to log data intended for
    /// inspection, diagnostics, and presentation layers. Internal
    /// classification and routing details are intentionally hidden.
    /// </summary>
    public sealed partial class ConsoleLogEntry
    {
        #region PROPERTIES

        /// <summary>
        /// Timestamp in milliseconds since application start.
        /// </summary>
        public long TimestampMs { get; }

        /// <summary>
        /// Logical subsystem or feature that produced the log
        /// (e.g. AI, World, Save).
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Diagnostic message associated with the log entry.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// High-level display category used by the console to determine
        /// how the log entry should be visually presented.
        /// </summary>
        public ConsoleLogDisplayKind DisplayKind { get; }

        #endregion
    }
}
