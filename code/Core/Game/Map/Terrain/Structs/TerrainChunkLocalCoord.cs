namespace CosmosCasino.Core.Game.Map.Terrain
{
    /// <summary>
    /// Immutable value type representing a zero-based tile coordinate inside a terrain chunk.
    /// </summary>
    public readonly struct TerrainChunkLocalCoord : IEquatable<TerrainChunkLocalCoord>
    {
        #region Fields

        /// <summary>
        /// Local tile X index inside a chunk.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Local tile Y index inside a chunk.
        /// </summary>
        public readonly int Y;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="TerrainChunkLocalCoord"/> with the specified chunk-local tile indexes.
        /// </summary>
        /// <param name="x">Local tile X index inside a chunk.</param>
        /// <param name="y">Local tile Y index inside a chunk.</param>
        public TerrainChunkLocalCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two <see cref="TerrainChunkLocalCoord"/> values are equal.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(TerrainChunkLocalCoord left, TerrainChunkLocalCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TerrainChunkLocalCoord"/> values are different.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if the coordinates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(TerrainChunkLocalCoord left, TerrainChunkLocalCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate equals another <see cref="TerrainChunkLocalCoord"/>.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(TerrainChunkLocalCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Determines whether this coordinate equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the object is a <see cref="TerrainChunkLocalCoord"/> with matching values; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj is TerrainChunkLocalCoord other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code suitable for hash-based collections.
        /// </summary>
        /// <returns>A hash code derived from the coordinate components.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        /// <summary>
        /// Returns a human-readable string representation of the coordinate.
        /// </summary>
        /// <returns>A string in the format <c>(X, Y)</c>.</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        #endregion
    }
}
