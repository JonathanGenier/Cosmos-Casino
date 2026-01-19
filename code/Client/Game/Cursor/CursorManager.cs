using CosmosCasino.Core.Game.Map.Cell;
using Godot;
using System;

/// <summary>
/// Manages the initialization and resolution of the world-space cursor position within the application.
/// </summary>
/// <remarks>The CursorManager coordinates cursor position detection, including handling collision masks and plane
/// height for cursor placement. The manager must be initialized before use. This class is not thread-safe.</remarks>
public sealed partial class CursorManager : InitializableNodeManager
{
    #region Fields

    private CursorResolver? _resolver;

    private Vector3 _lastWorldPosition;
    private bool _hasLastWorldPosition;

    private MapCellCoord _lastCell;
    private bool _hasLastCell;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the cursor moves to a different cell in the grid.
    /// </summary>
    /// <remarks>Subscribers can use this event to respond to changes in the cursor's position, such as
    /// updating UI elements or triggering related actions. The event provides a <see cref="CursorContext"/> object
    /// containing information about the new cell.</remarks>
    public event Action<CursorContext>? CursorCellChanged;

    /// <summary>
    /// Occurs when the cursor context is lost.
    /// </summary>
    /// <remarks>Subscribers can use this event to perform cleanup or reset operations when the cursor context
    /// is no longer valid. The event is raised without any event data.</remarks>
    public event Action? CursorContextLost;

    #endregion

    #region Properties

    private CursorResolver Resolver
    {
        get => _resolver ?? throw new InvalidOperationException($"{nameof(CursorManager)} not initialized.");
        set => _resolver = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the cursor system with the specified collision mask and plane height.
    /// </summary>
    /// <param name="buildableCollisionMask">A bitmask that specifies which layers are considered buildable for collision detection.</param>
    /// <param name="planeHeight">The height, in world units, at which the cursor's reference plane is positioned. The default is 0.</param>
    public void Initialize(uint buildableCollisionMask, float planeHeight = 0f)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(CursorManager)} already initialized.");
        }

        var rayProvider = new CursorRayProvider();
        var physicsResolver = new CursorPhysicsResolver(buildableCollisionMask);
        var planeResolver = new CursorPlaneResolver(planeHeight);

        Resolver = new CursorResolver(rayProvider, physicsResolver, planeResolver);
        MarkInitialized();
    }

    #endregion

    #region Cursor Position

    /// <summary>
    /// Attempts to retrieve the current cursor context, including the screen and world positions, if available.
    /// </summary>
    /// <remarks>If the cursor is currently hovering over a GUI control, the method returns <see
    /// langword="false"/> and the world position in <paramref name="cursorContext"/> is not set. This method does not
    /// modify the state of the cursor manager.</remarks>
    /// <param name="cursorContext">When this method returns, contains a <see cref="CursorContext"/> structure with the current cursor's screen
    /// position and, if resolved, the corresponding world position. If the method returns <see langword="false"/>, the
    /// world position may be uninitialized.</param>
    /// <returns><see langword="true"/> if the cursor context was successfully resolved and a valid world position is available;
    /// otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the cursor manager has not been initialized.</exception>
    public bool TryGetCursorContext(out CursorContext cursorContext)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(CursorManager)} not initialized.");
        }

        var viewport = GetViewport();

        if (viewport == null)
        {
            cursorContext = default;
            return false;
        }

        var screenPosition = viewport.GetMousePosition();

        if (viewport.GuiGetHoveredControl() != null)
        {
            cursorContext = new CursorContext(screenPosition, default, false);
            return false;
        }

        var isValid = Resolver.TryResolve(out var worldPosition);
        cursorContext = new CursorContext(screenPosition, worldPosition, isValid);

        return isValid;
    }

    #endregion

    #region Godot Processes

    /// <summary>
    /// Handles per-frame processing for cursor context updates, including tracking world and cell position changes.
    /// </summary>
    /// <remarks>This method is called automatically by the engine each frame. It manages state related to the
    /// cursor's position in both world and cell coordinates, and raises events when the cursor context is lost or when
    /// the cell position changes. Override this method to implement custom per-frame logic related to cursor
    /// processing.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. This value can be used for time-based calculations.</param>
    protected override void OnProcess(double delta)
    {
        if (!TryGetCursorContext(out var cursorContext))
        {
            if (_hasLastWorldPosition || _hasLastCell)
            {
                _hasLastWorldPosition = false;
                _hasLastCell = false;
                CursorContextLost?.Invoke();
            }

            return;
        }

        // World position change (continuous)
        if (!_hasLastWorldPosition)
        {
            _hasLastWorldPosition = true;
            _lastWorldPosition = cursorContext.WorldPosition;
        }
        else if (!cursorContext.WorldPosition.IsEqualApprox(_lastWorldPosition))
        {
            _lastWorldPosition = cursorContext.WorldPosition;
        }

        // Cell position change (discrete)
        var cell = cursorContext.CellPosition;

        if (!_hasLastCell)
        {
            _hasLastCell = true;
            _lastCell = cell;
            CursorCellChanged?.Invoke(cursorContext);
        }
        else if (cell != _lastCell)
        {
            _lastCell = cell;
            CursorCellChanged?.Invoke(cursorContext);
        }
    }


    #endregion
}
