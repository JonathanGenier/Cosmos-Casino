namespace CosmosCasino.Core.Game.Map.Cell
{
    /// <summary>
    /// Represents a single cell within a map that can contain a floor and/or a wall.
    /// </summary>
    /// <remarks>A MapCell tracks the presence of a floor and a wall independently. Operations on the cell,
    /// such as placing or removing a floor or wall, are subject to constraints (for example, a wall cannot be placed
    /// unless a floor is present). This type is intended for internal use within map construction or editing
    /// logic.</remarks>
    internal sealed class MapCell
    {
        #region PROPERTIES

        /// <summary>
        /// Gets a value indicating whether a floor is present.
        /// </summary>
        internal bool HasFloor { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a wall is present.
        /// </summary>
        internal bool HasWall { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MapCell"/> contains neither a floor nor a wall.
        /// </summary>
        internal bool IsEmpty => !HasFloor && !HasWall;

        #endregion

        #region Floor Methods

        /// <summary>
        /// Attempts to place a floor in the current cell if one is not already present.
        /// </summary>
        /// <returns>A <see cref="MapCellResult"/> indicating whether the floor was placed successfully or the reason for
        /// failure.</returns>
        internal MapCellResult TryPlaceFloor()
        {
            if (HasFloor)
            {
                return MapCellResult.Failed(MapCellFailureReason.Blocked);
            }

            HasFloor = true;
            return MapCellResult.Placed();
        }

        /// <summary>
        /// Attempts to remove the floor from the current map cell, if present and not blocked by a wall.
        /// </summary>
        /// <returns>A <see cref="MapCellResult"/> indicating the outcome of the operation. Returns <see
        /// cref="MapCellResult.Removed"/> if the floor was successfully removed; <see cref="MapCellResult.Skipped"/> if
        /// there was no floor to remove; or <see cref="MapCellResult.Failed"/> if the operation was blocked by a wall.</returns>
        internal MapCellResult TryRemoveFloor()
        {
            if (!HasFloor)
            {
                return MapCellResult.Skipped(MapCellFailureReason.NoFloor);
            }

            if (HasWall)
            {
                return MapCellResult.Failed(MapCellFailureReason.Blocked);
            }

            HasFloor = false;
            return MapCellResult.Removed();
        }

        #endregion

        #region Wall Methods

        /// <summary>
        /// Attempts to place a wall in the current map cell.
        /// </summary>
        /// <remarks>A wall can only be placed if the cell has a floor and does not already contain a
        /// wall.</remarks>
        /// <returns>A <see cref="MapCellResult"/> indicating the outcome of the operation. The result will indicate success if
        /// the wall was placed, or provide a failure reason if placement was not possible.</returns>
        internal MapCellResult TryPlaceWall()
        {
            if (!HasFloor)
            {
                return MapCellResult.Failed(MapCellFailureReason.NoFloor);
            }

            if (HasWall)
            {
                return MapCellResult.Skipped(MapCellFailureReason.Blocked);
            }

            HasWall = true;
            return MapCellResult.Placed();
        }

        /// <summary>
        /// Attempts to remove the wall from the current map cell.
        /// </summary>
        /// <returns>A <see cref="MapCellResult"/> indicating the outcome of the operation. Returns <see
        /// cref="MapCellResult.Removed"/> if the wall was successfully removed; otherwise, returns <see
        /// cref="MapCellResult.Skipped"/> with a failure reason if no wall was present.</returns>
        internal MapCellResult TryRemoveWall()
        {
            if (!HasWall)
            {
                return MapCellResult.Skipped(MapCellFailureReason.NoWall);
            }

            HasWall = false;
            return MapCellResult.Removed();
        }

        #endregion
    }
}