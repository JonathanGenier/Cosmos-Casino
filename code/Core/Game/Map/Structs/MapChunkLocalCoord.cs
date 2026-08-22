namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Immutable zero-based X/Z coordinate local to one authoritative map chunk.
    /// </summary>
    public readonly struct MapChunkLocalCoord : IEquatable<MapChunkLocalCoord>
    {
        #region Fields

        /// <summary>
        /// Local X index inside a map chunk.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Local Z index inside a map chunk.
        /// </summary>
        public readonly int Z;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="MapChunkLocalCoord"/> with indexes inside the authoritative map chunk size.
        /// </summary>
        /// <param name="x">Local X index inside the chunk.</param>
        /// <param name="z">Local Z index inside the chunk.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when either coordinate is outside the range <c>[0, MapChunkMetrics.ChunkSize)</c>.
        /// </exception>
        public MapChunkLocalCoord(int x, int z)
        {
            ValidateIndex(x, nameof(x));
            ValidateIndex(z, nameof(z));

            X = x;
            Z = z;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two map chunk-local coordinates are equal.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MapChunkLocalCoord left, MapChunkLocalCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two map chunk-local coordinates are different.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if the coordinates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MapChunkLocalCoord left, MapChunkLocalCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate equals another <see cref="MapChunkLocalCoord"/>.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(MapChunkLocalCoord other)
        {
            return X == other.X && Z == other.Z;
        }

        /// <summary>
        /// Determines whether this coordinate equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> if the object is a matching <see cref="MapChunkLocalCoord"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is MapChunkLocalCoord other && Equals(other);
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

        #region Validation

        private static void ValidateIndex(int value, string paramName)
        {
            if (value < 0 || value >= MapChunkMetrics.ChunkSize)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    value,
                    $"Map chunk-local coordinates must be within [0, {MapChunkMetrics.ChunkSize}).");
            }
        }

        #endregion
    }
}
