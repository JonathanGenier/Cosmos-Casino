namespace CosmosCasino.Core.Console.Command
{
    /// <summary>
    /// Internal registry responsible for storing, validating, and executing
    /// debug console commands.
    /// Acts as the single source of truth for command lookup and dispatch.
    /// </summary>
    internal sealed class ConsoleCommandRegistry
    {
        #region FIELDS

        private readonly Dictionary<string, IConsoleCommand> _commands = new(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes the command registry and registers all available
        /// debug commands.
        /// </summary>
        /// <param name="debugConsole">
        /// The debug console providing dependencies required by commands.
        /// </param>
        internal ConsoleCommandRegistry(ConsoleManager debugConsole)
        {
            ArgumentNullException.ThrowIfNull(debugConsole);

            ConsoleCommandRegistrar.RegisterAll(Register, debugConsole);
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Parses and executes a debug command input string.
        /// </summary>
        /// <param name="input">
        /// Raw command input where the first token represents the command
        /// identifier and remaining tokens are treated as arguments.
        /// </param>
        /// <returns>
        /// The result of command execution, including success state
        /// and user-facing feedback.
        /// </returns>
        internal ConsoleCommandResult Execute(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                return ConsoleCommandResult.Failed("Empty command");
            }

            var key = parts[0];
            var args = parts.Skip(1).ToArray();

            if (!_commands.TryGetValue(key, out var command))
            {
                return ConsoleCommandResult.Failed($"Unknown command: {key}");
            }

            var result = command.Execute(args);

            if (string.IsNullOrWhiteSpace(result.Message))
            {
                var outcome = result.Success ? "succeeded" : "failed";
                var message = $"Command '{command.Command}' {outcome} but returned no feedback.";

                return result.Success
                    ? ConsoleCommandResult.Ok(message)
                    : ConsoleCommandResult.Failed(message);
            }

            return result;
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Registers a debug command with the registry.
        /// </summary>
        /// <param name="command">
        /// The command instance to register.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a command with the same identifier
        /// has already been registered.
        /// </exception>
        private void Register(IConsoleCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentException.ThrowIfNullOrWhiteSpace(command.Command);
            ArgumentException.ThrowIfNullOrWhiteSpace(command.Description);

#if !DEBUG
            if (command.Safety == ConsoleCommandSafety.Unsafe)
            {
                return;
            }
#endif

            if (!_commands.TryAdd(command.Command, command))
            {
                throw new InvalidOperationException(
                    $"Duplicate command registration: '{command.Command}'.");
            }
        }

        #endregion
    }
}
