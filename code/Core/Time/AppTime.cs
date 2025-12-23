using System.Diagnostics;

namespace CosmosCasino.Core.Time
{
    /// <summary>
    /// Provides monotonic application time.
    /// </summary>
    public static class AppTime
    {
        #region FIELDS

        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Milliseconds elapsed since application start.
        /// Guaranteed to be monotonic.
        /// </summary>
        public static long ElapsedMs => _stopwatch.ElapsedMilliseconds;

        #endregion
    }
}