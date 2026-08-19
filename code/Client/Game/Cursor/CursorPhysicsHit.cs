using Godot;

/// <summary>
/// Immutable physical cursor hit returned by Godot physics.
/// </summary>
internal readonly struct CursorPhysicsHit
{
    #region Initialization

    /// <summary>
    /// Initializes a new physical cursor hit.
    /// </summary>
    /// <param name="worldPosition">The world-space position where the ray intersected the collider.</param>
    /// <param name="collider">The collision object hit by the ray.</param>
    public CursorPhysicsHit(Vector3 worldPosition, CollisionObject3D collider)
    {
        WorldPosition = worldPosition;
        Collider = collider;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the world-space position where the ray intersected the collider.
    /// </summary>
    public Vector3 WorldPosition { get; }

    /// <summary>
    /// Gets the collision object hit by the ray.
    /// </summary>
    public CollisionObject3D Collider { get; }

    #endregion
}
