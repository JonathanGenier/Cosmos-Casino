namespace CosmosCasino.Core.Console.Command
{
    /// <summary>
    /// Represents the outcome of a debug command execution.
    /// Encapsulates success state and optional human-readable feedback
    /// intended for display in the debug console.
    /// </summary>
    public sealed partial class ConsoleCommandResult
    {
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

        /// <summary>
        /// Indicates whether this command result should be displayed
        /// in the debug console UI.
        /// This allows commands to suppress redundant or purely
        /// side-effect-driven feedback (e.g. clearing the console).
        /// </summary>
        public bool ShowInConsole { get; }

        #endregion
    }
}
