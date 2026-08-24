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
    /// <param name="rayOrigin">The world-space ray origin used to produce the hit.</param>
    /// <param name="worldPosition">The world-space position where the ray intersected the collider.</param>
    /// <param name="worldNormal">The world-space surface normal reported by physics at the hit.</param>
    /// <param name="collider">The collision object hit by the ray.</param>
    /// <param name="faceIndex">The triangle face index reported by Godot for compatible concave collision shapes.</param>
    public CursorPhysicsHit(
        Vector3 rayOrigin,
        Vector3 worldPosition,
        Vector3 worldNormal,
        CollisionObject3D collider,
        int faceIndex = -1)
    {
        RayOrigin = rayOrigin;
        WorldPosition = worldPosition;
        WorldNormal = worldNormal;
        Collider = collider;
        FaceIndex = faceIndex;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the world-space ray origin used to produce the hit.
    /// </summary>
    public Vector3 RayOrigin { get; }

    /// <summary>
    /// Gets the world-space position where the ray intersected the collider.
    /// </summary>
    public Vector3 WorldPosition { get; }

    /// <summary>
    /// Gets the world-space surface normal reported by physics at the hit.
    /// </summary>
    public Vector3 WorldNormal { get; }

    /// <summary>
    /// Gets the collision object hit by the ray.
    /// </summary>
    public CollisionObject3D Collider { get; }

    /// <summary>
    /// Gets the triangle face index reported by Godot, or -1 when unavailable.
    /// </summary>
    public int FaceIndex { get; }

    #endregion
}
