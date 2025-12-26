namespace CosmosCasino.Core.Debug.Command
{
    /// <summary>
    /// Represents the outcome of a debug command execution.
    /// Encapsulates success state and optional human-readable feedback
    /// intended for display in the debug console.
    /// </summary>
    public readonly struct CommandResult
    {
        /// <summary>
        /// Initializes a new command result with the specified outcome.
        /// </summary>
        /// <param name="success">
        /// Indicates whether the command completed successfully.
        /// </param>
        /// <param name="message">
        /// Optional feedback message associated with the command result.
        /// </param>
        private CommandResult(bool success, string message)
        {
            Success = success;
            Message = message ?? string.Empty;
        }

        #region PROPERTIES

        /// <summary>
        /// Indicates whether the command executed successfully.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Feedback message produced by the command.
        /// May be empty if no feedback was provided.
        /// </summary>
        public string Message { get; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Creates a successful command result.
        /// </summary>
        /// <param name="message">
        /// Optional feedback message to display in the debug console.
        /// </param>
        /// <returns>
        /// A <see cref="CommandResult"/> representing successful execution.
        /// </returns>
        public static CommandResult Ok(string? message = null)
        {
            return new CommandResult(true, message ?? string.Empty);
        }

        /// <summary>
        /// Creates a failed command result.
        /// </summary>
        /// <param name="message">
        /// Optional feedback message explaining the failure.
        /// </param>
        /// <returns>
        /// A <see cref="CommandResult"/> representing failed execution.
        /// </returns>
        public static CommandResult Failed(string? message = null)
        {
            return new CommandResult(false, message ?? string.Empty);
        }

        #endregion
    }
}
