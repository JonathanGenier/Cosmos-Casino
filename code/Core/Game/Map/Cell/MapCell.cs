using CosmosCasino.Core.Game.Build.Domain;

namespace CosmosCasino.Core.Game.Map.Cell
{
    /// <summary>
    /// Represents a single cell within a map, tracking the presence of a floor and a wall.
    /// </summary>
    /// <remarks>A MapCell provides methods to validate and perform operations for placing or removing floors
    /// and walls. The state of each cell is determined by the HasFloor and HasWall properties. Operations should be
    /// validated using the corresponding validation methods before attempting to modify the cell's state. This type is
    /// intended for internal use within map-building logic.</remarks>
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
        /// Validates whether a floor can be placed in the current map cell.
        /// </summary>
        /// <returns>A <see cref="MapCellValidationResult"/> indicating whether placing a floor is valid. Returns a result
        /// representing a no-op if the cell already has a floor; otherwise, returns a valid result.</returns>
        internal MapCellValidationResult ValidatePlaceFloor()
        {
            if (HasFloor)
            {
                return MapCellValidationResult.NoOp();
            }

            return MapCellValidationResult.Valid();
        }

        /// <summary>
        /// Marks the cell as having a floor if the specified validation result indicates a valid operation.
        /// </summary>
        /// <param name="validation">The result of the map cell validation to check before placing the floor. Must have an outcome of
        /// BuildOperationOutcome.Valid.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid operation.</exception>
        internal void PlaceFloor(MapCellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place floor: validation result is not valid.");
            }

            HasFloor = true;
        }

        /// <summary>
        /// Validates whether the floor can be removed from the current map cell.
        /// </summary>
        /// <remarks>The operation is considered invalid if the cell contains a wall. If the cell does not
        /// have a floor, the operation is treated as a no-op.</remarks>
        /// <returns>A <see cref="MapCellValidationResult"/> indicating whether the floor removal operation is valid, invalid, or
        /// has no effect.</returns>
        internal MapCellValidationResult ValidateRemoveFloor()
        {
            if (!HasFloor)
            {
                return MapCellValidationResult.NoOp();
            }

            if (HasWall)
            {
                return MapCellValidationResult.Invalid(BuildOperationFailureReason.Blocked);
            }

            return MapCellValidationResult.Valid();
        }

        /// <summary>
        /// Removes the floor from the current map cell if the specified validation result indicates a valid operation.
        /// </summary>
        /// <param name="validation">The validation result that determines whether the floor can be removed. Must have an outcome of
        /// BuildOperationOutcome.Valid.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid operation.</exception>
        internal void RemoveFloor(MapCellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove floor: validation result is not valid.");
            }

            HasFloor = false;
        }

        #endregion

        #region Wall Methods

        /// <summary>
        /// Validates whether a wall can be placed on the current map cell.
        /// </summary>
        /// <remarks>Use this method to check placement rules before attempting to add a wall to the cell.
        /// The result specifies if the operation is allowed or provides the reason it is not.</remarks>
        /// <returns>A <see cref="MapCellValidationResult"/> indicating whether placing a wall is valid, invalid due to missing
        /// floor, or a no-op if a wall already exists.</returns>
        internal MapCellValidationResult ValidatePlaceWall()
        {
            if (!HasFloor)
            {
                return MapCellValidationResult.Invalid(BuildOperationFailureReason.NoFloor);
            }

            if (HasWall)
            {
                return MapCellValidationResult.NoOp();
            }

            return MapCellValidationResult.Valid();
        }

        /// <summary>
        /// Attempts to place a wall on the map cell using the specified validation result.
        /// </summary>
        /// <param name="validation">The result of validating whether a wall can be placed on the map cell. Must indicate a valid outcome.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid outcome for wall placement.</exception>
        internal void PlaceWall(MapCellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place wall: validation result is not valid.");
            }

            HasWall = true;
        }

        /// <summary>
        /// Determines whether removing a wall from the current cell is a valid operation.
        /// </summary>
        /// <returns>A <see cref="MapCellValidationResult"/> indicating whether the wall can be removed. Returns a result
        /// representing a no-op if there is no wall to remove; otherwise, returns a valid result.</returns>
        internal MapCellValidationResult ValidateRemoveWall()
        {
            if (!HasWall)
            {
                return MapCellValidationResult.NoOp();
            }

            return MapCellValidationResult.Valid();
        }

        /// <summary>
        /// Removes the wall from the current cell if the specified validation result indicates the operation is valid.
        /// </summary>
        /// <param name="validation">A validation result that determines whether the wall can be removed. The operation proceeds only if the
        /// outcome is valid.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid outcome.</exception>
        internal void RemoveWall(MapCellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove wall: validation result is not valid.");
            }

            HasWall = false;
        }

        #endregion
    }
}