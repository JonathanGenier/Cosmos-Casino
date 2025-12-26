namespace CosmosCasino.Core.Debug.Command.Commands.Log
{
    /// <summary>
    /// Debug command that clears all log entries from the active
    /// debug console.
    /// </summary>
    public sealed class LogClearCommand : IDebugCommand
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
        public LogClearCommand(Func<bool> tryClearLogs)
        {
            _tryClearLogs = tryClearLogs ?? throw new ArgumentNullException(nameof(tryClearLogs));
        }

        #endregion

        #region PUBLIC METHODS

        /// <inheritdoc/>
        public string Command => "log.clear";

        /// <inheritdoc/>
        public string Description => "Clear the console logs.";

        /// <inheritdoc/>
        public CommandSafety Safety => CommandSafety.Safe;

        /// <inheritdoc/>
        public CommandResult Execute(string[] args)
        {
            // No args expected
            bool success = _tryClearLogs();

            return success
                ? CommandResult.Ok("Logs cleared")
                : CommandResult.Failed("Failed to clear logs");
        }

        #endregion
    }
}
