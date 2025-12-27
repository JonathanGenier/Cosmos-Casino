using System.Diagnostics;

namespace CosmosCasino.Core.Time
{
    /// <summary>
    /// Provides monotonic application time.
    /// </summary>
    internal static class AppTime
    {
        #region FIELDS

        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Milliseconds elapsed since application start.
        /// Guaranteed to be monotonic.
        /// </summary>
        internal static long ElapsedMs => _stopwatch.ElapsedMilliseconds;

        #endregion
    }
}