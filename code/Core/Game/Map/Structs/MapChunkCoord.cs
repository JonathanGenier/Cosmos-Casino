namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Immutable global chunk address for an authoritative map chunk on the horizontal X/Z plane.
    /// </summary>
    public readonly struct MapChunkCoord : IEquatable<MapChunkCoord>
    {
        #region Fields

        /// <summary>
        /// Global map-chunk X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Global map-chunk Z coordinate.
        /// </summary>
        public readonly int Z;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="MapChunkCoord"/> with the specified unbounded global chunk coordinates.
        /// </summary>
        /// <param name="x">Global map-chunk X coordinate.</param>
        /// <param name="z">Global map-chunk Z coordinate.</param>
        public MapChunkCoord(int x, int z)
        {
            X = x;
            Z = z;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two map chunk coordinates are equal.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MapChunkCoord left, MapChunkCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two map chunk coordinates are different.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if the coordinates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MapChunkCoord left, MapChunkCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate equals another <see cref="MapChunkCoord"/>.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(MapChunkCoord other)
        {
            return X == other.X && Z == other.Z;
        }

        /// <summary>
        /// Determines whether this coordinate equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> if the object is a matching <see cref="MapChunkCoord"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is MapChunkCoord other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code suitable for hash-based collections.
        /// </summary>
        /// <returns>A hash code derived from the coordinate components.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }

        /// <summary>
        /// Returns a human-readable string representation of the coordinate.
        /// </summary>
        /// <returns>A string in the format <c>(X, Z)</c>.</returns>
        public override string ToString()
        {
            return $"({X}, {Z})";
        }

        #endregion
    }
}
