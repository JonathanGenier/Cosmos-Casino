namespace CosmosCasino.Core.Debug.Logging
{
    /// <summary>
    /// Represents the severity and verbosity of a log entry.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Informational messages such as lifecycle events and milestones.
        /// </summary>
        Info,

        /// <summary>
        /// High-frequency internal narration.
        /// Usually noisy and developer-focused.
        /// </summary>
        Verbose,

        /// <summary>
        /// Indicates a suspicious or unexpected state that is recoverable.
        /// </summary>
        Warning,

        /// <summary>
        /// Indicates a broken or failed state.
        /// </summary>
        Error
    }
}
