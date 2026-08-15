namespace CosmosCasino.Core.Game.Map.Terrain
{
    /// <summary>
    /// Immutable value type representing a terrain chunk coordinate in signed chunk-grid space.
    /// </summary>
    public readonly struct TerrainChunkGridCoord : IEquatable<TerrainChunkGridCoord>
    {
        #region Fields

        /// <summary>
        /// Chunk-grid X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Chunk-grid Y coordinate.
        /// </summary>
        public readonly int Y;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="TerrainChunkGridCoord"/> with the specified signed chunk-grid coordinates.
        /// </summary>
        /// <param name="x">Chunk-grid X coordinate.</param>
        /// <param name="y">Chunk-grid Y coordinate.</param>
        public TerrainChunkGridCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two <see cref="TerrainChunkGridCoord"/> values are equal.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(TerrainChunkGridCoord left, TerrainChunkGridCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TerrainChunkGridCoord"/> values are different.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if the coordinates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(TerrainChunkGridCoord left, TerrainChunkGridCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate equals another <see cref="TerrainChunkGridCoord"/>.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(TerrainChunkGridCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Determines whether this coordinate equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the object is a <see cref="TerrainChunkGridCoord"/> with matching values; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj is TerrainChunkGridCoord other && Equals(other);
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
