using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Services;
using Godot;
using System;

/// <summary>
/// Central application entry point and state coordinator.
/// <para>
/// <see cref="AppManager"/> is autoloaded and persists for the entire lifetime
/// of the application. It owns the high-level application state and coordinates
/// scene transitions based on that state.
/// </para>
/// <para>
/// This class also acts as the bridge between the Godot client layer and the
/// core (engine-agnostic) services.
/// </para>
/// </summary>
public sealed partial class AppManager : Node
{
    #region FIELDS

    private CoreServices _core;
    private ClientServices _client;
    private bool _isShutdown;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Singleton instance of the application manager.
    /// Guaranteed to be available after the node enters the scene tree.
    /// </summary>
    public static AppManager Instance { get; private set; }

    /// <summary>
    /// Current high-level application state.
    /// </summary>
    public AppState State { get; private set; }

    #endregion

    #region METHODS

    /// <summary>
    /// Ensures a single instance of <see cref="AppManager"/> exists and performs
    /// early application initialization before any scenes are loaded.
    /// </summary>
    public override void _EnterTree()
    {
        if (Instance != null)
        {
            ConsoleLog.Warning("Application", "Multiple instances of AppManager detected. There should only be one instance. No consequences. Deplicate is QueueFree.");
            QueueFree();
            return;
        }

        ConsoleLog.System("AppManager", "Setting up...");
        Instance = this;
        State = AppState.Boot;

        InitializeCoreServices();
        InitializeClientServices();
        ConsoleLog.System("AppManager", "Ready");
    }


    /// <summary>
    /// Performs final application teardown when the manager leaves the scene tree.
    /// This method releases all core-owned disposable resources to ensure
    /// that event subscriptions and other mechanical resources are cleaned
    /// up before the application fully exits.
    /// </summary>
    /// <inheritdoc/>
    public override void _ExitTree()
    {
        _core.Dispose();
        base._ExitTree();
    }

    /// <summary>
    /// Initiates a controlled application shutdown.
    /// This method performs a graceful logical shutdown of core systems
    /// before requesting termination of the Godot scene tree.
    /// Safe to call multiple times.
    /// </summary>
    public void Shutdown()
    {
        if (_isShutdown)
        {
            return;
        }

        _isShutdown = true;
        _core.Shutdown();

        GetTree().Quit();
    }

    /// <summary>
    /// Convenience overload for state changes originating from UI or signals
    /// that provide the target state as an integer value.
    /// </summary>
    /// <param name="newState">Target application state as an integer.</param>
    public void ChangeState(int newState)
    {
        ChangeState((AppState)newState);
    }

    /// <summary>
    /// Changes the current application state and triggers the corresponding
    /// scene transition.
    /// </summary>
    /// <param name="newState">Target application state.</param>
    public void ChangeState(AppState newState)
    {
        if (State == newState)
        {
            return;
        }

        var success = ChangeScene(newState);

        if (success)
        {
            State = newState;
        }
    }

    /// <summary>
    /// Resolves the scene path associated with a given application state.
    /// </summary>
    /// <param name="state">Application state to resolve.</param>
    /// <returns>
    /// Scene path corresponding to the specified state, or an empty string
    /// if the state has no associated scene.
    /// </returns>
    private static string GetScenePathForState(AppState state)
    {
        return state switch
        {
            AppState.MainMenu => ScenePaths.MainMenu,
            AppState.Loading => ScenePaths.Loading,
            AppState.Game => ScenePaths.Game,
            _ => string.Empty
        };
    }

    /// <summary>
    /// Performs the actual scene transition for the given application state.
    /// <para>
    /// This method isolates scene-loading logic from state management and delegates
    /// the loading process to <see cref="SceneLoader"/>.
    /// </para>
    /// </summary>
    /// <param name="state">Application state whose scene should be loaded.</param>
    private bool ChangeScene(AppState state)
    {
        var path = GetScenePathForState(state);
        return SceneLoader.Load(path, _core, _client);
    }

    /// <summary>
    /// Initializes all core services required by the application.
    /// <para>
    /// This method establishes the core layer and is called once during
    /// application startup.
    /// </para>
    /// </summary>
    private void InitializeCoreServices()
    {
        if (_core != null)
        {
            throw new InvalidOperationException("CoreServices already initialized.");
        }

        string savePath = OS.GetUserDataDir();
        _core = new CoreServices(savePath);
    }

    /// <summary>
    /// Initializes all client-side services and presentation-layer systems.
    /// Client services are initialized after core services to ensure the client
    /// layer can safely reference core functionality without creating reverse
    /// dependencies.
    /// </summary>
    private void InitializeClientServices()
    {
        if (_client != null)
        {
            throw new InvalidOperationException("ClientServices already initialized.");
        }

        _client = new ClientServices(_core);
        AddChild(_client);
    }

    #endregion
}
