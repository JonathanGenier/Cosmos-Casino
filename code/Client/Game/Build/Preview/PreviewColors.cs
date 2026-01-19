using Godot;

/// <summary>
/// Defines colors used by build preview visuals to indicate
/// whether a build operation is valid, invalid, or has no effect.
/// </summary>
public static class PreviewColors
{
    /// <summary>
    /// Color used when a build operation is valid.
    /// </summary>
    public static readonly Color ValidColor = new(0.0f, 1.0f, 0.0f, 0.1f);

    /// <summary>
    /// Color used when a build operation results in no change.
    /// </summary>
    public static readonly Color NoOpColor = new(0.3f, 0.5f, 1.0f, 0.1f);

    /// <summary>
    /// Color used when a build operation is invalid.
    /// </summary>
    public static readonly Color InvalidColor = new(1.0f, 0.0f, 0.0f, 0.1f);
}
