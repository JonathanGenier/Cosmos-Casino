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
        private static bool _isBootstrapping = true;

        #endregion

        #region ACTIONS

        /// <summary>
        /// Raised whenever a new log entry is produced by the logging system.
        /// Intended for internal consumers such as DebugConsole to capture
        /// and buffer log output.
        /// </summary>
        internal static event Action<ConsoleLogEntry>? OnLog;

        #endregion

        #region METHODS

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
            if (!_isBootstrapping)
            {
                return Array.Empty<ConsoleLogEntry>();
            }

            _isBootstrapping = false;

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
            _isBootstrapping = true;
            OnLog = null;
        }
#endif

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

            if (_isBootstrapping)
            {
                _earlyLogs!.Add(entry);
            }

            OnLog?.Invoke(entry);
        }

        #endregion

        #region CLASSES

        /// <summary>
        /// Internal disposable scope used to bracket system initialization
        /// with deterministic lifecycle logging.
        /// Emits a "Setting Up" message on creation and a matching "Ready"
        /// message exactly once when the scope is disposed.
        /// </summary>
        private sealed class SystemLogScope : IDisposable
        {
            private readonly string _category;
            private bool _disposed;

            /// <summary>
            /// Initializes a new system log scope for the specified category
            /// and immediately records the beginning of its setup phase.
            /// </summary>
            /// <param name="category">
            /// Logical system or service name whose initialization lifecycle
            /// is being logged.
            /// </param>
            public SystemLogScope(string category)
            {
                _category = category;
                System(_category, "Setting up...");
            }

            /// <summary>
            /// Completes the system log scope by emitting the corresponding
            /// "Ready" message.
            /// Ensures idempotent disposal so lifecycle completion is logged
            /// at most once.
            /// </summary>
            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                System(_category, "Ready!");
            }
        }

        #endregion
    }
}
