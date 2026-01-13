namespace CosmosCasino.Core.Application.Console.Logging
{
    /// <summary>
    /// Indicates whether a log entry is safe to be visible in production.
    /// </summary>
    internal enum ConsoleLogSafety
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
