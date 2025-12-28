using CosmosCasino.Core.Console.Command;
using CosmosCasino.Core.Console.Logging;

namespace CosmosCasino.Core.Console
{
    /// <summary>
    /// Central runtime debug console.
    /// Collects log output, exposes buffered log data for inspection,
    /// and provides a command execution surface for developer tooling.
    /// </summary>
    public sealed partial class ConsoleManager : IDisposable
    {
        #region FIELDS

        private readonly ConsoleCommandRegistry _commands;
        private readonly ConsoleLogBuffer _buffer;
        private bool _isDisposed;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes the debug console and attaches it to the global
        /// logging pipeline.
        /// Drains any logs recorded during application bootstrap and
        /// begins capturing new log entries in an internal buffer.
        /// </summary>
        /// <param name="logCapacity">
        /// Maximum number of log entries retained in memory.
        /// Older entries are discarded when capacity is exceeded.
        /// </param>
        internal ConsoleManager(int logCapacity = 500)
        {
            using (ConsoleLog.SystemScope(nameof(ConsoleManager)))
            {
                TryClearLogs = ClearLogs;
                _commands = new ConsoleCommandRegistry(this);
                _buffer = new ConsoleLogBuffer(logCapacity);

                // Logs that have been registered before the console is set is stored temporarily in ConsoleLog.
                foreach (var entry in ConsoleLog.DrainEarlyLogs())
                {
                    AddLog(entry);
                }

                ConsoleLog.OnLog += AddLog;
            }
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Internal delegate used by debug commands to clear the log buffer.
        /// Not intended for external callers.
        /// </summary>
        internal Func<bool> TryClearLogs { get; }

        #endregion

        #region METHODS

        /// <summary>
        /// Detaches the console from the global logging pipeline and
        /// releases any associated event subscriptions.
        /// Safe to call multiple times.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            ConsoleLog.OnLog -= AddLog;
            _isDisposed = true;
        }

        /// <summary>
        /// Clears all log entries from the internal buffer.
        /// </summary>
        /// <returns>
        /// Always returns <c>true</c> to indicate the operation completed.
        /// </returns>
        private bool ClearLogs()
        {
            _buffer.Clear();
            Cleared?.Invoke();
            return true;
        }

        /// <summary>
        /// Adds a log entry to the buffer and notifies subscribers.
        /// </summary>
        /// <param name="entry">
        /// The log entry to record.
        /// </param>
        private void AddLog(ConsoleLogEntry entry)
        {
            _buffer.Add(entry);
            EntryAdded?.Invoke(entry);
        }

        #endregion
    }
}
