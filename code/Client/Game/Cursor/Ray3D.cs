using Godot;

/// <summary>
/// Represents a ray in three-dimensional space, defined by an origin point and a normalized direction vector.
/// </summary>
/// <remarks>A Ray3D is commonly used in geometric computations such as ray casting, intersection tests, and
/// spatial queries. The direction vector is always normalized to ensure consistent behavior in calculations.</remarks>
public readonly struct Ray3D
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance of the Ray3D class with the specified origin and direction.
    /// </summary>
    /// <param name="origin">The starting point of the ray in 3D space.</param>
    /// <param name="direction">The direction vector of the ray. The vector is normalized before being assigned.</param>
    public Ray3D(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction.Normalized();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the origin point of the coordinate system represented by this instance.
    /// </summary>
    public Vector3 Origin { get; }

    /// <summary>
    /// Gets the direction vector represented by this instance.
    /// </summary>
    public Vector3 Direction { get; }

    #endregion

    #region Get

    /// <summary>
    /// Calculates the point located at a specified distance along the ray.
    /// </summary>
    /// <param name="distance">The distance from the origin at which to calculate the point. Positive values extend in the direction of the
    /// ray; negative values extend in the opposite direction.</param>
    /// <returns>A <see cref="Vector3"/> representing the point at the specified distance from the origin along the ray.</returns>
    public Vector3 GetPoint(float distance)
    {
        return Origin + (Direction * distance);
    }

    #endregion
}
