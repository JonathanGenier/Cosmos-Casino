namespace CosmosCasino.Core.Debug.Logging
{
    /// <summary>
    /// PUBLIC API
    /// Internal implementation of the DevLog logging pipeline.
    /// </summary>
    public static partial class DevLog
    {
        #region PUBLIC METHODS

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
        /// Writes a system-level diagnostic log entry.
        /// System logs are intended for low-level application and infrastructure
        /// lifecycle events such as boot sequencing, service initialization,
        /// and internal state transitions.
        /// These entries are marked as unsafe by default and are therefore
        /// stripped from production builds.
        /// </summary>
        /// <param name="category">
        /// Logical subsystem producing the log entry
        /// (e.g. BootController, AppManager, ClientServices).
        /// </param>
        /// <param name="message">
        /// Human-readable diagnostic message describing the system action.
        /// </param>
        public static void System(
            string category,
            string message)
        {
            Write(LogLevel.Info, LogSafety.Unsafe, LogKind.System, category, message);
        }

        #endregion
    }
}
