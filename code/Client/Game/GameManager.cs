using CosmosCasino.Core.Game;
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

    private bool _sceneReady = false;
    private bool _sessionInitialized = false;
    private bool _simulationHasStarted = false;

    private AppServices? _appServices;
    private GameSession? _gameSession;
    private CameraManager? _cameraManager;
    private BuildProcessManager? _buildProcessManager;
    private GameUiManager? _gameUiManager;
    private InteractionManager? _interactionManager;
    private SpawnManager? _spawnManager;
    private CursorManager? _cursorManager;
    private CameraInputFlow? _cameraInputFlow;
    private BuildContextFlow? _buildContextFlow;
    private BuildRequestFlow? _buildRequestFlow;
    private BuildSpawnFlow? _buildSpawnFlow;
    private BuildPreviewFlow? _buildPreviewFlow;
    private ResourceAssembler? _resourceAssembler;
#if DEBUG
    private CursorDebugVisualizer? _cursorDebugVisualizer;
#endif

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current high-level game state.
    /// </summary>
    public GameState State { get; private set; } = GameState.Loading;

    private AppServices AppServices
    {
        get => _appServices ?? throw new InvalidOperationException($"{nameof(AppServices)} is not initialized.");
        set => _appServices = value;
    }

    private GameSession GameSession
    {
        get => _gameSession ?? throw new InvalidOperationException($"{nameof(GameSession)} is not initialized.");
        set => _gameSession = value;
    }

    private CameraManager CameraManager
    {
        get => _cameraManager ?? throw new InvalidOperationException($"{nameof(CameraManager)} is not initialized.");
        set => _cameraManager = value;
    }

    private BuildProcessManager BuildProcessManager
    {
        get => _buildProcessManager ?? throw new InvalidOperationException($"{nameof(BuildProcessManager)} is not initialized.");
        set => _buildProcessManager = value;
    }

    private GameUiManager GameUiManager
    {
        get => _gameUiManager ?? throw new InvalidOperationException($"{nameof(GameUiManager)} is not initialized.");
        set => _gameUiManager = value;
    }

    private InteractionManager InteractionManager
    {
        get => _interactionManager ?? throw new InvalidOperationException($"{nameof(InteractionManager)} is not initialized.");
        set => _interactionManager = value;
    }

    private SpawnManager SpawnManager
    {
        get => _spawnManager ?? throw new InvalidOperationException($"{nameof(SpawnManager)} is not initialized.");
        set => _spawnManager = value;
    }

    private CursorManager CursorManager
    {
        get => _cursorManager ?? throw new InvalidOperationException($"{nameof(CursorManager)} is not initialized.");
        set => _cursorManager = value;
    }

    private CameraInputFlow CameraInputFlow
    {
        get => _cameraInputFlow ?? throw new InvalidOperationException($"{nameof(CameraInputFlow)} is not initialized.");
        set => _cameraInputFlow = value;
    }

    private BuildContextFlow BuildContextFlow
    {
        get => _buildContextFlow ?? throw new InvalidOperationException($"{nameof(BuildContextFlow)} is not initialized.");
        set => _buildContextFlow = value;
    }

    private BuildRequestFlow BuildRequestFlow
    {
        get => _buildRequestFlow ?? throw new InvalidOperationException($"{nameof(BuildRequestFlow)} is not initialized.");
        set => _buildRequestFlow = value;
    }

    private BuildSpawnFlow BuildSpawnFlow
    {
        get => _buildSpawnFlow ?? throw new InvalidOperationException($"{nameof(BuildSpawnFlow)} is not initialized.");
        set => _buildSpawnFlow = value;
    }

    private BuildPreviewFlow BuildPreviewFlow
    {
        get => _buildPreviewFlow ?? throw new InvalidOperationException($"{nameof(BuildPreviewFlow)} is not initialized.");
        set => _buildPreviewFlow = value;
    }

    private ResourceAssembler ResourceAssembler
    {
        get => _resourceAssembler ?? throw new InvalidOperationException($"{nameof(ResourceAssembler)} is not initialized.");
        set => _resourceAssembler = value;
    }

#if DEBUG
    private CursorDebugVisualizer CursorDebugVisualizer
    {
        get => _cursorDebugVisualizer ?? throw new InvalidOperationException($"{nameof(CursorDebugVisualizer)} is not initialized.");
        set => _cursorDebugVisualizer = value;
    }
