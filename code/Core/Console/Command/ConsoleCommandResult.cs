namespace CosmosCasino.Core.Console.Command
{
    /// <summary>
    /// Represents the outcome of a debug command execution.
    /// Encapsulates success state and optional human-readable feedback
    /// intended for display in the debug console.
    /// </summary>
    public sealed partial class ConsoleCommandResult
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new command result with the specified outcome.
        /// </summary>
        /// <param name="success">
        /// Indicates whether the command completed successfully.
        /// </param>
        /// <param name="message">
        /// Optional feedback message associated with the command result.
        /// </param>
        /// <param name="showInConsole">
        /// Indicates whether this command result should be rendered
        /// as feedback in the console UI.
        /// </param>
        private ConsoleCommandResult(bool success, string message, bool showInConsole)
        {
            Success = success;
            Message = message ?? string.Empty;
            ShowInConsole = showInConsole;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Creates a successful command result.
        /// </summary>
        /// <param name="message">
        /// Optional feedback message to display in the debug console.
        /// </param>
        /// <param name="showInConsole">
        /// Indicates whether the feedback message should be rendered
        /// in the debug console UI.
        /// </param>
        /// <returns>
        /// A <see cref="ConsoleCommandResult"/> representing successful execution.
        /// </returns>
        internal static ConsoleCommandResult Ok(string? message = null, bool showInConsole = true)
        {
            return new ConsoleCommandResult(true, message ?? string.Empty, showInConsole);
        }

        /// <summary>
        /// Creates a failed command result.
        /// </summary>
        /// <param name="message">
        /// Optional feedback message explaining the failure.
        /// </param>
        /// <returns>
        /// A <see cref="ConsoleCommandResult"/> representing failed execution.
        /// </returns>
        internal static ConsoleCommandResult Failed(string? message = null)
        {
            return new ConsoleCommandResult(false, message ?? string.Empty, true);
        }

        #endregion
    }
}
