using CosmosCasino.Core.Console.Logging;
using Godot;
using System;

/// <summary>
/// Manages primary world interaction gestures and routes them to the
/// currently active interaction tool.
/// This manager owns the gesture lifecycle (start, update, end, cancel)
/// and tool switching, but does not interpret gameplay meaning or
/// execute world logic directly.
/// </summary>
/// <param name="bootstrap">
/// Client bootstrap providing access to initialized services.
/// </param>
public sealed partial class InteractionManager(ClientBootstrap bootstrap)
    : ClientManager(bootstrap)
{
    #region FIELDS

    private IInteractionHandler _activeHandler = null!;
    private CursorContext _pressContext;
    private bool _isPrimaryHeld;
    private BuildInteractionHandler _buildHandler;
    private SelectionInteractionHandler _selectionHandler;

    #endregion

    #region METHODS

    /// <summary>
    /// Switches the active interaction tool used to interpret primary
    /// interaction gestures.
    /// If a gesture is currently in progress, it is cancelled before the
    /// tool switch is applied.
    /// </summary>
    /// <param name="tool">
    /// The interaction tool to activate.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if an unsupported interaction tool is specified.
    /// </exception>
    public void SetTool(InteractionTool tool)
    {
        if (_isPrimaryHeld)
        {
            _isPrimaryHeld = false;
            _activeHandler.OnPrimaryGestureCancelled(_pressContext);
        }

        _activeHandler = tool switch
        {
            InteractionTool.Selection => _selectionHandler,
            InteractionTool.Build => _buildHandler,
            _ => throw new ArgumentOutOfRangeException(nameof(tool))
        };
    }

    /// <summary>
    /// Initializes the interaction manager by wiring input signals,
    /// creating interaction handlers, and activating the default
    /// interaction tool.
    /// </summary>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(InteractionManager)))
        {
            var input = ClientServices.InputManager;
            input.PrimaryInteractionPressed += OnPrimaryInteractionPressed;
            input.PrimaryInteractionReleased += OnPrimaryInteractionReleased;
            InitializeHandlers();
            SetTool(InteractionTool.Selection);
        }
    }

    /// <summary>
    /// Disconnects input signal bindings when this node is removed from
    /// the scene tree to prevent dangling callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        var input = ClientServices.InputManager;
        input.PrimaryInteractionPressed -= OnPrimaryInteractionPressed;
        input.PrimaryInteractionReleased -= OnPrimaryInteractionReleased;
    }

    /// <summary>
    /// Updates the active interaction handler while a primary interaction
    /// gesture is in progress, providing live cursor context for visual
    /// feedback such as selection or build previews.
    /// </summary>
    /// <param name="delta">
    /// Frame time in seconds.
    /// </param>
    public override void _Process(double delta)
    {
        if (!_isPrimaryHeld)
        {
            return;
        }

        CursorContext currentContext = BuildCursorContext();
        _activeHandler.OnPrimaryGestureUpdated(_pressContext, currentContext);
    }

    /// <summary>
    /// Handles the start of a primary interaction gesture by capturing the
    /// initial cursor context and notifying the active interaction handler.
    /// </summary>
    private void OnPrimaryInteractionPressed()
    {
        if (_isPrimaryHeld)
        {
            return;
        }

        _pressContext = BuildCursorContext();

        if (!_pressContext.IsValid)
        {
            return;
        }

        _isPrimaryHeld = true;
        _activeHandler.OnPrimaryGestureStarted(_pressContext);
    }

    /// <summary>
    /// Handles the end of a primary interaction gesture and notifies the
    /// active interaction handler with the final cursor context.
    /// </summary>
    private void OnPrimaryInteractionReleased()
    {
        if (!_isPrimaryHeld)
        {
            return;
        }

        _isPrimaryHeld = false;
        CursorContext releaseContext = BuildCursorContext();
        _activeHandler.OnPrimaryGestureEnded(_pressContext, releaseContext);
    }

    /// <summary>
    /// Builds a cursor context snapshot containing the current screen-space
    /// mouse position and resolved world-space cursor position.
    /// </summary>
    /// <returns>
    /// A cursor context describing the current cursor state.
    /// </returns>
    private CursorContext BuildCursorContext()
    {
        Vector2 screenPosition = GetViewport().GetMousePosition();
        bool isValid = CursorService.TryGetCursorPosition(out Vector3 worldPosition);

        return new CursorContext(
            screenPosition: screenPosition,
            worldPosition: worldPosition,
            isValid: isValid);
    }

    /// <summary>
    /// Creates and initializes all interaction handlers used by this
    /// interaction manager.
    /// </summary>
    private void InitializeHandlers()
    {
        _buildHandler = new BuildInteractionHandler();
        _selectionHandler = new SelectionInteractionHandler();
    }

    #endregion
}
