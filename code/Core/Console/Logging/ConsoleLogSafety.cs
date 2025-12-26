namespace CosmosCasino.Core.Console.Logging
{
    /// <summary>
    /// Indicates whether a log entry is safe to be visible in production.
    /// </summary>
    public enum ConsoleLogSafety
    {
        /// <summary>
        /// Safe to include in production builds.
        /// </summary>
        Safe,

        /// <summary>
        /// Developer-only information. Must not appear in production.
        /// </summary>
        Unsafe
    }
}
