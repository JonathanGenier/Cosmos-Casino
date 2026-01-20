using System;

/// <summary>
/// Manages user interaction modes and gesture handling for primary input actions, coordinating between selection and
/// build tools within the scene.
/// </summary>
/// <remarks>The InteractionManager is responsible for switching between different interaction tools, such as
/// selection and build modes, and delegates input gestures to the appropriate handler. It integrates with input and
/// cursor management systems to provide real-time feedback and ensures that input signal bindings are properly managed
/// throughout the node's lifecycle. This class is typically used as a central controller for user-driven interactions
/// in the scene and should be initialized with the required managers before use.</remarks>
public sealed partial class InteractionManager : InitializableNodeManager
{
    #region Fields

    private bool _isPrimaryHeld;

    private IInteractionHandler? _activeHandler;
    private CursorContext? _cursorContext;
    private InputManager? _inputManager;
    private CursorManager? _cursorManager;
    private BuildContext? _buildContext;
    private BuildInteractionHandler? _buildHandler;
    private SelectionInteractionHandler? _selectionHandler;

    #endregion

    #region Properties

    private InputManager InputManager
    {
        get => _inputManager ?? throw new InvalidOperationException($"{nameof(InputManager)} is not initialized.");
        set => _inputManager = value;
    }

    private CursorManager CursorManager
    {
        get => _cursorManager ?? throw new InvalidOperationException($"{nameof(CursorManager)} is not initialized.");
        set => _cursorManager = value;
    }

    private BuildContext BuildContext
    {
        get => _buildContext ?? throw new InvalidOperationException($"{nameof(BuildContext)} is not initialized.");
        set => _buildContext = value;
    }

    private BuildInteractionHandler BuildHandler
    {
        get => _buildHandler ?? throw new InvalidOperationException($"{nameof(BuildInteractionHandler)} is not initialized.");
        set => _buildHandler = value;
    }

    private SelectionInteractionHandler SelectionHandler
    {
        get => _selectionHandler ?? throw new InvalidOperationException($"{nameof(SelectionInteractionHandler)} is not initialized.");
        set => _selectionHandler = value;
    }

    private IInteractionHandler ActiveHandler
    {
        get => _activeHandler ?? throw new InvalidOperationException($"{nameof(IInteractionHandler)} not set. Call {nameof(SetTool)} first.");
        set => _activeHandler = value;
    }

