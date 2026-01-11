using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Services;
using Godot;
using System;

/// <summary>
/// Coordinates game session lifecycle, runtime managers, input flows,
/// build execution, UI, camera control, and state transitions.
/// </summary>
public sealed partial class GameManager : NodeManager
{
    #region Fields

    private readonly BuildContext _buildContext = new();

    private CoreServices _coreServices;
    private AppServices _appServices;

    private CameraManager _cameraManager;
    private ClientBuildManager _clientBuildManager;
    private GameUiManager _gameUiManager;
    private InteractionManager _interactionManager;
    private SpawnManager _spawnManager;
    private CursorManager _cursorManager;

    private CameraInputFlow _cameraInputFlow;
    private BuildContextFlow _buildContextFlow;
    private BuildRequestFlow _buildRequestFlow;
    private BuildSpawnFlow _buildSpawnFlow;

    private ResourcePreloader _spawnResourcesPreloader;
#if DEBUG
    private CursorDebugVisualizer _cursorDebugVisualizer;
#endif

    private bool _isReady = false;
    private bool _isInitialized = false;

    #endregion

    #region Events

    /// <summary>
    /// Invoked when the active game session ends.
    /// </summary>
    private Action _endGame;

    /// <summary>
    /// Invoked to shut down the application.
    /// </summary>
    private Action _shutdownApp;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current high-level game state.
    /// </summary>
    public GameState State { get; private set; } = GameState.Loading;

    #endregion

    #region Godot Process

    /// <summary>
    /// Starts a new game session using the specified core and application services, and sets up callbacks for ending
    /// the game or shutting down the application.
    /// </summary>
    /// <param name="coreServices">The core services required to initialize and run the game. Cannot be null.</param>
    /// <param name="appServices">The application-level services used during the game session. Cannot be null.</param>
    /// <param name="endGame">An action to invoke when the game session ends. Cannot be null.</param>
    /// <param name="shutdownApp">An action to invoke to shut down the application. Cannot be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if the game is not ready to start a new session.</exception>
    public void StartNewGame(CoreServices coreServices, AppServices appServices, Action endGame, Action shutdownApp)
    {
        if (!_isReady)
        {
            throw new InvalidOperationException();
        }

        Initialize(coreServices, appServices, endGame, shutdownApp);
        StartGame();
    }

    /// <summary>
    /// Initializes and starts a new game session using the specified core and application services, and sets up
    /// callbacks for ending the game and shutting down the application.
    /// </summary>
    /// <param name="coreServices">The core services required for game initialization and runtime operations. Cannot be null.</param>
    /// <param name="appServices">The application-level services used during the game session. Cannot be null.</param>
    /// <param name="endGame">An action to invoke when the game session ends. Cannot be null.</param>
    /// <param name="shutdownApp">An action to invoke to shut down the application. Cannot be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if the game is not ready to be loaded.</exception>
    public void LoadGame(CoreServices coreServices, AppServices appServices, Action endGame, Action shutdownApp)
    {
        if (!_isReady)
        {
            throw new InvalidOperationException();
        }

        Initialize(coreServices, appServices, endGame, shutdownApp);
        // Load map
        // Load Entities
        StartGame();
    }

    /// <summary>
    /// Called when the node enters the scene tree for the first time. Performs initialization logic required before the
    /// node is used.
    /// </summary>
    /// <remarks>This method is typically used to set up references to child nodes, preload resources, or
    /// perform other setup tasks that must occur after the node is added to the scene. Override this method to
    /// implement custom initialization logic. For more information, see the Godot documentation on the _Ready lifecycle
    /// method.</remarks>
    public override void _Ready()
    {
        _spawnResourcesPreloader = GetNode<ResourcePreloader>("SpawnResources");

#if DEBUG
        var cursorVisualizer = GD.Load<PackedScene>("res://scenes/game/debug/cursor_debug_visualizer.tscn");
        _cursorDebugVisualizer = (CursorDebugVisualizer)cursorVisualizer.Instantiate();
        AddChild(_cursorDebugVisualizer);
#endif

        _isReady = true;
    }

