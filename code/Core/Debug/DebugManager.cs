using CosmosCasino.Core.Debug.Logging;

namespace CosmosCasino.Core.Debug
{
    /// <summary>
    /// Initializes and owns core debugging infrastructure for the application.
    /// Responsible for constructing the debug console and coordinating
    /// debug-related system startup.
    /// </summary>
    public sealed class DebugManager
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Creates and initializes the debug subsystem.
        /// Sets up the debug console and emits system-level
        /// lifecycle logs during initialization.
        /// </summary>
        public DebugManager()
        {
            DevLog.System("DebugManager", "Setting up...");
            DebugConsole = new DebugConsole();
            DevLog.System("DebugManager", "Ready");
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The active debug console instance used to collect logs
        /// and execute debug commands.
        /// </summary>
        public DebugConsole DebugConsole { get; init; }

        #endregion
    }
}
