using Godot;

/// <summary>
/// Provides a mechanism for resolving the current world-space position of a cursor using prioritized resolution
/// strategies.
/// </summary>
/// <remarks>The CursorResolver attempts to determine the cursor's position by first evaluating physics-based
/// intersections and, if unsuccessful, falling back to a plane-based resolution. This class is typically used in
/// scenarios where accurate cursor placement in 3D space is required, such as in interactive applications or
/// games.</remarks>
public sealed class CursorResolver
{
    #region Fields

    private readonly CursorRayProvider _rayProvider;
    private readonly CursorPhysicsResolver _physicsResolver;
    private readonly CursorPlaneResolver _planeResolver;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CursorResolver class with the specified ray provider, physics resolver, and
    /// plane resolver.
    /// </summary>
    /// <param name="rayProvider">The provider used to generate rays for cursor positioning.</param>
    /// <param name="physicsResolver">The resolver responsible for handling cursor interactions with physical objects.</param>
    /// <param name="planeResolver">The resolver used to determine cursor placement on planar surfaces.</param>
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
    /// Attempts to resolve a 3D position based on the current ray and available resolvers.
    /// </summary>
    /// <remarks>This method first attempts to obtain a ray from the underlying ray provider. If successful,
    /// it tries to resolve a position using the physics resolver, and if that fails, it attempts to resolve using the
    /// plane resolver. The method does not throw exceptions for failure; callers should check the return value to
    /// determine success.</remarks>
    /// <param name="position">When this method returns, contains the resolved 3D position if the operation succeeds; otherwise, contains the
    /// default value for <see cref="Vector3"/>. This parameter is passed uninitialized.</param>
    /// <returns>true if a position was successfully resolved; otherwise, false.</returns>
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