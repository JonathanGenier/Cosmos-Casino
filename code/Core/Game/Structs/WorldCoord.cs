namespace CosmosCasino.Core.Game
{
    /// <summary>
    /// Engine-agnostic horizontal world coordinate.
    /// X maps to world X; Y maps to the horizontal world axis used by map rows and terrain tiles.
    /// </summary>
    public readonly struct WorldCoord : IEquatable<WorldCoord>
    {
        #region Fields

        /// <summary>
        /// Horizontal world X coordinate.
        /// </summary>
        public readonly float X;

        /// <summary>
        /// Horizontal world Y coordinate.
        /// </summary>
        public readonly float Y;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="WorldCoord"/> with the specified horizontal world coordinates.
        /// </summary>
        /// <param name="x">
        /// Horizontal world X coordinate.
        /// </param>
        /// <param name="y">
        /// Horizontal world Y coordinate.
        /// </param>
        public WorldCoord(float x, float y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two <see cref="WorldCoord"/> values are equal.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(WorldCoord left, WorldCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="WorldCoord"/> values are different.
        /// </summary>
        /// <param name="left">The left-hand coordinate.</param>
        /// <param name="right">The right-hand coordinate.</param>
        /// <returns><c>true</c> if the coordinates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(WorldCoord left, WorldCoord right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this coordinate equals another <see cref="WorldCoord"/>.
        /// </summary>
        /// <param name="other">The coordinate to compare against.</param>
        /// <returns><c>true</c> if both coordinates are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(WorldCoord other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        /// <summary>
        /// Determines whether this coordinate equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the object is a <see cref="WorldCoord"/> with matching values; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj is WorldCoord other && Equals(other);
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
