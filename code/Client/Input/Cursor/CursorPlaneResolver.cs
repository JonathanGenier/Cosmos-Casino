using Godot;

/// <summary>
/// Provides a deterministic fallback strategy for resolving the
/// world-space cursor position by intersecting a ray with a fixed
/// horizontal plane.
/// <para>
/// This resolver is intended to be used when no physics-based geometry
/// is intersected by the cursor ray, ensuring a valid cursor position
/// is always available within empty or unbounded world space.
/// </para>
/// <para>
/// The plane is defined by a constant height and extends infinitely
/// along the X and Z axes.
/// </para>
/// </summary>
public sealed class CursorPlaneResolver
{
    #region FIELDS

    private readonly Plane _plane;

    #endregion

    #region CONSTRUCTOR

    /// <summary>
    /// Initializes a new instance of the <see cref="CursorPlaneResolver"/>
    /// with a plane positioned at the specified world-space height.
    /// </summary>
    /// <param name="height">
    /// World-space height at which the fallback plane is placed.
    /// </param>
    public CursorPlaneResolver(float height = 0f)
    {
        _plane = new Plane(Vector3.Up, height);
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Attempts to resolve the cursor position by intersecting the
    /// provided ray with the configured fallback plane.
    /// </summary>
    /// <param name="ray">
    /// World-space ray representing the cursor direction.
    /// </param>
    /// <param name="position">
    /// Output parameter that receives the resolved intersection point
    /// if the ray intersects the plane.
    /// </param>
    /// <returns>
    /// <c>true</c> if the ray intersects the plane; otherwise <c>false</c>.
    /// </returns>
    public bool TryResolve(in Ray3D ray, out Vector3 position)
    {
        position = default;

        Vector3? hit = _plane.IntersectsRay(ray.Origin, ray.Direction);
        if (hit == null)
        {
            return false;
        }

        position = hit.Value;
        return true;
    }

    #endregion
}