    /// <summary>
    /// Performs cleanup operations when the node is removed from the scene tree.
    /// </summary>
    /// <remarks>This method is called automatically by the engine when the node exits the scene tree. It
    /// disposes of managed resources associated with the node to ensure proper resource management. Override this
    /// method to implement additional cleanup logic if necessary.</remarks>
    public override void _ExitTree()
    {
        _buildContextFlow?.Dispose();
        _buildRequestFlow?.Dispose();
        _buildSpawnFlow?.Dispose();
        _cameraInputFlow?.Dispose();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Performs one-time initialization of game subsystems and managers.
    /// Binds injected services, creates manager nodes, and initializes them with their respective dependencies.
    /// </summary>
    /// <param name="coreServices">Core framework services required by lower-level managers. Must not be null.</param>
    /// <param name="appServices">Application-level services (input, ui, etc.) required by higher-level managers. Must not be null.</param>
    /// <param name="endGame">Callback invoked when the current game session ends. Must not be null.</param>
    /// <param name="shutdownApp">Callback invoked to request application shutdown. Must not be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if this method is called more than once on the same instance.</exception>
    private void Initialize(CoreServices coreServices, AppServices appServices, Action endGame, Action shutdownApp)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException("GameManager already initialized.");
        }

        ArgumentNullException.ThrowIfNull(coreServices);
        ArgumentNullException.ThrowIfNull(appServices);
        ArgumentNullException.ThrowIfNull(endGame);
        ArgumentNullException.ThrowIfNull(shutdownApp);

        _coreServices = coreServices;
        _appServices = appServices;
        _endGame = endGame;
        _shutdownApp = shutdownApp;

        _clientBuildManager = AddInitializableNode<ClientBuildManager>(
            cbm => cbm.Initialize(_coreServices.BuildManager));
        _cursorManager = AddInitializableNode<CursorManager>(
            cm => cm.Initialize(CollisionLayers.Buildable));
        _interactionManager = AddInitializableNode<InteractionManager>(
            im => im.Initialize(_appServices.InputManager, _cursorManager, _buildContext));
        _spawnManager = AddInitializableNode<SpawnManager>(
            sm => sm.Initialize(SpawnCatalog.LoadFromResourcePreloader(_spawnResourcesPreloader)));

        _cameraManager = CreateNode<CameraManager>();
        _gameUiManager = CreateInitializableNode<GameUiManager>(
            gum => gum.Initialize());

#if DEBUG
        _cursorDebugVisualizer.Initialize(_cursorManager);
#endif

        _isInitialized = true;
    }

    /// <summary>
    /// Creates and wires the runtime flow objects that mediate interactions between managers.
    /// These flows subscribe to input and manager events and should be disposed when the node exits the tree.
    /// </summary>
    private void InitializeFlows()
    {
        _buildContextFlow = new BuildContextFlow(_gameUiManager.BuildUiManager, _buildContext, _interactionManager);
        _buildRequestFlow = new BuildRequestFlow(_interactionManager, _clientBuildManager);
        _buildSpawnFlow = new BuildSpawnFlow(_clientBuildManager, _spawnManager);
        _cameraInputFlow = new CameraInputFlow(_appServices.InputManager, _cameraManager);
    }

    /// <summary>
    /// Performs the steps necessary to start gameplay:
    /// - Adds manager nodes to the scene tree
    /// - Initializes flow objects
    /// - Sets the initial game state to paused
    /// </summary>
    private void StartGame()
    {
        AddChild(_cameraManager);
        AddChild(_gameUiManager);

        InitializeFlows();
        SetState(GameState.Paused);
    }

    #endregion 

    #region Game State

    /// <summary>
    /// Sets the current high-level game state and notifies interested subsystems (input manager).
    /// If the new state matches the current state this method is a no-op.
    /// </summary>
    /// <param name="newState">The new game state to apply.</param>
    private void SetState(GameState newState)
    {
        if (State == newState)
        {
            return;
        }

        State = newState;
        _appServices.InputManager.OnGameStateChanged(newState);
        ConsoleLog.System(nameof(GameManager), $"GameState → {State}");
    }

    #endregion
}