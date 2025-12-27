using CosmosCasino.Core.Console.Logging;
using Godot;

/// <summary>
/// Coordinates the client-side debug console, including console command submission
/// and log presentation, and bridges UI interactions with the core console.
/// </summary>
/// <param name="bootstrap">
/// Bootstrap context providing access to core and client service collections
/// required for console initialization.
/// </param>
public sealed partial class ClientConsoleManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    #region FIELDS

    /// <summary>
    /// Controller for the debug log console UI.
    /// Managed and coordinated by this manager.
    /// </summary>
    private ConsoleUi _consoleUi;

    /// <summary>
    /// Sink responsible for forwarding log entries from the core logging system
    /// to the in-game debug log console UI.
    /// Acts as a bridge between engine-agnostic logging and client-side presentation.
    /// </summary>
    private ConsoleAdapter _consoleAdapter;

    #endregion

    #region METHODS

    /// <inheritdoc/>
    public override void _Ready()
    {
        ConsoleLog.System("UiManager", "Setting up...");
        InitializeConsoleUi();
        ConsoleLog.System("UiManager", "Ready");
    }

    /// <summary>
    /// Handles a request to toggle the visibility of the debug log console UI.
    /// Invoked in response to a debug-related input intent.
    /// </summary>
    private void OnToggleConsoleUi()
    {
        _consoleUi.Toggle();
    }

    private void OnCommandSubmitted(string command)
    {
        _consoleAdapter.AppendCommand(command);
        var result = CoreServices.ConsoleManager.ExecuteCommand(command);
        _consoleAdapter.AppendCommandResult(result);
    }

    /// <summary>
    /// Instantiates and attaches the debug log console UI to the scene tree.
    /// The console is created once and retained for the lifetime of the client
    /// layer, with visibility managed independently of its existence.
    /// </summary>
    private void InitializeConsoleUi()
    {
        _consoleUi = AddOwnedNode(GD.Load<PackedScene>(UiPaths.Console).Instantiate<ConsoleUi>());
        _consoleUi.Toggle();
        _consoleAdapter = new ConsoleAdapter(_consoleUi, CoreServices.ConsoleManager);

        var input = ClientServices.InputManager;
        input.ToggleConsoleUi += OnToggleConsoleUi;
        _consoleUi.CommandSubmitted += OnCommandSubmitted;
    }

    #endregion
}
