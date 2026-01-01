using Godot;

/// <summary>
/// Immutable snapshot of cursor-related state at a specific moment in time.
/// Constructed by InteractionManager using CursorService and
/// engine-level cursor queries. Contains no logic.
/// </summary>
public readonly struct CursorContext
{
    #region CONSTRUCTOR

    /// <summary>
    /// Initializes a new cursor context snapshot with the resolved
    /// screen-space and world-space cursor state.
    /// </summary>
    /// <param name="screenPosition">
    /// Cursor position in screen space, in pixels.
    /// </param>
    /// <param name="worldPosition">
    /// Cursor position in world space when resolution succeeds.
    /// </param>
    /// <param name="isValid">
    /// Indicates whether a valid world-space cursor position was resolved.
    /// </param>
    public CursorContext(
        Vector2 screenPosition,
        Vector3 worldPosition,
        bool isValid)
    {
        ScreenPosition = screenPosition;
        WorldPosition = worldPosition;
        IsValid = isValid;
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Cursor position in screen space (pixels).
    /// Used for interaction intent classification (click vs drag).
    /// </summary>
    public Vector2 ScreenPosition { get; }

    /// <summary>
    /// Cursor position in world space.
    /// Undefined if <see cref="IsValid"/> is false.
    /// </summary>
    public Vector3 WorldPosition { get; }

    /// <summary>
    /// Whether a valid world-space cursor position was resolved.
    /// </summary>
    public bool IsValid { get; }

    #endregion
}
