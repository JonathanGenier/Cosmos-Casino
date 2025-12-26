using CosmosCasino.Core.Debug.Command.Commands.Log;

namespace CosmosCasino.Core.Debug.Command
{
    /// <summary>
    /// Centralized registration point for all debug console commands.
    /// Responsible for constructing command instances and registering
    /// them with the command registry during console initialization.
    /// </summary>
    internal static class DebugCommandRegistrar
    {
        /// <summary>
        /// Registers all available debug commands.
        /// </summary>
        /// <param name="register">
        /// Callback used to register a command instance with the
        /// command registry.
        /// </param>
        /// <param name="debugConsole">
        /// The active debug console used to supply required
        /// command dependencies.
        /// </param>
        internal static void RegisterAll(Action<IDebugCommand> register, DebugConsole debugConsole)
        {
            register(new LogClearCommand(debugConsole.TryClearLogs));
        }
    }
}