#endif

    #endregion

    #region Godot Process

    /// <summary>
    /// Starts a new game session using the specified core and application services, and sets up callbacks for ending
    /// the game or shutting down the application.
    /// </summary>
    /// <param name="appServices">The application-level services used during the game session. Cannot be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if the game is not ready to start a new session.</exception>
    public void StartNewGame(AppServices appServices)
    {
        if (!_sceneReady)
        {
            throw new InvalidOperationException($"{nameof(GameManager)} scene not ready yet.");
        }

        GameSession = GameSession.CreateNewSession();
        Initialize(appServices);
        StartSimulation();
    }

    /// <summary>
    /// Initializes and starts a new game session using the specified core and application services, and sets up
    /// callbacks for ending the game and shutting down the application.
    /// </summary>
    /// <param name="appServices">The application-level services used during the game session. Cannot be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if the game is not ready to be loaded.</exception>
    public void LoadGame(AppServices appServices)
    {
        if (!_sceneReady)
        {
            throw new InvalidOperationException($"{nameof(GameManager)} scene not ready yet.");
        }

        GameSession = GameSession.LoadSession(); // Pass GameSaveData
        Initialize(appServices);
        // Load map
        // Load Entities
        StartSimulation();
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
        ResourceAssembler = GetNode<ResourceAssembler>("ResourceAssembler");

#if DEBUG
        var cursorVisualizer = GD.Load<PackedScene>("res://scenes/game/debug/cursor_debug_visualizer.tscn");
        CursorDebugVisualizer = (CursorDebugVisualizer)cursorVisualizer.Instantiate();
        AddChild(CursorDebugVisualizer);
#endif

        _sceneReady = true;
    }

    /// <summary>
    /// Performs cleanup operations when the node is removed from the scene tree.
    /// </summary>
    /// <remarks>This method is called automatically by the engine when the node exits the scene tree. It
    /// disposes of managed resources associated with the node to ensure proper resource management. Override this
    /// method to implement additional cleanup logic if necessary.</remarks>
    public override void _ExitTree()
    {
        BuildContextFlow?.Dispose();
        BuildRequestFlow?.Dispose();
        BuildSpawnFlow?.Dispose();
        CameraInputFlow?.Dispose();
        _sessionInitialized = false;
        _simulationHasStarted = false;
        State = GameState.Loading;
    }

    /// <summary>
    /// Performs per-frame processing for the node, updating the preview flow if the scene is ready, the simulation has
    /// started, and the session is initialized.
    /// </summary>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. Used to synchronize updates with the frame rate.</param>
    public override void _Process(double delta)
    {
        if (!_sceneReady || !_simulationHasStarted || !_sessionInitialized)
        {
            return;
        }

        BuildPreviewFlow.Process();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Performs one-time initialization of game subsystems and managers.
    /// Binds injected services, creates manager nodes, and initializes them with their respective dependencies.
    /// </summary>
    /// <param name="appServices">Application-level services (input, ui, etc.) required by higher-level managers. Must not be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if this method is called more than once on the same instance.</exception>
    private void Initialize(AppServices appServices)
    {
        if (_sessionInitialized)
        {
            throw new InvalidOperationException($"{nameof(GameManager)} is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(appServices);

        AppServices = appServices;

        CursorManager = AddInitializableNode<CursorManager>(
            cm => cm.Initialize(CollisionLayers.Buildable));

        var buildProcessServices = new BuildProcessServices(
            GameSession.BuildManager,
            ResourceAssembler.PreviewResources);

        BuildProcessManager = AddInitializableNode<BuildProcessManager>(
            cbm => cbm.Initialize(buildProcessServices));

        InteractionManager = AddInitializableNode<InteractionManager>(
            im => im.Initialize(AppServices.InputManager, CursorManager, _buildContext));

        SpawnManager = AddInitializableNode<SpawnManager>(
            sm => sm.Initialize(ResourceAssembler.SpawnResources));

        CameraManager = CreateNode<CameraManager>();
        GameUiManager = CreateInitializableNode<GameUiManager>(
            gum => gum.Initialize());

#if DEBUG
        CursorDebugVisualizer.Initialize(CursorManager);
#endif

        _sessionInitialized = true;
    }

    /// <summary>
    /// Creates and wires the runtime flow objects that mediate interactions between managers.
    /// These flows subscribe to input and manager events and should be disposed when the node exits the tree.
    /// </summary>
    private void InitializeFlows()
    {
        if (!_sessionInitialized)
        {
            throw new InvalidOperationException($"Cannot initialize flows before {nameof(GameManager)} initialization.");
        }

        BuildContextFlow = new BuildContextFlow(GameUiManager.BuildUiManager, _buildContext, InteractionManager);
        BuildRequestFlow = new BuildRequestFlow(InteractionManager, BuildProcessManager);
        BuildSpawnFlow = new BuildSpawnFlow(BuildProcessManager, SpawnManager);
        BuildPreviewFlow = new BuildPreviewFlow(_buildContext, BuildProcessManager.BuildPreviewManager, CursorManager);
        CameraInputFlow = new CameraInputFlow(AppServices.InputManager, CameraManager);
    }

    /// <summary>
    /// Performs the steps necessary to start gameplay:
    /// - Adds manager nodes to the scene tree
    /// - Initializes flow objects
    /// - Sets the initial game state to paused
    /// </summary>
    private void StartSimulation()
    {
        if (!_sessionInitialized)
        {
            throw new InvalidOperationException($"Cannot start simulation before {nameof(GameManager)} session initialization.");
        }

        if (_simulationHasStarted)
        {
            throw new InvalidOperationException("Simulation already started.");
        }

        AddChild(CameraManager);
        AddChild(GameUiManager);

        InitializeFlows();
        SetState(GameState.Paused);
        _simulationHasStarted = true;
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
        AppServices.InputManager.OnGameStateChanged(newState);
        ConsoleLog.System(nameof(GameManager), $"GameState → {State}");
    }

    #endregion
}