using CosmosCasino.Core.Application.Console.Command;

namespace CosmosCasino.Core.Application.Console
{
    /// <summary>
    /// Central runtime debug console.
    /// Collects log output, exposes buffered log data for inspection,
    /// and provides a command execution surface for developer tooling.
    /// </summary>
    public sealed partial class ConsoleManager : IDisposable
    {
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
        /// Number of log entries currently stored.
        /// </summary>
        public int Count => _buffer.Count;

        #endregion

        #region METHODS

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

        #endregion
    }
}
