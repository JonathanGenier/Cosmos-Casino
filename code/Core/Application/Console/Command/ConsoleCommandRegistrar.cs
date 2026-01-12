namespace CosmosCasino.Core.Application.Console.Command
{
    /// <summary>
    /// Centralized registration point for all debug console commands.
    /// Responsible for constructing command instances and registering
    /// them with the command registry during console initialization.
    /// </summary>
    internal static class ConsoleCommandRegistrar
    {
        #region METHODS

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
        internal static void RegisterAll(Action<IConsoleCommand> register, ConsoleManager debugConsole)
        {
            register(new ClearConsoleCommand(debugConsole.TryClearLogs));
        }

        #endregion
    }
}