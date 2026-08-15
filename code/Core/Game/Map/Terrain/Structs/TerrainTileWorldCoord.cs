namespace CosmosCasino.Core.Game.Map.Terrain
{
    /// <summary>
    /// Immutable value type representing a terrain tile coordinate in signed world-tile space.
    /// </summary>
    public readonly struct TerrainTileWorldCoord : IEquatable<TerrainTileWorldCoord>
    {
        #region Fields

        /// <summary>
        /// World-tile X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// World-tile Y coordinate.
        /// </summary>
        public readonly int Y;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="TerrainTileWorldCoord"/> with the specified signed world-tile coordinates.
        /// </summary>
        /// <param name="x">World-tile X coordinate.</param>
        /// <param name="y">World-tile Y coordinate.</param>
        public TerrainTileWorldCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two <see cref="TerrainTileWorldCoord"/> values are equal.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(TerrainTileWorldCoord left, TerrainTileWorldCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TerrainTileWorldCoord"/> values are different.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if the coordinates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(TerrainTileWorldCoord left, TerrainTileWorldCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate equals another <see cref="TerrainTileWorldCoord"/>.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(TerrainTileWorldCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Determines whether this coordinate equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the object is a <see cref="TerrainTileWorldCoord"/> with matching values; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj is TerrainTileWorldCoord other && Equals(other);
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
