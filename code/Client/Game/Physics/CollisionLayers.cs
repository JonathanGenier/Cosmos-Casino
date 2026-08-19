/// <summary>
/// Godot 3D physics collision layer mappings.
/// These values must match the editor configuration.
/// </summary>
public static class CollisionLayers
{
    /// <summary>
    /// Buildable physics bodies that can be targeted by the cursor.
    /// Godot 3D Physics Layer 1.
    /// </summary>
    public const uint Buildable = 1u << 0;

    /// <summary>
    /// No physics collision layers.
    /// </summary>
    internal const uint None = 0u;

    /// <summary>
    /// Rendered terrain surfaces that can be targeted by the cursor.
    /// Godot 3D Physics Layer 2.
    /// </summary>
    internal const uint Terrain = 1u << 1;
}
