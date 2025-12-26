using CosmosCasino.Core.Debug.Logging;
using Godot;

/// <summary>
/// Controls the behavior of the main menu scene and handles
/// user input that triggers high-level application state changes.
/// </summary>
internal partial class MainMenuScene : SceneController
{
    #region GODOT METHODS

    /// <summary>
    /// Initializes the main menu scene and wires up user interface
    /// event handlers when the node becomes ready.
    /// </summary>
    public override void _Ready()
    {
        var playButton = GetNode<Button>("Button_Play");
        playButton.Pressed += OnPlayPressed;
        DevLog.System("MainMenuController", "Ready");
    }

    #endregion

    #region CALLBACKS

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
