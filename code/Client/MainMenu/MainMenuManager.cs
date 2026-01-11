using CosmosCasino.Core.Console.Logging;
using Godot;
using System;

/// <summary>
/// Controls the behavior of the main menu scene and handles
/// user input that triggers high-level application state changes.
/// </summary>
public sealed partial class MainMenuManager : NodeManager
{
    #region Fields

    private Button _playButton;

    #endregion

    #region Events

    private Action _startNewGame;
    private Action _loadGame;
    private Action _shutdown;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the callbacks used to start a new game, load an existing game, and shut down the application.
    /// </summary>
    /// <param name="startNewGame">The action to invoke when starting a new game. Cannot be null.</param>
    /// <param name="loadGame">The action to invoke when loading an existing game. Cannot be null.</param>
    /// <param name="shutdown">The action to invoke when shutting down the application. Cannot be null.</param>
    public void Initialize(Action startNewGame, Action loadGame, Action shutdown)
    {
        _startNewGame = startNewGame;
        _loadGame = loadGame;
        _shutdown = shutdown;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes the main menu scene and wires up user interface
    /// event handlers when the node becomes ready.
    /// </summary>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(MainMenuManager)))
        {
            _playButton = GetNode<Button>("Menu/PlayButton");
            _playButton.Pressed += OnPlayPressed;
        }
    }

    /// <summary>
    /// Disconnects UI signal bindings when this node is removed from the
    /// scene tree to prevent dangling references and duplicate callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        _playButton.Pressed -= OnPlayPressed;
    }

    #endregion

    #region Buttons Methods

    /// <summary>
    /// Handles the Play button action and transitions the application
    /// into the loading state to begin a new game session.
    /// </summary>
    private void OnPlayPressed()
    {
        _startNewGame();
    }

    #endregion
}
