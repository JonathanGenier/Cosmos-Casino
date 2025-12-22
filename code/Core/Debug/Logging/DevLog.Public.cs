namespace CosmosCasino.Core.Debug.Logging
{
    /// <summary>
    /// Internal implementation of the DevLog logging pipeline.
    /// </summary>
    public static partial class DevLog
    {
        // =========================================================================
        // PUBLIC API
        // =========================================================================

        /// <summary>
        /// Writes a general informational log entry.
        /// Used for normal application flow and high-level state changes.
        /// </summary>
        /// <param name="category">
        /// Logical subsystem producing the log (e.g. AI, World, Save).
        /// </param>
        /// <param name="message">
        /// Human-readable diagnostic message.
        /// </param>
        /// <param name="safety">
        /// Controls whether the entry is allowed in production builds.
        /// Unsafe entries are stripped in production.
        /// </param>
        public static void Info(
            string category,
            string message,
            LogSafety safety = LogSafety.Safe)
        {
            Write(LogLevel.Info, safety, LogKind.General, category, message);
        }

        /// <summary>
        /// Writes a developer-focused log entry.
        /// Intended for deep diagnostics and internal reasoning.
        /// Unsafe by default and stripped in production builds.
        /// </summary>
        /// <param name="category">
        /// Logical subsystem producing the log (e.g. AI, World, Save).
        /// </param>
        /// <param name="message">
        /// Human-readable diagnostic message.
        /// </param>
        /// <param name="safety">
        /// Controls whether the entry is allowed in production builds.
        /// Unsafe entries are stripped in production.
        /// </param>
        public static void Verbose(
            string category,
            string message,
            LogSafety safety = LogSafety.Unsafe)
        {
            Write(LogLevel.Verbose, safety, LogKind.General, category, message);
        }

        /// <summary>
        /// Writes a warning log entry.
        /// Indicates unexpected or recoverable situations that may
        /// require attention but do not halt execution.
        /// Warnings should always appear in all environments.
        /// </summary>
        /// <param name="category">
        /// Logical subsystem producing the log (e.g. AI, World, Save).
        /// </param>
        /// <param name="message">
        /// Human-readable diagnostic message.
        /// </param>
        public static void Warning(
            string category,
            string message)
        {
            Write(LogLevel.Warning, LogSafety.Safe, LogKind.General, category, message);
        }

        /// <summary>
        /// Writes an error log entry.
        /// Indicates a failure or critical issue that prevents
        /// an operation from completing successfully.
        /// Errors should always appear in all environments.
        /// </summary>
        /// <param name="category">
        /// Logical subsystem producing the log (e.g. AI, World, Save).
        /// </param>
        /// <param name="message">
        /// Human-readable diagnostic message.
        /// </param>
        public static void Error(
            string category,
            string message)
        {
            Write(LogLevel.Error, LogSafety.Safe, LogKind.General, category, message);
        }

        /// <summary>
        /// Writes a semantic event log entry.
        /// Events represent discrete, meaningful occurrences such as
        /// lifecycle transitions or world state changes.
        /// </summary>
        /// <param name="category">
        /// Logical subsystem producing the log (e.g. AI, World, Save).
        /// </param>
        /// <param name="message">
        /// Human-readable diagnostic message.
        /// </param>
        /// <param name="safety">
        /// Controls whether the entry is allowed in production builds.
        /// Unsafe entries are stripped in production.
        /// </param>
        public static void Event(
            string category,
            string message,
            LogSafety safety = LogSafety.Safe)
        {
            Write(LogLevel.Info, safety, LogKind.Event, category, message);
        }

        /// <summary>
        /// Writes a command log entry.
        /// Commands represent user or developer-initiated actions
        /// such as console input or debug commands.
        /// </summary>
        /// <param name="category">
        /// Logical subsystem producing the log (e.g. AI, World, Save).
        /// </param>
        /// <param name="message">
        /// Human-readable diagnostic message.
        /// </param>
        /// <param name="safety">
        /// Controls whether the entry is allowed in production builds.
        /// Unsafe entries are stripped in production.
        /// </param>
        public static void Command(
            string category,
            string message,
            LogSafety safety = LogSafety.Safe)
        {
            Write(LogLevel.Info, safety, LogKind.Command, category, message);
        }
    }
}
