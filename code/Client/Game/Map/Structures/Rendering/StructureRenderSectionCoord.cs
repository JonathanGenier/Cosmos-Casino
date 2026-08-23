using System;

/// <summary>
/// Client-owned 3D render-section coordinate for generated structure geometry.
/// </summary>
internal readonly struct StructureRenderSectionCoord : IEquatable<StructureRenderSectionCoord>
{
    #region Initialization

    /// <summary>
    /// Initializes a new render-section coordinate.
    /// </summary>
    /// <param name="x">The section coordinate on the global X axis.</param>
    /// <param name="y">The section coordinate on the global Y axis.</param>
    /// <param name="z">The section coordinate on the global Z axis.</param>
    internal StructureRenderSectionCoord(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the section coordinate on the global X axis.
    /// </summary>
    internal int X { get; }

    /// <summary>
    /// Gets the section coordinate on the global Y axis.
    /// </summary>
    internal int Y { get; }

    /// <summary>
    /// Gets the section coordinate on the global Z axis.
    /// </summary>
    internal int Z { get; }

    #endregion

    #region Equality Operators

    /// <summary>
    /// Determines whether two section coordinates are equal.
    /// </summary>
    /// <param name="left">The left-hand coordinate.</param>
    /// <param name="right">The right-hand coordinate.</param>
    /// <returns><c>true</c> when both coordinates are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(StructureRenderSectionCoord left, StructureRenderSectionCoord right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two section coordinates are different.
    /// </summary>
    /// <param name="left">The left-hand coordinate.</param>
    /// <param name="right">The right-hand coordinate.</param>
    /// <returns><c>true</c> when the coordinates are different; otherwise, <c>false</c>.</returns>
    public static bool operator !=(StructureRenderSectionCoord left, StructureRenderSectionCoord right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Determines whether this coordinate equals another section coordinate.
    /// </summary>
    /// <param name="other">The coordinate to compare.</param>
    /// <returns><c>true</c> when both coordinates are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(StructureRenderSectionCoord other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    /// <summary>
    /// Determines whether this coordinate equals another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when the object is an equal section coordinate; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is StructureRenderSectionCoord other && Equals(other);
    }

    /// <summary>
    /// Gets a hash code derived from the coordinate components.
    /// </summary>
    /// <returns>A hash code suitable for hash-based collections.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    /// <summary>
    /// Returns a human-readable coordinate string.
    /// </summary>
    /// <returns>The coordinate in <c>(X, Y, Z)</c> form.</returns>
    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    #endregion
}
