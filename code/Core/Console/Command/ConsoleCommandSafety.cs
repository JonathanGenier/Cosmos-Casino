namespace CosmosCasino.Core.Console.Command
{
    /// <summary>
    /// Defines whether a debug command is permitted to run in
    /// production environments.
    /// </summary>
    public enum ConsoleCommandSafety
    {
        /// <summary>
        /// The command is safe to execute in all environments,
        /// including production builds.
        /// </summary>
        Safe,

        /// <summary>
        /// The command is intended for development or debugging only
        /// and is excluded from production builds.
        /// </summary>
        Unsafe,
    }
}
