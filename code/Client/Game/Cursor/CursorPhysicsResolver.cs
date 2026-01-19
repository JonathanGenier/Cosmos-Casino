using Godot;

/// <summary>
/// Provides functionality to resolve cursor hit positions in a 3D physics world using raycasting with configurable
/// collision filtering and maximum distance.
/// </summary>
public sealed class CursorPhysicsResolver
{
    #region Fields

    private readonly uint _collisionMask;
    private readonly float _maxDistance;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CursorPhysicsResolver class with the specified collision mask and maximum
    /// distance.
    /// </summary>
    /// <param name="collisionMask">A bitmask that specifies which layers are considered for collision detection.</param>
    /// <param name="maxDistance">The maximum distance, in world units, to check for collisions. Must be positive. The default is 10000.</param>
    public CursorPhysicsResolver(uint collisionMask, float maxDistance = 10000f)
    {
        _collisionMask = collisionMask;
        _maxDistance = maxDistance;
    }

    #endregion

    #region Resolver

    /// <summary>
    /// Attempts to find the first intersection point between the specified ray and any physics body in the scene.
    /// </summary>
    /// <remarks>This method only considers collisions with physics bodies and ignores areas. The intersection
    /// test is limited by the configured maximum distance and collision mask. If the scene tree or world is not
    /// available, the method returns <see langword="false"/>.</remarks>
    /// <param name="ray">The ray to cast into the scene. Specifies the origin and direction for the intersection test.</param>
    /// <param name="worldPosition">When this method returns, contains the position of the first intersection point if an intersection is found;
    /// otherwise, contains <see cref="Vector3.Zero"/>.</param>
    /// <returns><see langword="true"/> if the ray intersects a physics body and the intersection position is found; otherwise,
    /// <see langword="false"/>.</returns>
    public bool TryResolve(in Ray3D ray, out Vector3 worldPosition)
    {
        worldPosition = default;

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

        worldPosition = (Vector3)result["position"];
        return true;
    }

    #endregion
}
