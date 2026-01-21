using CosmosCasino.Core.Application.Services;
using Godot;
using System;

/// <summary>
/// Manages the application's core lifecycle, state transitions, and service initialization. Provides centralized
/// control over application startup, shutdown, and scene management.
/// </summary>
/// <remarks>AppManager acts as the main entry point for application logic, coordinating the initialization of
/// core and application services, handling transitions between application states (such as main menu and game), and
/// managing the application's shutdown process. This class is designed to be used as a singleton within the scene tree;
/// only one instance should exist at any time. AppManager extends NodeManager and integrates with the Godot engine's
/// node lifecycle methods to ensure proper resource management and application flow.</remarks>
public sealed partial class AppManager : NodeManager
{
    #region Fields
    private static AppManager? _instance;
    private bool _isShutdown;

    private AppServices? _appServices;
    private CoreServices? _coreServices;
    private AppUiManager? _appUiManager;
    private MainMenuManager? _mainMenuManager;
    private GameManager? _gameManager;
    private SceneLoader? _sceneLoader;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current state of the application lifecycle.
    /// </summary>
    public AppState State { get; private set; } = AppState.Boot;

    private AppServices AppServices
    {
        get => _appServices ?? throw new InvalidOperationException($"{nameof(AppServices)} is not initialized.");
        set => _appServices = value;
    }

    private CoreServices CoreServices
    {
        get => _coreServices ?? throw new InvalidOperationException($"{nameof(CoreServices)} is not initialized.");
        set => _coreServices = value;
    }

    private AppUiManager AppUiManager
    {
        get => _appUiManager ?? throw new InvalidOperationException($"{nameof(AppUiManager)} is not initialized.");
        set => _appUiManager = value;
    }

    private MainMenuManager MainMenuManager
    {
        get => _mainMenuManager ?? throw new InvalidOperationException($"{nameof(MainMenuManager)} is not initialized.");
        set => _mainMenuManager = value;
    }

    private GameManager GameManager
    {
        get => _gameManager ?? throw new InvalidOperationException($"{nameof(GameManager)} is not initialized.");
        set => _gameManager = value;
    }

    private SceneLoader SceneLoader
    {
        get => _sceneLoader ?? throw new InvalidOperationException($"{nameof(SceneLoader)} is not initialized.");
        set => _sceneLoader = value;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes the singleton instance of the AppManager when the node enters the scene tree.
    /// </summary>
    /// <remarks>This method is called automatically by the Godot engine when the node is added to the scene
    /// tree. It ensures that only one instance of AppManager exists at any time. Attempting to add a second instance
    /// will result in an exception.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if an instance of AppManager already exists in the scene tree.</exception>
    public override void _EnterTree()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException($"An instances of {nameof(AppManager)} already exists.");
        }

