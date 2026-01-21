using Godot;
using System;

/// <summary>
/// Controls the behavior of the main menu scene and handles
/// user input that triggers high-level application state changes.
/// </summary>
public sealed partial class MainMenuManager : NodeManager
{
    #region Fields

    private Button? _playButton;

    #endregion

    #region Events

    private Action? _startNewGame;

    #endregion

    #region Properties

    private Button PlayButton
    {
        get => _playButton ?? throw new InvalidOperationException($"{nameof(Button)} is not initialized.");
        set => _playButton = value;
    }

    private Action StartNewGame
    {
        get => _startNewGame ?? throw new InvalidOperationException("StartNewGame callback is not initialized.");
        set => _startNewGame = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the callbacks used to start a new game, load an existing game, and shut down the application.
    /// </summary>
    /// <param name="startNewGameCallback">The action to invoke when starting a new game. Cannot be null.</param>
    public void Initialize(Action startNewGameCallback)
    {
        StartNewGame = startNewGameCallback;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes the main menu scene and wires up user interface
    /// event handlers when the node becomes ready.
    /// </summary>
    public override void _Ready()
    {
        PlayButton = GetNode<Button>("Menu/PlayButton");
        PlayButton.Pressed += OnPlayPressed;
    }

    /// <summary>
    /// Disconnects UI signal bindings when this node is removed from the
    /// scene tree to prevent dangling references and duplicate callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        PlayButton.Pressed -= OnPlayPressed;
    }

    #endregion

    #region Buttons Methods

    /// <summary>
    /// Handles the Play button action and transitions the application
    /// into the loading state to begin a new game session.
    /// </summary>
    private void OnPlayPressed()
    {
        StartNewGame();
    }

    #endregion
}
