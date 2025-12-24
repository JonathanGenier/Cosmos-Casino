using CosmosCasino.Core.Debug.Logging;
using CosmosCasino.Core.Services;
using Godot;
using System.Threading.Tasks;


/// <summary>
/// Controls the loading phase between the main menu and gameplay.
/// <para>
/// <see cref="LoadingController"/> is responsible for preparing a game session,
/// such as starting a new game or loading an existing save, before transitioning
/// to the gameplay scene.
/// </para>
/// <para>
/// This controller acts as a bridge between the client layer and
/// <see cref="CoreServices"/> during the loading phase only.
/// </para>
/// </summary>
public partial class LoadingController : Node
{
    #region PUBLIC METHODS

    /// <summary>
    /// Entry point for the loading scene.
    /// Begins the asynchronous loading workflow when the node becomes ready.
    /// </summary>
    public override void _Ready()
    {
        _ = LoadAsync();
        DevLog.System("LoadingController", "Ready");
    }

    #endregion

    #region PRIVATE METHODS

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
        var coreServices = AppManager.Instance.CoreServices;

        DevLog.System("LoadingController", "Loading...");

        // Decide new game vs load game
        StartNewGame(coreServices);

        // Later:
        // core.Game.LoadSave(...)
        // core.Game.InitializeWorld()

        DevLog.System("LoadingController", "Loading complete");

        AppManager.Instance.CallDeferred(
            nameof(AppManager.ChangeState),
            (int)AppState.Game
        );
    }

    /// <summary>
    /// Starts a new game session using the provided core services.
    /// </summary>
    /// <param name="coreServices">
    /// Core services container used to initialize game-level systems.
    /// </param>
    private void StartNewGame(CoreServices coreServices)
    {
        var success = coreServices.StartNewGame();

        if (!success)
        {
            DevLog.Error("Game", "Could not start a new game,");
            return;
        }

        // Do something...
    }

    #endregion
}