    private CursorContext CursorContext
    {
        get
        {
            if (!_isPrimaryHeld)
            {
                throw new InvalidOperationException($"{nameof(CursorContext)} accessed while no primary interaction is active.");
            }

            return _cursorContext ?? throw new InvalidOperationException($"{nameof(CursorContext)} was not initialized.");
        }

        set => _cursorContext = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the component with the specified input, cursor, and build context managers.
    /// </summary>
    /// <param name="inputManager">The input manager to use for handling user input. Cannot be null.</param>
    /// <param name="cursorManager">The cursor manager responsible for cursor state and behavior. Cannot be null.</param>
    /// <param name="buildContext">The build context providing configuration and state for initialization. Cannot be null.</param>
    public void Initialize(InputManager inputManager, CursorManager cursorManager, BuildContext buildContext)
    {
        InputManager = inputManager;
        CursorManager = cursorManager;
        BuildContext = buildContext;
        MarkInitialized();
    }

    #endregion

    #region Interaction Tool Methods

    /// <summary>
    /// Sets the current interaction tool used for handling user input.
    /// </summary>
    /// <param name="tool">The interaction tool to activate. Must be a valid value of the <see cref="InteractionTool"/> enumeration.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="tool"/> is not a valid value of the <see cref="InteractionTool"/> enumeration.</exception>
    public void SetTool(InteractionTool tool)
    {
        if (_isPrimaryHeld && _cursorContext != null && _activeHandler != null)
        {
            _isPrimaryHeld = false;
            _activeHandler.OnPrimaryGestureCancelled(_cursorContext.Value);
        }

        ActiveHandler = tool switch
        {
            InteractionTool.Selection => SelectionHandler,
            InteractionTool.Build => BuildHandler,
            _ => throw new ArgumentOutOfRangeException(nameof(tool))
        };

        ConsoleLog.Info(nameof(InteractionManager), $"Switched to {tool} tool.");
    }

    /// <summary>
    /// Resets the current interaction tool to the default selection tool.
    /// </summary>
    public void ResetTool()
    {
        SetTool(InteractionTool.Selection);
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes the InteractionManager when the node enters the scene tree. Sets up input event handlers and
    /// prepares the manager for user interactions.
    /// </summary>
    /// <remarks>This method is called automatically by the Godot engine when the node is added to the scene.
    /// It ensures that required dependencies are initialized and configures the default interaction tool. Override this
    /// method to add additional setup logic if extending InteractionManager.</remarks>
    protected override void OnReady()
    {
        using (ConsoleLog.SystemScope(nameof(InteractionManager)))
        {
            InputManager.BuildPrimaryPressed += OnPrimaryInteractionPressed;
            InputManager.BuildPrimaryReleased += OnPrimaryInteractionReleased;

            InitializeHandlers();

            SetTool(InteractionTool.Selection);
        }
    }

    /// <summary>
    /// Called when the node is about to be removed from the scene tree. Unsubscribes from input and build event
    /// handlers to prevent memory leaks or unintended behavior after the node is freed.
    /// </summary>
    /// <remarks>Override this method to perform cleanup tasks before the node is removed from the scene. This
    /// method is typically used to disconnect signals or release resources that were acquired during the node's
    /// lifetime.</remarks>
    protected override void OnExit()
    {
        InputManager.BuildPrimaryPressed -= OnPrimaryInteractionPressed;
        InputManager.BuildPrimaryReleased -= OnPrimaryInteractionReleased;
    }

    /// <summary>
    /// Updates the gesture state for the current frame if the primary input is held.
    /// </summary>
    /// <remarks>This method is typically called once per frame by the engine. Gesture updates occur only
    /// while the primary input is active.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. This value can be used for time-based calculations
    /// within the update.</param>
    protected override void OnProcess(double delta)
    {
        if (!_isPrimaryHeld)
        {
            return;
        }

        CursorManager.TryGetCursorContext(out var currentContext);
        ActiveHandler.OnPrimaryGestureUpdated(CursorContext, currentContext);
    }

    #endregion

    #region Primary Interaction Actions

    /// <summary>
    /// Handles the logic for when the primary interaction input is pressed.
    /// </summary>
    /// <remarks>This method initiates the primary gesture if it is not already active and the current cursor
    /// context is valid. It should be called in response to a user action such as a mouse button or touch
    /// press.</remarks>
    private void OnPrimaryInteractionPressed()
    {
        CursorManager.TryGetCursorContext(out var pressedContext);

        if (!pressedContext.IsValid)
        {
            return;
        }

        if (_activeHandler == null)
        {
            throw new InvalidOperationException($"{nameof(InteractionManager)} has no active interaction handler.");
        }

        _cursorContext = pressedContext;
        _isPrimaryHeld = true;
        ActiveHandler.OnPrimaryGestureStarted(CursorContext);
    }

    /// <summary>
    /// Handles the release of the primary interaction, finalizing any active gesture or operation associated with it.
    /// </summary>
    /// <remarks>This method should be called when the primary interaction (such as a mouse button or touch
    /// press) is released. It ensures that any gesture or operation initiated by the primary interaction is properly
    /// completed. If the primary interaction is not currently held, the method has no effect.</remarks>
    private void OnPrimaryInteractionReleased()
    {
        var startContext = CursorContext;

        _isPrimaryHeld = false;

        CursorManager.TryGetCursorContext(out var releaseContext);
        ActiveHandler.OnPrimaryGestureEnded(startContext, releaseContext);

        _cursorContext = null;
    }

    #endregion

    #region Handlers

    /// <summary>
    /// Creates and initializes all interaction handlers used by this
    /// interaction manager.
    /// </summary>
    private void InitializeHandlers()
    {
        BuildHandler = new BuildInteractionHandler(BuildContext);
        SelectionHandler = new SelectionInteractionHandler();
    }

    #endregion   
}