namespace CosmosCasino.Core.Console.Logging
{
    /// <summary>
    /// INTERNAL CORE LOGIC
    /// Central logging API used by all systems.
    /// Applies environment safety rules and routes log entries
    /// into a fixed-size in-memory buffer for diagnostics and UI.
    /// </summary>
    public static partial class ConsoleLog
    {
        #region FIELDS

        private static List<ConsoleLogEntry>? _earlyLogs = new();
        private static bool _bootstrapping = true;

        #endregion

        #region ACTIONS

        /// <summary>
        /// Raised whenever a new log entry is produced by the logging system.
        /// Intended for internal consumers such as DebugConsole to capture
        /// and buffer log output.
        /// </summary>
        internal static event Action<ConsoleLogEntry>? OnLog;

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Drains all log entries recorded during application bootstrap and
        /// permanently disables early-log buffering.
        /// This method is intended to be called exactly once when the
        /// DebugConsole is initialized.
        /// </summary>
        /// <returns>
        /// The collection of log entries recorded during bootstrap,
        /// or an empty collection if buffering has already been drained.
        /// </returns>
        internal static IReadOnlyList<ConsoleLogEntry> DrainEarlyLogs()
        {
            if (!_bootstrapping)
            {
                return Array.Empty<ConsoleLogEntry>();
            }

            _bootstrapping = false;

            var drained = _earlyLogs!;
            _earlyLogs = null;

            return drained;
        }

#if DEBUG
        /// <summary>
        /// TEST SUPPORT ONLY.
        /// Resets static logging state to allow deterministic unit testing.
        /// Not used in production code paths.
        /// </summary>
        internal static void ResetForUnitTests()
        {
            _earlyLogs = new List<ConsoleLogEntry>();
            _bootstrapping = true;
            OnLog = null;
        }
#endif
        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Core logging implementation.
        /// Applies environment safety filtering, timestamps the entry,
        /// stores it in the log buffer, and mirrors output in debug builds.
        /// </summary>
        private static void Write(
            ConsoleLogLevel level,
            ConsoleLogSafety safety,
            ConsoleLogKind kind,
            string category,
            string message)
        {

            // Enforces that errors and warning should always be visible in debug or prod no matter what is the info.
            if ((level == ConsoleLogLevel.Error || level == ConsoleLogLevel.Warning) && safety == ConsoleLogSafety.Unsafe)
            {
#if DEBUG
                throw new InvalidOperationException(
                    "Error and Warning logs must always be safe."
                );
#else
                safety = ConsoleLogSafety.Safe;
#endif
            }

            // Enforce production safety.
#if !DEBUG
            if (safety == ConsoleLogSafety.Unsafe)
            {
                return;
            }
#endif

            long timestampMs = Time.AppTime.ElapsedMs;

            var entry = new ConsoleLogEntry(
                timestampMs,
                level,
                safety,
                kind,
                category,
                message
            );

            if (_bootstrapping)
            {
                _earlyLogs!.Add(entry);
            }

            OnLog?.Invoke(entry);
        }

        #endregion
    }
}
