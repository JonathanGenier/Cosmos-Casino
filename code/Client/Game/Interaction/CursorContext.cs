using Godot;

/// <summary>
/// Represents an immutable snapshot of the cursor's state, including its position in both screen space and world space,
/// and whether the world-space position is valid.
/// </summary>
public readonly struct CursorContext
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CursorContext class with the specified screen and world positions and validity
    /// state.
    /// </summary>
    /// <param name="screenPosition">The position of the cursor in screen coordinates.</param>
    /// <param name="worldPosition">The position of the cursor in world coordinates.</param>
    /// <param name="isValid">A value indicating whether the cursor context is valid. Set to <see langword="true"/> if the context is valid;
    /// otherwise, <see langword="false"/>.</param>
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

    #region Properties

    /// <summary>
    /// Gets the position of the object on the screen in pixel coordinates.
    /// </summary>
    public Vector2 ScreenPosition { get; }

    /// <summary>
    /// Gets the position of the object in world coordinates.
    /// </summary>
    public Vector3 WorldPosition { get; }

    /// <summary>
    /// Gets a value indicating whether the current object is in a valid state.
    /// </summary>
    public bool IsValid { get; }

    #endregion
}
