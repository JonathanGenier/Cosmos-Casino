namespace CosmosCasino.Core.Console.Logging
{
    /// <summary>
    /// Defines high-level display categories used by the console
    /// to determine how log entries are visually presented.
    /// This enum represents presentation intent only and is intentionally
    /// decoupled from internal log severity, safety, or semantic classification.
    /// </summary>
    public enum ConsoleLogDisplayKind
    {
        /// <summary>
        /// Default display style for general informational messages.
        /// </summary>
        General,

        /// <summary>
        /// Display style for gameplay or runtime events of interest.
        /// </summary>
        Event,
        /// <summary>
        /// Display style for system-level or engine-related messages.
        /// </summary>
        System,

        /// <summary>
        /// De-emphasized display style for low-priority or verbose output.
        /// </summary>
        Muted,

        /// <summary>
        /// Display style for warnings that indicate potential issues
        /// without halting execution.
        /// </summary>
        Warning,

        /// <summary>
        /// Display style for errors indicating failures or critical issues.
        /// </summary>
        Error,
    }
}
