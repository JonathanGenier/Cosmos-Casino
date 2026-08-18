using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Manages the initialization and resolution of the logical cursor target within the application.
/// </summary>
/// <remarks>The CursorManager coordinates cursor position detection, including handling collision masks and plane
/// height for cursor placement. The manager must be initialized before use. This class is not thread-safe.</remarks>
public sealed partial class CursorManager : InitializableNodeManager
{
    #region Fields

    private CursorResolver? _resolver;

    private Vector3 _lastWorldPosition;
    private bool _hasLastWorldPosition;

    private CursorTarget _lastTarget;
    private bool _hasLastTarget;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the cursor resolves to a different logical target.
    /// </summary>
    /// <remarks>Subscribers can use this event to respond to changes in the cursor target, such as updating
    /// build previews when the cursor moves from terrain to a buildable at the same map coordinate.</remarks>
    public event Action<CursorContext>? CursorTargetChanged;

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
    /// <param name="mapManager">The authoritative map manager used to resolve terrain target elevation.</param>
    /// <param name="collisionMask">A bitmask specifying which world surfaces participate in cursor collision detection.</param>
    /// <param name="planeHeight">The height, in world units, at which the cursor's reference plane is positioned. The default is 0.</param>
    public void Initialize(MapManager mapManager, uint collisionMask, float planeHeight = 0f)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(CursorManager)} already initialized.");
        }

        ArgumentNullException.ThrowIfNull(mapManager);

        var rayProvider = new CursorRayProvider();
        var physicsResolver = new CursorPhysicsResolver(collisionMask);
        var planeResolver = new CursorPlaneResolver(planeHeight);
        var targetResolver = new CursorTargetResolver(mapManager);

        Resolver = new CursorResolver(rayProvider, physicsResolver, planeResolver, targetResolver);
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
            cursorContext = CursorContext.Invalid(screenPosition);
            return false;
        }

        if (!Resolver.TryResolve(out var worldPosition, out var target))
        {
            cursorContext = CursorContext.Invalid(screenPosition);
            return false;
        }

        cursorContext = new CursorContext(screenPosition, worldPosition, target);
        return true;
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
            if (_hasLastWorldPosition || _hasLastTarget)
            {
                _hasLastWorldPosition = false;
                _hasLastTarget = false;
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

        // Logical target change (discrete)
        var target = cursorContext.Target;

        if (!_hasLastTarget)
        {
            _hasLastTarget = true;
            _lastTarget = target;
            CursorTargetChanged?.Invoke(cursorContext);
        }
        else if (target != _lastTarget)
        {
            _lastTarget = target;
            CursorTargetChanged?.Invoke(cursorContext);
        }
    }


    #endregion
}