        _instance = this;
    }

    /// <summary>
    /// Called by the engine when the node is added to the scene for the first time. Use this method to perform
    /// initialization that requires the node to be part of the scene tree.
    /// </summary>
    /// <remarks>This method overrides the base implementation to defer the execution of the Boot method until
    /// after the current frame. This ensures that all nodes are fully initialized before Boot is called.</remarks>
    public override void _Ready()
    {
        CallDeferred(nameof(Boot));
    }

    /// <summary>
    /// Performs cleanup operations when the node is removed from the scene tree.
    /// </summary>
    /// <remarks>This method disposes of core services and should be called when the node is exiting the scene
    /// tree to release resources. Overrides the base implementation to ensure proper resource management.</remarks>
    public override void _ExitTree()
    {
        CoreServices.Dispose();
        base._ExitTree();
    }

    #endregion

    #region AppState

    /// <summary>
    /// Changes the application state to the specified state value.
    /// </summary>
    /// <param name="newState">The integer value representing the new application state. Must correspond to a valid value of the AppState
    /// enumeration.</param>
    private void ChangeState(int newState)
    {
        ChangeState((AppState)newState);
    }

    /// <summary>
    /// Changes the current application state to the specified state and returns the associated scene node if the
    /// transition is successful.
    /// </summary>
    /// <param name="newState">The new application state to transition to.</param>
    /// <returns>A <see cref="Node"/> representing the scene associated with the new state if the transition occurs; otherwise,
    /// <see langword="null"/> if the state is unchanged or the scene could not be loaded.</returns>
    private Node ChangeState(AppState newState)
    {
        if (State == newState)
        {
            return null!;
        }

        Node scene = ChangeScene(newState);

        if (scene == null)
        {
            return null!;
        }

        var previousState = State;
        State = newState;

        return scene;
    }

    /// <summary>
    /// Gets the scene path associated with the specified application state.
    /// </summary>
    /// <param name="state">The application state for which to retrieve the corresponding scene path.</param>
    /// <returns>The path to the scene corresponding to the specified state. Returns an empty string if the state does not have
    /// an associated scene.</returns>
    private string GetScenePathForState(AppState state)
    {
        return state switch
        {
            AppState.MainMenu => ScenePaths.MainMenu,
            AppState.Game => ScenePaths.Game,
            _ => string.Empty
        };
    }

    /// <summary>
    /// Loads and returns the scene corresponding to the specified application state.
    /// </summary>
    /// <param name="state">The application state for which to load the corresponding scene.</param>
    /// <returns>A <see cref="Node"/> representing the loaded scene for the specified state.</returns>
    private Node ChangeScene(AppState state)
    {
        var path = GetScenePathForState(state);
        return SceneLoader.Load(path);
    }

    #endregion

    #region App Lifecycle

    /// <summary>
    /// Initializes core and application services and prepares the application for user interaction.
    /// </summary>
    /// <remarks>This method sets up essential services and user interface components required at application
    /// startup. It should be called once during the application's initialization phase before any user interaction
    /// occurs.</remarks>
    private void Boot()
    {
        InitializeCoreServices();
        InitializeAppServices();

        AppUiManager = AddInitializableNode<AppUiManager>(
            aum => aum.Initialize(AppServices.InputManager, CoreServices.ConsoleManager));

        SceneLoader = new SceneLoader(GetTree());
        StartMainMenu();
    }

    /// <summary>
    /// Initializes and displays the main menu by creating and configuring the main menu manager.
    /// </summary>
    /// <remarks>This method should be called only once during the application's lifecycle to ensure proper
    /// initialization of the main menu. Subsequent calls will result in an exception.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the main menu manager already exists, or if the main menu scene root does not have a MainMenuManager
    /// attached.</exception>
    private void StartMainMenu()
    {
        var scene = ChangeState(AppState.MainMenu);
        MainMenuManager = scene as MainMenuManager ?? throw new InvalidOperationException($"MainMenu scene root does not have {nameof(MainMenuManager)} attached.");
        MainMenuManager.Initialize(StartGame);
    }

    /// <summary>
    /// Initializes and starts a new game session. Sets up the game manager and transitions the application to the game
    /// state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if a game session is already in progress or if the game scene root does not have a GameManager attached.</exception>
    private void StartGame()
    {
        var scene = ChangeState(AppState.Game);
        GameManager = scene as GameManager ?? throw new InvalidOperationException($"Game scene root does not have {nameof(GameManager)} attached.");
        GameManager.StartNewGame(AppServices);
    }

    /// <summary>
    /// Loads the saved game data into the current game session.
    /// </summary>
    private void LoadGame()
    {
        // TODO: Load saved data
    }

    /// <summary>
    /// Ends the current game session and returns the application to the main menu.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the game manager does not exist when attempting to end the game.</exception>
    private void EndGame()
    {
        GameManager.QueueFree();
        ChangeState(AppState.MainMenu);
    }

    /// <summary>
    /// Shuts down the application and releases all associated resources.
    /// </summary>
    /// <remarks>This method initiates the shutdown process for the application. After calling this method,
    /// the application will begin terminating and no further operations should be performed. Multiple calls to this
    /// method have no effect after the first invocation.</remarks>
    private void Shutdown()
    {
        if (_isShutdown)
        {
            return;
        }

        _isShutdown = true;
        CoreServices.Shutdown();

        GetTree().Quit();
    }

    #endregion

    #region Services

    /// <summary>
    /// Initializes the core services required by the application. This method must be called before accessing core
    /// service functionality.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the core services have already been initialized.</exception>
    private void InitializeCoreServices()
    {
        string savePath = OS.GetUserDataDir();
        CoreServices = new CoreServices(savePath);
    }

    /// <summary>
    /// Initializes the application services for the current instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the application services have already been initialized.</exception>
    private void InitializeAppServices()
    {
        AppServices = AddNode<AppServices>();
    }

    #endregion
}