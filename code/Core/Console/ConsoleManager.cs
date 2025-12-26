using CosmosCasino.Core.Console.Command;
using CosmosCasino.Core.Console.Logging;

namespace CosmosCasino.Core.Console
{
    /// <summary>
    /// Central runtime debug console.
    /// Collects log output, exposes buffered log data for inspection,
    /// and provides a command execution surface for developer tooling.
    /// </summary>
    public sealed class ConsoleManager : IDisposable
    {
        #region FIELDS

        private readonly ConsoleCommandRegistry _commands;
        private readonly ConsoleLogBuffer _buffer;
        private bool _disposed;

        #endregion

        #region CONSTRUCTORS

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
            TryClearLogs = ClearLogs;
            _commands = new ConsoleCommandRegistry(this);
            _buffer = new ConsoleLogBuffer(logCapacity);
            _disposed = false;

            // Logs that have been registered before the console is set is stored temporarily in DevLog.
            foreach (var entry in ConsoleLog.DrainEarlyLogs())
            {
                AddLog(entry);
            }

            ConsoleLog.OnLog += AddLog;
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Invoked whenever a new log entry is added to the buffer.
        /// Intended for reactive consumers such as debug UI or diagnostics.
        /// </summary>
        public event Action<ConsoleLogEntry>? EntryAdded;

        /// <summary>
        /// Raised when the console log buffer is cleared.
        /// Intended for presentation-layer consumers to react
        /// to a full reset of console state (e.g. clearing UI output).
        /// </summary>
        public event Action? Cleared;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The fixed maximum number of log entries retained by the buffer.
        /// </summary>
        public int Capacity => _buffer.Capacity;

        /// <summary>
        /// Monotonic counter incremented on every log write.
        /// Used for change detection by consumers.
        /// </summary>
        public long Version => _buffer.Version;

        /// <summary>
        /// Number of log entries currently stored.
        /// </summary>
        public int Count => _buffer.Count;

        /// <summary>
        /// Internal delegate used by debug commands to clear the log buffer.
        /// Not intended for external callers.
        /// </summary>
        internal Func<bool> TryClearLogs { get; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Returns a snapshot of all buffered log entries ordered
        /// from oldest to newest.
        /// </summary>
        /// <returns>
        /// A read-only collection of log entries currently stored
        /// in the console buffer.
        /// </returns>
        public IReadOnlyList<ConsoleLogEntry> GetLogs()
        {
            return _buffer.Snapshot();
        }

        /// <summary>
        /// Executes a debug command entered through the console.
        /// </summary>
        /// <param name="input">
        /// Raw command input string. The first token represents
        /// the command identifier; remaining tokens are treated as arguments.
        /// </param>
        /// <returns>
        /// The result of command execution, including success state
        /// and optional feedback message.
        /// </returns>
        public ConsoleCommandResult ExecuteCommand(string input)
        {
            return _commands.Execute(input);
        }

        /// <summary>
        /// Detaches the console from the global logging pipeline and
        /// releases any associated event subscriptions.
        /// Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            ConsoleLog.OnLog -= AddLog;
            _disposed = true;
        }

        #endregion

        #region PRIVATE METHODS

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
