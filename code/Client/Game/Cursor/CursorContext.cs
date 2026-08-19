using CosmosCasino.Core.Game.Map;
using Godot;

/// <summary>
/// Represents an immutable snapshot of the cursor's state, including screen position, world position, and logical target.
/// </summary>
public readonly struct CursorContext
{
    #region Initialization

    /// <summary>
    /// Initializes a valid cursor context with the specified screen position, world position, and logical target.
    /// </summary>
    /// <param name="screenPosition">The position of the cursor in screen coordinates.</param>
    /// <param name="worldPosition">The physical or fallback position of the cursor in world coordinates.</param>
    /// <param name="target">The logical cursor target.</param>
    public CursorContext(
        Vector2 screenPosition,
        Vector3 worldPosition,
        CursorTarget target)
    {
        ScreenPosition = screenPosition;
        WorldPosition = worldPosition;
        Target = target;
        IsValid = true;
    }

    private CursorContext(
        Vector2 screenPosition,
        Vector3 worldPosition,
        CursorTarget target,
        bool isValid)
    {
        ScreenPosition = screenPosition;
        WorldPosition = worldPosition;
        Target = target;
        IsValid = isValid;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the position of the object on the screen in pixel coordinates.
    /// </summary>
    public Vector2 ScreenPosition { get; }

    /// <summary>
    /// Gets the physical or fallback cursor position in world coordinates.
    /// </summary>
    public Vector3 WorldPosition { get; }

    /// <summary>
    /// Gets the logical cursor target.
    /// </summary>
    public CursorTarget Target { get; }

    /// <summary>
    /// Gets a value indicating whether the current object is in a valid state.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the cell coordinates corresponding to the current world position.
    /// </summary>
    public MapCoord CellPosition => Target.Coord;

    #endregion

    #region Factories

    /// <summary>
    /// Creates an invalid cursor context at the specified screen position.
    /// </summary>
    /// <param name="screenPosition">The position of the cursor in screen coordinates.</param>
    /// <returns>An invalid cursor context.</returns>
    public static CursorContext Invalid(Vector2 screenPosition)
    {
        return new CursorContext(screenPosition, default, default, false);
    }

    #endregion
}
