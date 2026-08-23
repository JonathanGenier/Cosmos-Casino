namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Immutable global logical cell coordinate in authoritative X/Y/Z map space.
    /// </summary>
    public readonly partial struct MapCellCoord : IEquatable<MapCellCoord>
    {
        #region Fields

        /// <summary>
        /// Global horizontal X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Global vertical Y coordinate.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Global horizontal Z coordinate.
        /// </summary>
        public readonly int Z;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="MapCellCoord"/> with the specified global logical coordinates.
        /// </summary>
        /// <param name="x">Global horizontal X coordinate.</param>
        /// <param name="y">Global vertical Y coordinate.</param>
        /// <param name="z">Global horizontal Z coordinate.</param>
        public MapCellCoord(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two map cell coordinates are equal.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MapCellCoord left, MapCellCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two map cell coordinates are different.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if the coordinates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MapCellCoord left, MapCellCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate equals another <see cref="MapCellCoord"/>.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(MapCellCoord other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Determines whether this coordinate equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> if the object is a matching <see cref="MapCellCoord"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is MapCellCoord other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code suitable for hash-based collections.
        /// </summary>
        /// <returns>A hash code derived from the coordinate components.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        /// <summary>
        /// Returns a human-readable string representation of the coordinate.
        /// </summary>
        /// <returns>A string in the format <c>(X, Y, Z)</c>.</returns>
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        #endregion
    }
}
