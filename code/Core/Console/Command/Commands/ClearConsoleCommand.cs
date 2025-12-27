namespace CosmosCasino.Core.Console.Command.Commands
{
    /// <summary>
    /// Console command that clears all log entries from the active console.
    /// </summary>
    internal sealed class ClearConsoleCommand : IConsoleCommand
    {
        #region FIELDS

        private readonly Func<bool> _tryClearLogs;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the log clear command.
        /// </summary>
        /// <param name="tryClearLogs">
        /// Delegate invoked to clear the console log buffer.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="tryClearLogs"/> is <c>null</c>.
        /// </exception>
        internal ClearConsoleCommand(Func<bool> tryClearLogs)
        {
            _tryClearLogs = tryClearLogs ?? throw new ArgumentNullException(nameof(tryClearLogs));
        }

        #endregion

        #region PROPERTIES

        /// <inheritdoc/>
        string IConsoleCommand.Command => "clear";

        /// <inheritdoc/>
        string IConsoleCommand.Description => "Clear the console logs.";

        /// <inheritdoc/>
        ConsoleCommandSafety IConsoleCommand.Safety => ConsoleCommandSafety.Safe;

        #endregion

        #region METHODS

        /// <inheritdoc/>
        ConsoleCommandResult IConsoleCommand.Execute(string[] args)
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
