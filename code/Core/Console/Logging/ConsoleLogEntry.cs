namespace CosmosCasino.Core.Console.Logging
{
    /// <summary>
    /// INTERNAL CODE
    /// Represents a single immutable, structured log entry
    /// produced by the ConsoleLog logging system.
    /// </summary>
    public sealed partial class ConsoleLogEntry
    {
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
        internal ConsoleLogEntry(
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
            DisplayKind = MapDisplayKind(Level, Kind);
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Severity level of the log entry.
        /// </summary>
        internal ConsoleLogLevel Level { get; }

        /// <summary>
        /// Indicates whether the log entry is allowed to appear
        /// in production environments.
        /// </summary>
        internal ConsoleLogSafety Safety { get; }

        /// <summary>
        /// Semantic classification of the log entry
        /// </summary>
        internal ConsoleLogKind Kind { get; }

        #endregion

        #region METHODS

        private static ConsoleLogDisplayKind MapDisplayKind(ConsoleLogLevel level, ConsoleLogKind kind)
        {
            return level switch
            {
                ConsoleLogLevel.Error => ConsoleLogDisplayKind.Error,
                ConsoleLogLevel.Warning => ConsoleLogDisplayKind.Warning,
                ConsoleLogLevel.Verbose => ConsoleLogDisplayKind.Muted,
                ConsoleLogLevel.Info => MapByLogKind(kind),
                _ => ConsoleLogDisplayKind.General
            };
        }

        private static ConsoleLogDisplayKind MapByLogKind(ConsoleLogKind kind)
        {
            return kind switch
            {
                ConsoleLogKind.General => ConsoleLogDisplayKind.General,
                ConsoleLogKind.Event => ConsoleLogDisplayKind.Event,
                ConsoleLogKind.System => ConsoleLogDisplayKind.System,
                _ => ConsoleLogDisplayKind.General
            };
        }

        #endregion
    }
}
