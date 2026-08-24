using System;

/// <summary>
/// Key for one repeated-structure MultiMesh batch.
/// </summary>
internal readonly struct StructureInstanceBatchKey : IEquatable<StructureInstanceBatchKey>
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance batch key.
    /// </summary>
    /// <param name="presentationKey">The repeated presentation key.</param>
    /// <param name="sectionCoord">The render section coordinate owning the batch.</param>
    internal StructureInstanceBatchKey(
        StructurePresentationKey presentationKey,
        StructureRenderSectionCoord sectionCoord)
    {
        PresentationKey = presentationKey;
        SectionCoord = sectionCoord;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the repeated presentation key.
    /// </summary>
    internal StructurePresentationKey PresentationKey { get; }

    /// <summary>
    /// Gets the render section coordinate owning the batch.
    /// </summary>
    internal StructureRenderSectionCoord SectionCoord { get; }

    #endregion

    #region Equality Operators

    /// <summary>
    /// Determines whether two keys are equal.
    /// </summary>
    /// <param name="left">The left key.</param>
    /// <param name="right">The right key.</param>
    /// <returns><c>true</c> when both keys are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(StructureInstanceBatchKey left, StructureInstanceBatchKey right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two keys are different.
    /// </summary>
    /// <param name="left">The left key.</param>
    /// <param name="right">The right key.</param>
    /// <returns><c>true</c> when the keys are different; otherwise, <c>false</c>.</returns>
    public static bool operator !=(StructureInstanceBatchKey left, StructureInstanceBatchKey right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Determines whether this key equals another key.
    /// </summary>
    /// <param name="other">The other key.</param>
    /// <returns><c>true</c> when both keys are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(StructureInstanceBatchKey other)
    {
        return PresentationKey == other.PresentationKey
            && SectionCoord == other.SectionCoord;
    }

    /// <summary>
    /// Determines whether this key equals another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when the object is an equal key; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is StructureInstanceBatchKey other && Equals(other);
    }

    /// <summary>
    /// Gets a hash code derived from the presentation key and section coordinate.
    /// </summary>
    /// <returns>A hash code suitable for hash-based collections.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(PresentationKey, SectionCoord);
    }

    /// <summary>
    /// Returns a human-readable key string.
    /// </summary>
    /// <returns>The presentation key and section coordinate.</returns>
    public override string ToString()
    {
        return $"{PresentationKey}@{SectionCoord}";
    }

    #endregion
}
