namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Immutable value type representing a 3D grid coordinate used to
    /// identify a single cell within the map.
    /// </summary>
    internal readonly struct CellCoord : IEquatable<CellCoord>
    {
        #region FIELDS

        /// <summary>
        /// X-axis coordinate of the cell.
        /// </summary>
        internal readonly int X;

        /// <summary>
        /// Y-axis coordinate of the cell.
        /// </summary>
        internal readonly int Y;

        /// <summary>
        /// Z-axis (layer or elevation) coordinate of the cell.
        /// </summary>
        internal readonly int Z;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new <see cref="CellCoord"/> with the specified
        /// grid coordinates.
        /// </summary>
        /// <param name="x">X-axis coordinate.</param>
        /// <param name="y">Y-axis coordinate.</param>
        /// <param name="z">Z-axis (layer) coordinate.</param>
        internal CellCoord(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Determines whether two <see cref="CellCoord"/> values represent
        /// the same grid coordinate.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns>
        /// <c>true</c> if both coordinates are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(CellCoord left, CellCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="CellCoord"/> values represent
        /// different grid coordinates.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns>
        /// <c>true</c> if the coordinates are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(CellCoord left, CellCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate is equal to another
        /// <see cref="CellCoord"/> instance.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns>
        /// <c>true</c> if all coordinate components match; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(CellCoord other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Determines whether this coordinate is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the object is a <see cref="CellCoord"/> with matching
        /// values; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj is CellCoord other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code suitable for use in hash-based collections
        /// such as dictionaries and hash sets.
        /// </summary>
        /// <returns>
        /// A hash code derived from the coordinate components.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        /// <summary>
        /// Returns a human-readable string representation of the coordinate.
        /// </summary>
        /// <returns>
        /// A string in the format <c>(X, Y, Z)</c>.
        /// </returns>
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        #endregion
    }
}
