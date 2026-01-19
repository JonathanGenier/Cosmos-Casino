using Godot;

/// <summary>
/// Provides functionality to resolve a world-space position by intersecting a ray with a fallback plane at a specified
/// height.
/// </summary>
/// <remarks>This class is typically used to determine a cursor or pointer position in 3D space when no other
/// intersection is available. The fallback plane is positioned parallel to the XZ plane at the given world-space
/// height. Instances of this class are immutable and thread-safe.</remarks>
public sealed class CursorPlaneResolver
{
    #region Fields

    private readonly Plane _plane;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CursorPlaneResolver class with a plane at the specified height.
    /// </summary>
    /// <param name="height">The height, in world units, at which to position the plane. Defaults to 0 if not specified.</param>
    public CursorPlaneResolver(float height = 0f)
    {
        _plane = new Plane(Vector3.Up, height);
    }

    #endregion

    #region Resolver

    /// <summary>
    /// Attempts to find the intersection point between the specified ray and the plane.
    /// </summary>
    /// <param name="ray">The ray to test for intersection with the plane.</param>
    /// <param name="worldPosition">When this method returns, contains the intersection point if the ray intersects the plane; otherwise, contains
    /// <see cref="Vector3"/>.</param>
    /// <returns><see langword="true"/> if the ray intersects the plane and the intersection point is found; otherwise, <see
    /// langword="false"/>.</returns>
    public bool TryResolve(in Ray3D ray, out Vector3 worldPosition)
    {
        worldPosition = default;

        Vector3? hit = _plane.IntersectsRay(ray.Origin, ray.Direction);
        if (hit == null)
        {
            return false;
        }

        worldPosition = hit.Value;
        return true;
    }

    #endregion
}