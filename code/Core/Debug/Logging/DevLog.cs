using CosmosCasino.Core.Debug.Environment;

namespace CosmosCasino.Core.Debug.Logging
{
    /// <summary>
    /// Central logging API used by all systems.
    /// Applies environment safety rules and routes log entries
    /// into a fixed-size in-memory buffer for diagnostics and UI.
    /// </summary>
    public static partial class DevLog
    {
        // =========================================================================
        // INTERNAL CORE LOGIC
        // =========================================================================

        private static readonly LogBuffer _buffer = new LogBuffer(capacity: 500);

        /// <summary>
        /// Exposes the internal log buffer for read-only consumers
        /// such as debug UI and diagnostic tools.
        /// </summary>
        public static LogBuffer Buffer => _buffer;

        /// <summary>
        /// Core logging implementation.
        /// Applies environment safety filtering, timestamps the entry,
        /// stores it in the log buffer, and mirrors output in debug builds.
        /// </summary>
        private static void Write(
            LogLevel level,
            LogSafety safety,
            LogKind kind,
            string category,
            string message)
        {

            // Enforces that errors and warning should always be visible in debug or prod no matter what is the info.
            if((level == LogLevel.Error || level == LogLevel.Warning) && safety == LogSafety.Unsafe)
            {
#if DEBUG
                throw new InvalidOperationException(
                    "Error and Warning logs must always be safe."
                );
#else
                safety = LogSafety.Safe;
#endif
            }

            // Enforce production safety.
            if(AppEnvironment.IsProd && safety == LogSafety.Unsafe)
            {
                return;
            }

            long timestampMs = Time.AppTime.ElapsedMs;

            var entry = new LogEntry(
                timestampMs,
                level,
                safety,
                kind,
                category,
                message
            );

            _buffer.Add(entry);

#if DEBUG
            MirrorToConsole(entry);
#endif
        }

#if DEBUG
        /// <summary>
        /// Mirrors log entries to the standard console in debug builds.
        /// This is a temporary development aid and will be replaced
        /// by the in-game debug UI.
        /// </summary>
        private static void MirrorToConsole(in LogEntry entry)
        {
            Console.WriteLine(
                $"[{entry.TimestampMs}] [{entry.Category}] {entry.Message}"
            );
        }
#endif
    }
}
