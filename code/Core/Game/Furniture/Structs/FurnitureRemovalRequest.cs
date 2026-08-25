using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Immutable request to remove the authoritative furniture occupying a target map cell.
    /// </summary>
    public readonly struct FurnitureRemovalRequest : IEquatable<FurnitureRemovalRequest>
    {
        #region Initialization

        /// <summary>
        /// Initializes a new furniture removal request.
        /// </summary>
        /// <param name="targetCell">A map cell that may be occupied by the furniture to remove.</param>
        public FurnitureRemovalRequest(MapCellCoord targetCell)
        {
            TargetCell = targetCell;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the target map cell to resolve through authoritative occupancy.
        /// </summary>
        public MapCellCoord TargetCell { get; }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two removal requests target the same map cell.
        /// </summary>
        /// <param name="left">The left-hand request.</param>
        /// <param name="right">The right-hand request.</param>
        /// <returns><c>true</c> when both requests are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(FurnitureRemovalRequest left, FurnitureRemovalRequest right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two removal requests target different map cells.
        /// </summary>
        /// <param name="left">The left-hand request.</param>
        /// <param name="right">The right-hand request.</param>
        /// <returns><c>true</c> when the requests are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(FurnitureRemovalRequest left, FurnitureRemovalRequest right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether this removal request equals another request.
        /// </summary>
        /// <param name="other">The request to compare.</param>
        /// <returns><c>true</c> when both requests target the same cell; otherwise, <c>false</c>.</returns>
        public bool Equals(FurnitureRemovalRequest other)
        {
            return TargetCell.Equals(other.TargetCell);
        }

        /// <summary>
        /// Determines whether this removal request equals the specified object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> when <paramref name="obj"/> is an equal removal request; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is FurnitureRemovalRequest other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code suitable for hash-based collections.
        /// </summary>
        /// <returns>A hash code derived from the target cell.</returns>
        public override int GetHashCode()
        {
            return TargetCell.GetHashCode();
        }

        #endregion
    }
}
