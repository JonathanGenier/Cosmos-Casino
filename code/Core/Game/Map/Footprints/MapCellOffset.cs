namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Immutable offset from a footprint anchor in authoritative X/Y/Z map-cell space.
    /// </summary>
    public readonly struct MapCellOffset : IEquatable<MapCellOffset>
    {
        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="MapCellOffset"/> with the specified relative offsets.
        /// </summary>
        /// <param name="x">Horizontal X offset.</param>
        /// <param name="y">Vertical Y offset.</param>
        /// <param name="z">Horizontal Z offset.</param>
        public MapCellOffset(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the horizontal X offset.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the vertical Y offset.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the horizontal Z offset.
        /// </summary>
        public int Z { get; }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two map-cell offsets are equal.
        /// </summary>
        /// <param name="left">The left-hand offset.</param>
        /// <param name="right">The right-hand offset.</param>
        /// <returns><c>true</c> if both offsets are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MapCellOffset left, MapCellOffset right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two map-cell offsets are different.
        /// </summary>
        /// <param name="left">The left-hand offset.</param>
        /// <param name="right">The right-hand offset.</param>
        /// <returns><c>true</c> if the offsets are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MapCellOffset left, MapCellOffset right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this offset equals another <see cref="MapCellOffset"/>.
        /// </summary>
        /// <param name="other">The offset to compare against.</param>
        /// <returns><c>true</c> if both offsets are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(MapCellOffset other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Determines whether this offset equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> if the object is a matching <see cref="MapCellOffset"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is MapCellOffset other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code suitable for hash-based collections.
        /// </summary>
        /// <returns>A hash code derived from the offset components.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        /// <summary>
        /// Returns a human-readable string representation of the offset.
        /// </summary>
        /// <returns>A string in the format <c>(X, Y, Z)</c>.</returns>
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotates this offset around the vertical Y axis.
        /// </summary>
        /// <param name="rotation">The quarter-turn rotation to apply.</param>
        /// <returns>The rotated offset.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the rotation is unsupported or a rotated axis cannot be represented.
        /// </exception>
        internal MapCellOffset Rotate(FootprintRotation rotation)
        {
            return rotation switch
            {
                FootprintRotation.Deg0 => this,
                FootprintRotation.Deg90 => new MapCellOffset(Z, Y, NegateAxis(X, nameof(X))),
                FootprintRotation.Deg180 => new MapCellOffset(NegateAxis(X, nameof(X)), Y, NegateAxis(Z, nameof(Z))),
                FootprintRotation.Deg270 => new MapCellOffset(NegateAxis(Z, nameof(Z)), Y, X),
                _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, "Unsupported footprint rotation."),
            };
        }

        #endregion

        #region Helpers

        private static int NegateAxis(int value, string paramName)
        {
            long negated = -(long)value;

            if (negated > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    value,
                    $"Rotated map-cell offsets must be within [{int.MinValue}, {int.MaxValue}].");
            }

            return (int)negated;
        }

        #endregion
    }
}
