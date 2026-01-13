namespace CosmosCasino.Core.Application.Console.Logging
{
    /// <summary>
    /// Describes the semantic intent of a log entry.
    /// Used primarily for presentation and grouping.
    /// </summary>
    internal enum ConsoleLogKind
    {
        /// <summary>
        /// General-purpose log entry.
        /// This is the default kind used for normal application flow
        /// and informational diagnostics.
        /// </summary>
        General,

        /// <summary>
        /// Indicates that a discrete, meaningful event occurred.
        /// Events typically represent game transitions,
        /// state changes, or notable world actions.
        /// </summary>
        Event,

        /// <summary>
        /// Indicates a low-level system or infrastructure log entry.
        /// Used for engine, application lifecycle, and service-level
        /// diagnostics that are not part of normal gameplay flow.
        /// </summary>
        System,
    }
}
