using Godot;

/// <summary>
/// Represents an immutable world-space ray defined by an origin
/// point and a normalized direction vector.
/// <para>
/// This value type is used throughout the cursor system to pass
/// ray data between providers and resolvers without coupling to
/// engine-specific raycasting APIs.
/// </para>
/// </summary>
public readonly struct Ray3D
{
    #region CONSTRUCTOR

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray3D"/> struct
    /// with the specified origin and direction.
    /// </summary>
    /// <param name="origin">
    /// World-space starting point of the ray.
    /// </param>
    /// <param name="direction">
    /// World-space direction of the ray. The vector is normalized
    /// automatically.
    /// </param>
    public Ray3D(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction.Normalized();
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// World-space origin of the ray.
    /// </summary>
    public Vector3 Origin { get; }

    /// <summary>
    /// Normalized world-space direction of the ray.
    /// </summary>
    public Vector3 Direction { get; }

    #endregion

    #region METHODS

    /// <summary>
    /// Computes a point along the ray at the specified distance
    /// from the origin.
    /// </summary>
    /// <param name="distance">
    /// Distance from the ray origin.
    /// </param>
    /// <returns>
    /// World-space position located along the ray.
    /// </returns>
    public Vector3 GetPoint(float distance)
    {
        return Origin + (Direction * distance);
    }

    #endregion
}
