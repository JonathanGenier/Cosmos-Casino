namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Immutable value type representing the legacy 2D horizontal map coordinate used by current terrain and build
    /// APIs. Use <see cref="MapCellCoord"/> for global 3D occupancy coordinates.
    /// </summary>
    public readonly struct MapCoord : IEquatable<MapCoord>
    {
        #region FIELDS

        /// <summary>
        /// X-axis coordinate of the cell.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Legacy second horizontal coordinate of the cell, aligned with terrain tile Y and client world Z.
        /// </summary>
        public readonly int Y;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new <see cref="MapCoord"/> with the specified
        /// grid coordinates.
        /// </summary>
        /// <param name="x">X-axis coordinate.</param>
        /// <param name="y">Legacy second horizontal coordinate.</param>
        public MapCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Determines whether two <see cref="MapCoord"/> values represent
        /// the same grid coordinate.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns>
        /// <c>true</c> if both coordinates are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(MapCoord left, MapCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="MapCoord"/> values represent
        /// different grid coordinates.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns>
        /// <c>true</c> if the coordinates are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(MapCoord left, MapCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate is equal to another
        /// <see cref="MapCoord"/> instance.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns>
        /// <c>true</c> if all coordinate components match; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(MapCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Determines whether this coordinate is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the object is a <see cref="MapCoord"/> with matching
        /// values; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj is MapCoord other && Equals(other);
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
            return HashCode.Combine(X, Y);
        }

        /// <summary>
        /// Returns a human-readable string representation of the coordinate.
        /// </summary>
        /// <returns>
        /// A string in the format <c>(X, Y)</c>.
        /// </returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        #endregion
    }
}
