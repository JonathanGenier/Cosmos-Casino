using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents a single map cell that binds terrain data with buildable state,
    /// tracking the presence and validity of floors and walls at a given coordinate.
    /// </summary>
    internal sealed class Cell
    {
        #region Initialization

        /// <summary>
        /// Initializes a new map cell at the specified coordinate with its associated terrain tile.
        /// </summary>
        /// <param name="coord">The map coordinate identifying this cell.</param>
        /// <param name="terrainTile">The terrain tile providing elevation and slope data for the cell.</param>
        internal Cell(MapCoord coord, TerrainTile terrainTile)
        {
            Coord = coord;
            TerrainTile = terrainTile;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the map coordinate that uniquely identifies this cell.
        /// </summary>
        internal MapCoord Coord { get; }

        /// <summary>
        /// Gets the terrain tile backing this cell’s elevation and slope data.
        /// </summary>
        internal TerrainTile TerrainTile { get; }

        /// <summary>
        /// Gets a value indicating whether a floor is present.
        /// </summary>
        internal bool HasFloor { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a wall is present.
        /// </summary>
        internal bool HasWall { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Cell"/> contains neither a floor nor a wall.
        /// </summary>
        internal bool IsEmpty => !HasFloor && !HasWall;

        #endregion

        #region Floor Validation Methods

        /// <summary>
        /// Validates whether a floor can be placed in the current map cell.
        /// </summary>
        /// <returns>A <see cref="CellValidationResult"/> indicating whether placing a floor is valid. Returns a result
        /// representing a no-op if the cell already has a floor; otherwise, returns a valid result.</returns>
        internal CellValidationResult ValidatePlaceFloor()
        {
            if (HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Validates whether the floor can be removed from the current map cell.
        /// </summary>
        /// <remarks>The operation is considered invalid if the cell contains a wall. If the cell does not
        /// have a floor, the operation is treated as a no-op.</remarks>
        /// <returns>A <see cref="CellValidationResult"/> indicating whether the floor removal operation is valid, invalid, or
        /// has no effect.</returns>
        internal CellValidationResult ValidateRemoveFloor()
        {
            if (!HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            if (HasWall)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.Blocked);
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Floor Operation Methods

        /// <summary>
        /// Marks the cell as having a floor if the specified validation result indicates a valid operation.
        /// </summary>
        /// <param name="validation">The result of the map cell validation to check before placing the floor. Must have an outcome of
        /// BuildOperationOutcome.Valid.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid operation.</exception>
        internal void PlaceFloor(CellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place floor: validation result is not valid.");
            }

            HasFloor = true;
        }

        /// <summary>
        /// Removes the floor from the current map cell if the specified validation result indicates a valid operation.
        /// </summary>
        /// <param name="validation">The validation result that determines whether the floor can be removed. Must have an outcome of
        /// BuildOperationOutcome.Valid.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid operation.</exception>
        internal void RemoveFloor(CellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove floor: validation result is not valid.");
            }

            HasFloor = false;
        }

        #endregion

        #region Wall Validation Methods

        /// <summary>
        /// Validates whether a wall can be placed on the current map cell.
        /// </summary>
        /// <remarks>Use this method to check placement rules before attempting to add a wall to the cell.
        /// The result specifies if the operation is allowed or provides the reason it is not.</remarks>
        /// <returns>A <see cref="CellValidationResult"/> indicating whether placing a wall is valid, invalid due to missing
        /// floor, or a no-op if a wall already exists.</returns>
        internal CellValidationResult ValidatePlaceWall()
        {
            if (!HasFloor)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.NoFloor);
            }

            if (HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Determines whether removing a wall from the current cell is a valid operation.
        /// </summary>
        /// <returns>A <see cref="CellValidationResult"/> indicating whether the wall can be removed. Returns a result
        /// representing a no-op if there is no wall to remove; otherwise, returns a valid result.</returns>
        internal CellValidationResult ValidateRemoveWall()
        {
            if (!HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Wall Operation Methods

        /// <summary>
        /// Attempts to place a wall on the map cell using the specified validation result.
        /// </summary>
        /// <param name="validation">The result of validating whether a wall can be placed on the map cell. Must indicate a valid outcome.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid outcome for wall placement.</exception>
        internal void PlaceWall(CellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place wall: validation result is not valid.");
            }

            HasWall = true;
        }

        /// <summary>
        /// Removes the wall from the current cell if the specified validation result indicates the operation is valid.
        /// </summary>
        /// <param name="validation">A validation result that determines whether the wall can be removed. The operation proceeds only if the
        /// outcome is valid.</param>
        /// <exception cref="InvalidOperationException">Thrown if the validation result does not indicate a valid outcome.</exception>
        internal void RemoveWall(CellValidationResult validation)
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