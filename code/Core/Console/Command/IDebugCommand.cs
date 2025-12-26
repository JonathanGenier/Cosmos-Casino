namespace CosmosCasino.Core.Debug.Command
{
    /// <summary>
    /// Defines a debug console command that can be executed at runtime.
    /// Implementations provide command metadata and execution logic
    /// for developer tooling and diagnostics.
    /// </summary>
    public interface IDebugCommand
    {
        /// <summary>
        /// The unique identifier used to invoke the command from the
        /// debug console (e.g. <c>"log.clear"</c>).
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Short human-readable description of the command’s purpose.
        /// Displayed in help output or diagnostic listings.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Indicates whether the command is safe to execute in
        /// production environments.
        /// </summary>
        CommandSafety Safety { get; }

        /// <summary>
        /// Executes the command using the provided arguments.
        /// </summary>
        /// <param name="args">
        /// Arguments parsed from console input, excluding the command name.
        /// </param>
        /// <returns>
        /// The result of command execution, including success state
        /// and optional feedback message.
        /// </returns>
        CommandResult Execute(string[] args);
    }
}
