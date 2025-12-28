using CosmosCasino.Core.Console.Logging;
using Godot;
using System.Threading.Tasks;

/// <summary>
/// Controls the application boot sequence.
/// <para>
/// <see cref="BootScene"/> is attached to the boot scene and is responsible
/// for executing one-time startup tasks such as configuration loading,
/// resource warm-up, and cache initialization.
/// </para>
/// <para>
/// Once boot operations are complete, control is handed back to
/// <see cref="AppManager"/> to transition the application to the next state.
/// </para>
/// </summary>
public sealed partial class BootScene : SceneController
{
    #region METHODS

    /// <summary>
    /// Entry point for the boot scene.
    /// Starts the asynchronous boot sequence when the node becomes ready.
    /// </summary>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(BootScene)))
        {
            _ = BootAsync();
        }
    }

    /// <summary>
    /// Executes the asynchronous boot workflow.
    /// <para>
    /// This method is intended to perform all non-gameplay initialization
    /// required before the application becomes interactive, such as
    /// loading configuration files, user preferences, localization data,
    /// and preloading critical resources.
    /// </para>
    /// <para>
    /// Upon completion, the application state is transitioned to
    /// <see cref="AppState.MainMenu"/>.
    /// </para>
    /// </summary>
    private async Task BootAsync()
    {
        // LoadConfig();
        // LoadUserPreferences();
        // LoadLocalization();
        // LoadFeatureFlags();
        // CheckBuildVersion();
        // await WarmUpShaders();
        // await PreloadResources();
        // InitializeCaches();

        // Deferred to avoid scene tree modification during initialization
        AppManager.Instance.CallDeferred(
            nameof(AppManager.ChangeState),
            (int)AppState.MainMenu
        );
    }

    #endregion
}
