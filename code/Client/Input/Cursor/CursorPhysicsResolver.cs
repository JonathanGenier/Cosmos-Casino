using Godot;

/// <summary>
/// Resolves the world-space cursor position by performing a physics
/// raycast against buildable or interactive geometry.
/// <para>
/// This resolver queries the active 3D physics space using a configurable
/// collision mask and maximum distance, returning the first valid hit
/// position encountered along the ray.
/// </para>
/// <para>
/// The resolver is stateless and does not cache results; it is intended
/// to be used as part of a higher-level cursor resolution pipeline that
/// may include additional fallback strategies.
/// </para>
/// </summary>
public sealed class CursorPhysicsResolver
{
    #region FIELDS

    private readonly uint _collisionMask;
    private readonly float _maxDistance;

    #endregion

    #region CONSTRUCTOR

    /// <summary>
    /// Initializes a new instance of the <see cref="CursorPhysicsResolver"/>
    /// with the specified collision filtering and raycast range.
    /// </summary>
    /// <param name="collisionMask">
    /// Physics collision mask used to identify buildable or targetable
    /// geometry.
    /// </param>
    /// <param name="maxDistance">
    /// Maximum distance the raycast may travel before giving up.
    /// </param>
    public CursorPhysicsResolver(uint collisionMask, float maxDistance = 10000f)
    {
        _collisionMask = collisionMask;
        _maxDistance = maxDistance;
    }

    #endregion

    #region METHOD

    /// <summary>
    /// Attempts to resolve a cursor hit position by raycasting into the
    /// active 3D physics world.
    /// </summary>
    /// <param name="ray">
    /// World-space ray representing the cursor direction.
    /// </param>
    /// <param name="position">
    /// Output parameter that receives the resolved hit position if a
    /// collision is detected.
    /// </param>
    /// <returns>
    /// <c>true</c> if a valid physics hit was found; otherwise <c>false</c>.
    /// </returns>
    public bool TryResolve(in Ray3D ray, out Vector3 position)
    {
        position = default;

        var tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return false;
        }

        var spaceState = tree.Root.GetWorld3D().DirectSpaceState;

        var query = PhysicsRayQueryParameters3D.Create(
                ray.Origin,
                ray.Origin + (ray.Direction * _maxDistance)
            );

        query.CollisionMask = _collisionMask;
        query.CollideWithBodies = true;
        query.CollideWithAreas = false;

        var result = spaceState.IntersectRay(query);

        if (result.Count == 0)
        {
            return false;
        }

        position = (Vector3)result["position"];
        return true;
    }

    #endregion
}
