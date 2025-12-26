using CosmosCasino.Core.Debug.Logging;
using CosmosCasino.Core.Services;
using Godot;

/// <summary>
/// Central coordinator for client-side user interface systems.
/// <see cref="UiManager"/> acts as the composition root for UI controllers,
/// owning their lifecycle and coordinating responses to UI-related input
/// intents without directly handling input polling or UI rendering logic.
/// This class is responsible for instantiating long-lived UI elements
/// (such as debug overlays or menus) and delegating visibility or behavior
/// changes to their respective controllers.
/// </summary>
internal partial class UiManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    #region FIELDS

    /// <summary>
    /// Controller for the debug log console UI.
    /// Managed and coordinated by this manager.
    /// </summary>
    private LogConsoleUi _logConsole;

    /// <summary>
    /// Sink responsible for forwarding log entries from the core logging system
    /// to the in-game debug log console UI.
    /// Acts as a bridge between engine-agnostic logging and client-side presentation.
    /// </summary>
    private LogConsoleSink _consoleSink;

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Initializes UI systems owned by this manager.
    /// UI elements created here are guaranteed to exist before any
    /// input-driven UI coordination occurs.
    /// </summary>
    public override void _Ready()
    {
        DevLog.System("UiManager", "Setting up...");
        InitializeLogConsoleUi();
        DevLog.System("UiManager", "Ready");
    }

    #endregion

    #region CALLBACKS

    /// <summary>
    /// Handles a request to toggle the visibility of the debug log console UI.
    /// Invoked in response to a debug-related input intent.
    /// </summary>
    private void OnToggleConsoleUi()
    {
        _logConsole.Toggle();
    }

    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Instantiates and attaches the debug log console UI to the scene tree.
    /// The console is created once and retained for the lifetime of the client
    /// layer, with visibility managed independently of its existence.
    /// </summary>
    private void InitializeLogConsoleUi()
    {
        _logConsole = GD.Load<PackedScene>(UiPaths.LogConsole).Instantiate<LogConsoleUi>();
        AddChild(_logConsole);
        _logConsole.Toggle();

        _consoleSink = new LogConsoleSink(_logConsole, CoreServices.DebugConsole);

        var input = ClientServices.InputManager;
        input.ToggleConsoleUi += OnToggleConsoleUi;
    }

    #endregion
}