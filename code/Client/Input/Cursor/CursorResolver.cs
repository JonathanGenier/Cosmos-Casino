using Godot;

/// <summary>
/// Coordinates multiple cursor resolution strategies to determine
/// the current world-space cursor position.
/// <para>
/// The resolver first attempts to resolve the cursor using physics-based
/// raycasting against buildable or interactive geometry. If no collision
/// is found, it falls back to a deterministic plane-based resolution,
/// ensuring a stable cursor position in empty world space.
/// </para>
/// <para>
/// Resolution strategies are evaluated in priority order and short-circuit
/// on the first successful result.
/// </para>
/// </summary>
public sealed class CursorResolver
{
    #region FIELDS

    private readonly CursorRayProvider _rayProvider;
    private readonly CursorPhysicsResolver _physicsResolver;
    private readonly CursorPlaneResolver _planeResolver;

    #endregion

    #region CONSTRUCTOR

    /// <summary>
    /// Initializes a new instance of the <see cref="CursorResolver"/>
    /// with the specified ray provider and resolution strategies.
    /// </summary>
    /// <param name="rayProvider">
    /// Provider used to generate the cursor ray.
    /// </param>
    /// <param name="physicsResolver">
    /// Primary resolver used to detect cursor intersections with
    /// physics-enabled world geometry.
    /// </param>
    /// <param name="planeResolver">
    /// Fallback resolver used when no physics intersection is found.
    /// </param>
    public CursorResolver(
        CursorRayProvider rayProvider,
        CursorPhysicsResolver physicsResolver,
        CursorPlaneResolver planeResolver)
    {
        _rayProvider = rayProvider;
        _physicsResolver = physicsResolver;
        _planeResolver = planeResolver;
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Attempts to resolve the current world-space cursor position.
    /// <para>
    /// The method evaluates available resolution strategies in priority
    /// order and returns the first successful result.
    /// </para>
    /// </summary>
    /// <param name="position">
    /// Output parameter that receives the resolved cursor position.
    /// </param>
    /// <returns>
    /// <c>true</c> if a valid cursor position was resolved; otherwise
    /// <c>false</c>.
    /// </returns>
    public bool TryResolve(out Vector3 position)
    {
        position = default;

        if (!_rayProvider.TryGetRay(out Ray3D ray))
        {
            return false;
        }

        if (_physicsResolver.TryResolve(ray, out position))
        {
            return true;
        }

        if (_planeResolver.TryResolve(ray, out position))
        {
            return true;
        }

        return false;
    }

    #endregion
}