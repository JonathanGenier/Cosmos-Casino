using CosmosCasino.Core.Console.Logging;
using Godot;

/// <summary>
/// Controls the behavior of the main menu scene and handles
/// user input that triggers high-level application state changes.
/// </summary>
public sealed partial class MainMenuScene : SceneController
{
    #region FIELDS

    private Button _playButton;

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the main menu scene and wires up user interface
    /// event handlers when the node becomes ready.
    /// </summary>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(MainMenuScene)))
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

    /// <summary>
    /// Handles the Play button action and transitions the application
    /// into the loading state to begin a new game session.
    /// </summary>
    private void OnPlayPressed()
    {
        AppManager.Instance.ChangeState(AppState.Loading);
    }

    #endregion
}
