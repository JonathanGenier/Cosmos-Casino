using Godot;
using System;

/// <summary>
/// Provides a mechanism for resolving the current logical cursor target using prioritized resolution strategies.
/// </summary>
/// <remarks>The CursorResolver attempts to determine the cursor's target by first evaluating physics-based
/// intersections and, if unsuccessful, falling back to a plane-based terrain resolution.</remarks>
internal sealed class CursorResolver
{
    #region Fields

    private readonly CursorRayProvider _rayProvider;
    private readonly CursorPhysicsResolver _physicsResolver;
    private readonly CursorPlaneResolver _planeResolver;
    private readonly CursorTargetResolver _targetResolver;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CursorResolver class with the specified ray provider, physics resolver, and
    /// plane resolver.
    /// </summary>
    /// <param name="rayProvider">The provider used to generate rays for cursor positioning.</param>
    /// <param name="physicsResolver">The resolver responsible for handling cursor interactions with physical objects.</param>
    /// <param name="planeResolver">The resolver used to determine cursor placement on planar surfaces.</param>
    /// <param name="targetResolver">The resolver used to interpret physical hits and fallback positions as logical targets.</param>
    internal CursorResolver(
        CursorRayProvider rayProvider,
        CursorPhysicsResolver physicsResolver,
        CursorPlaneResolver planeResolver,
        CursorTargetResolver targetResolver)
    {
        ArgumentNullException.ThrowIfNull(rayProvider);
        ArgumentNullException.ThrowIfNull(physicsResolver);
        ArgumentNullException.ThrowIfNull(planeResolver);
        ArgumentNullException.ThrowIfNull(targetResolver);

        _rayProvider = rayProvider;
        _physicsResolver = physicsResolver;
        _planeResolver = planeResolver;
        _targetResolver = targetResolver;
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Attempts to resolve a logical cursor target based on the current ray and available resolvers.
    /// </summary>
    /// <remarks>This method first attempts to obtain a ray from the underlying ray provider. If successful,
    /// it tries to resolve a position using the physics resolver, and if that fails, it attempts to resolve using the
    /// plane resolver. The method does not throw exceptions for failure; callers should check the return value to
    /// determine success.</remarks>
    /// <param name="worldPosition">When this method returns, contains the physical or fallback world position used for cursor visualization.</param>
    /// <param name="target">When this method returns, contains the resolved logical target.</param>
    /// <returns>true if a target was successfully resolved; otherwise, false.</returns>
    public bool TryResolve(out Vector3 worldPosition, out CursorTarget target)
    {
        worldPosition = default;
        target = default;

        if (!_rayProvider.TryGetRay(out Ray3D ray))
        {
            return false;
        }

        if (_physicsResolver.TryResolve(ray, out var physicsHit)
            && _targetResolver.TryResolve(physicsHit, out target))
        {
            worldPosition = physicsHit.WorldPosition;
            return true;
        }

        if (_planeResolver.TryResolve(ray, out worldPosition)
            && _targetResolver.TryResolveTerrain(worldPosition, out target))
        {
            return true;
        }

        return false;
    }

    #endregion
}
