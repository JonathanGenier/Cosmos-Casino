using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Services;
using System.Threading.Tasks;

/// <summary>
/// Controls the loading phase between the main menu and gameplay.
/// <para>
/// <see cref="LoadingScene"/> is responsible for preparing a game session,
/// such as starting a new game or loading an existing save, before transitioning
/// to the gameplay scene.
/// </para>
/// <para>
/// This controller acts as a bridge between the client layer and
/// <see cref="CoreServices"/> during the loading phase only.
/// </para>
/// </summary>
public sealed partial class LoadingScene : SceneController
{
    #region METHODS

    /// <summary>
    /// Entry point for the loading scene.
    /// Begins the asynchronous loading workflow when the node becomes ready.
    /// </summary>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(LoadingScene)))
        {
            _ = LoadAsync();
        }
    }

    /// <summary>
    /// Executes the asynchronous loading workflow.
    /// <para>
    /// This method determines whether a new game should be started or an
    /// existing save should be loaded, initializes core game systems,
    /// and prepares the application for gameplay.
    /// </para>
    /// <para>
    /// Once loading is complete, control is handed back to
    /// <see cref="AppManager"/> to transition the application to the
    /// gameplay state.
    /// </para>
    /// </summary>
    private async Task LoadAsync()
    {
        // Decide new game vs load game
        // Later:
        // core.Game.LoadSave(...)
        // core.Game.InitializeWorld()

        AppManager.Instance.CallDeferred(
            nameof(AppManager.ChangeState),
            (int)AppState.Game
        );
    }

    #endregion
}
