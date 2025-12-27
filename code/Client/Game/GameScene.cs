using CosmosCasino.Core.Console.Logging;

/// <summary>
/// Entry point for the gameplay scene.
/// <para>
/// <see cref="GameScene"/> is responsible for wiring the gameplay scene
/// to the core game logic once a game session has been initialized.
/// </para>
/// <para>
/// This controller should not contain core gameplay rules; instead, it
/// coordinates scene-level setup, node references, and forwards input or
/// events to the core <c>GameManager</c>.
/// </para>
/// </summary>
public sealed partial class GameScene : SceneController
{
    #region METHODS

    /// <summary>
    /// Called when the gameplay scene has finished loading and is ready.
    /// </summary>
    public override void _Ready()
    {
        ConsoleLog.System("GameController", "Ready");
    }

    #endregion
}

