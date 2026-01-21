namespace CosmosCasino.Core.Application.Console.Logging
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

        /// <summary>
        /// Maps the specified log level and log kind to a corresponding display kind for console output.
        /// </summary>
        /// <param name="level">The severity level of the log entry to map.</param>
        /// <param name="kind">The kind of log entry, used to further refine the display kind mapping for certain log levels.</param>
        /// <returns>A value of <see cref="ConsoleLogDisplayKind"/> that represents how the log entry should be displayed in the
        /// console.</returns>
        private static ConsoleLogDisplayKind MapDisplayKind(ConsoleLogLevel level, ConsoleLogKind kind)
        {
            return level switch
            {
                ConsoleLogLevel.Error => ConsoleLogDisplayKind.Error,
                ConsoleLogLevel.Warning => ConsoleLogDisplayKind.Warning,
                ConsoleLogLevel.Verbose => MapVerboseByLogKind(kind),
                ConsoleLogLevel.Info => MapInfoByLogKind(kind),
                _ => ConsoleLogDisplayKind.General
            };
        }

        /// <summary>
        /// Maps a specified log kind to its corresponding console log display kind.
        /// </summary>
        /// <param name="kind">The log kind to map to a display kind.</param>
        /// <returns>The corresponding <see cref="ConsoleLogDisplayKind"/> value for the specified log kind. Returns <see
        /// cref="ConsoleLogDisplayKind.General"/> if the log kind is not recognized.</returns>
        private static ConsoleLogDisplayKind MapInfoByLogKind(ConsoleLogKind kind)
        {
            return kind switch
            {
                ConsoleLogKind.General => ConsoleLogDisplayKind.General,
                ConsoleLogKind.Event => ConsoleLogDisplayKind.Event,
                ConsoleLogKind.System => ConsoleLogDisplayKind.System,
                _ => ConsoleLogDisplayKind.General
            };
        }

        /// <summary>
        /// Maps a specified log kind to its corresponding verbose display kind for console output.
        /// </summary>
        /// <param name="kind">The log kind to map to a verbose display kind.</param>
        /// <returns>A value of <see cref="ConsoleLogDisplayKind"/> that represents the verbose display kind for the specified
        /// log kind. Returns <see cref="ConsoleLogDisplayKind.Debug"/> if <paramref name="kind"/> is <see
        /// cref="ConsoleLogKind.Debug"/>; otherwise, returns <see cref="ConsoleLogDisplayKind.Muted"/>.</returns>
        private static ConsoleLogDisplayKind MapVerboseByLogKind(ConsoleLogKind kind)
        {
            return kind switch
            {
                ConsoleLogKind.Debug => ConsoleLogDisplayKind.Debug,
                _ => ConsoleLogDisplayKind.Muted
            };
        }

        #endregion
    }
}