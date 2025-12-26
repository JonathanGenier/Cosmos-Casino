namespace CosmosCasino.Core.Console.Command.Commands
{
    /// <summary>
    /// Debug command that clears all log entries from the active
    /// debug console.
    /// </summary>
    public sealed class ClearConsoleCommand : IConsoleCommand
    {
        #region FIELDS

        private readonly Func<bool> _tryClearLogs;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the log clear command.
        /// </summary>
        /// <param name="tryClearLogs">
        /// Delegate invoked to clear the console log buffer.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="tryClearLogs"/> is <c>null</c>.
        /// </exception>
        public ClearConsoleCommand(Func<bool> tryClearLogs)
        {
            _tryClearLogs = tryClearLogs ?? throw new ArgumentNullException(nameof(tryClearLogs));
        }

        #endregion

        #region PUBLIC METHODS

        /// <inheritdoc/>
        public string Command => "clear";

        /// <inheritdoc/>
        public string Description => "Clear the console logs.";

        /// <inheritdoc/>
        public ConsoleCommandSafety Safety => ConsoleCommandSafety.Safe;

        /// <inheritdoc/>
        public ConsoleCommandResult Execute(string[] args)
        {
            // No args expected
            bool success = _tryClearLogs();

            return success
                ? ConsoleCommandResult.Ok("Logs cleared", false)
                : ConsoleCommandResult.Failed("Failed to clear logs");
        }

        #endregion
    }
}
