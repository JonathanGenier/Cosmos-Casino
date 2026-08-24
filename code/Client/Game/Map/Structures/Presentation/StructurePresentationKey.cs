using System;

/// <summary>
/// Client-owned key for grouping compatible repeated structure presentations.
/// </summary>
internal readonly struct StructurePresentationKey : IEquatable<StructurePresentationKey>
{
    #region Initialization

    /// <summary>
    /// Initializes a new presentation key.
    /// </summary>
    /// <param name="value">The stable Client presentation key value.</param>
    internal StructurePresentationKey(int value)
    {
        Value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the stable Client presentation key value.
    /// </summary>
    internal int Value { get; }

    #endregion

    #region Equality Operators

    /// <summary>
    /// Determines whether two presentation keys are equal.
    /// </summary>
    /// <param name="left">The left presentation key.</param>
    /// <param name="right">The right presentation key.</param>
    /// <returns><c>true</c> when both keys are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(StructurePresentationKey left, StructurePresentationKey right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two presentation keys are different.
    /// </summary>
    /// <param name="left">The left presentation key.</param>
    /// <param name="right">The right presentation key.</param>
    /// <returns><c>true</c> when the keys are different; otherwise, <c>false</c>.</returns>
    public static bool operator !=(StructurePresentationKey left, StructurePresentationKey right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Determines whether this presentation key equals another key.
    /// </summary>
    /// <param name="other">The other presentation key.</param>
    /// <returns><c>true</c> when both keys are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(StructurePresentationKey other)
    {
        return Value == other.Value;
    }

    /// <summary>
    /// Determines whether this presentation key equals another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when the object is an equal presentation key; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is StructurePresentationKey other && Equals(other);
    }

    /// <summary>
    /// Gets a hash code derived from the presentation key value.
    /// </summary>
    /// <returns>A hash code suitable for hash-based collections.</returns>
    public override int GetHashCode()
    {
        return Value;
    }

    /// <summary>
    /// Returns a human-readable presentation key string.
    /// </summary>
    /// <returns>The presentation key value.</returns>
    public override string ToString()
    {
        return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    #endregion
}
