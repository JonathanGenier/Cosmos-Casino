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
    /// Attempts to retrieve the current cursor position in world coordinates.
    /// </summary>
    /// <remarks>The method returns false if the viewport is unavailable or if the cursor is currently
    /// hovering over a GUI control. In these cases, the output parameter is set to its default value.</remarks>
    /// <param name="position">When this method returns, contains the cursor position as a <see cref="Vector3"/> if the operation succeeds;
    /// otherwise, contains the default value.</param>
    /// <returns>true if the cursor position was successfully retrieved; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the cursor manager has not been initialized.</exception>
    public bool TryGetCursorPosition(out Vector3 position)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(CursorManager)} not initialized.");
        }

        position = default;

        var viewport = GetViewport();
        if (viewport == null)
        {
            return false;
        }

        if (viewport.GuiGetHoveredControl() != null)
        {
            return false;
        }

        return Resolver.TryResolve(out position);
    }

    #endregion
}
