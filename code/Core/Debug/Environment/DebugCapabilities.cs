namespace CosmosCasino.Core.Debug.Environment
{
    /// <summary>
    /// Defines which debug features are allowed in the current environment.
    /// Capabilities are about SAFETY, not UI visibility.
    /// </summary>
    public static class DebugCapabilities
    {
        /// <summary>
        /// Debug stats overlay (FPS, counts, cursor context).
        /// Safe to show in all environments.
        /// </summary>
        public static bool CanViewDebugOverlay => true;

        /// <summary>
        /// Verbose developer-only logs (internal reasoning).
        /// Not allowed in production.
        /// </summary>
        public static bool CanViewVerboseLogs => AppEnvironment.IsDev;

        /// <summary>
        /// Whether unsafe (state-mutating) debug commands may execute.
        /// Dev only.
        /// </summary>
        public static bool CanExecuteUnsafeCommands => AppEnvironment.IsDev;

        /// <summary>
        /// Whether safe diagnostic commands may execute.
        /// Allowed in all environments.
        /// </summary>
        public static bool CanExecuteSafeCommands => true;
    }
}