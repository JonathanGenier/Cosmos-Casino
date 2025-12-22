namespace CosmosCasino.Core.Debug.Logging
{
    /// <summary>
    /// Describes the semantic intent of a log entry.
    /// Used primarily for presentation and grouping.
    /// </summary>
    public enum LogKind
    {
        /// <summary>
        /// General-purpose log entry. This is the default kind.
        /// </summary>
        General,

        /// <summary>
        /// Indicates that a discrete, meaningful event occurred.
        /// </summary>
        Event,

        /// <summary>
        /// Indicates that a user command was entered or executed.
        /// </summary>
        Command
    }
}
