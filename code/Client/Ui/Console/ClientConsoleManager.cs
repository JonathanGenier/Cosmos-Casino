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

    private ConsoleUi _consoleUi;
    private ConsoleAdapter _consoleAdapter;

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the client-side debug console.
    /// Instantiates and attaches the console UI, wires it to the core
    /// console manager via a presentation adapter, and subscribes to
    /// debug input intents required to toggle console visibility.
    /// </summary>
    /// <inheritdoc/>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(ClientConsoleManager)))
        {
            _consoleUi = AddOwnedNode(GD.Load<PackedScene>(UiPaths.Console).Instantiate<ConsoleUi>(), nameof(ConsoleUi));
            _consoleUi.Toggle();
            _consoleAdapter = new ConsoleAdapter(_consoleUi, CoreServices.ConsoleManager);

            var input = ClientServices.InputManager;
            input.ToggleConsoleUi += OnToggleConsoleUi;
        }
    }

    /// <summary>
    /// Cleans up client-side debug console resources.
    /// Unsubscribes from input intent signals and disposes the console
    /// adapter to release event subscriptions bridging core and UI layers.
    /// </summary>
    /// <inheritdoc/>
    public override void _ExitTree()
    {
        var input = ClientServices.InputManager;
        input.ToggleConsoleUi -= OnToggleConsoleUi;
        _consoleAdapter.Dispose();
    }

    /// <summary>
    /// Handles a request to toggle the visibility of the debug log console UI.
    /// Invoked in response to a debug-related input intent.
    /// </summary>
    private void OnToggleConsoleUi()
    {
        _consoleUi.Toggle();
    }

    #endregion
}
