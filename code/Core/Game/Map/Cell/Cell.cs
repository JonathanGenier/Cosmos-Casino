using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents one sparse logical map cell at a specific global X/Y/Z coordinate.
    /// </summary>
    internal sealed class Cell
    {
        #region Fields

        private readonly CellLayer _contents = new();

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new map cell at the specified global coordinate.
        /// </summary>
        /// <param name="coord">The global X/Y/Z coordinate identifying this cell.</param>
        internal Cell(MapCellCoord coord)
        {
            Coord = coord;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the global X/Y/Z coordinate that uniquely identifies this cell.
        /// </summary>
        internal MapCellCoord Coord { get; }

        /// <summary>
        /// Gets a value indicating whether this cell contains no authoritative state.
        /// </summary>
        internal bool IsEmpty => _contents.IsEmpty;

        #endregion

        #region Cell Content Methods

        /// <summary>
        /// Determines whether this cell contains a floor.
        /// </summary>
        /// <returns><see langword="true"/> when the cell contains a floor; otherwise <see langword="false"/>.</returns>
        internal bool HasFloor()
        {
            return _contents.HasFloor;
        }

        /// <summary>
        /// Determines whether this cell contains a floor at the specified legacy elevation.
        /// </summary>
        /// <param name="elevation">The legacy elevation to query.</param>
        /// <returns><see langword="true"/> when this cell matches <paramref name="elevation"/> and contains a floor.</returns>
        internal bool HasFloorAt(Elevation elevation)
        {
            return Coord.Y == elevation.MapCellY && HasFloor();
        }

        /// <summary>
        /// Determines whether this cell contains a wall.
        /// </summary>
        /// <returns><see langword="true"/> when the cell contains a wall; otherwise <see langword="false"/>.</returns>
        internal bool HasWall()
        {
            return _contents.HasWall;
        }

        /// <summary>
        /// Determines whether this cell contains a wall at the specified legacy elevation.
        /// </summary>
        /// <param name="elevation">The legacy elevation to query.</param>
        /// <returns><see langword="true"/> when this cell matches <paramref name="elevation"/> and contains a wall.</returns>
        internal bool HasWallAt(Elevation elevation)
        {
            return Coord.Y == elevation.MapCellY && HasWall();
        }

        #endregion

        #region Floor Validation Methods

        /// <summary>
        /// Validates placing a floor in this cell without mutating state.
        /// </summary>
        /// <returns>A no-op result when a floor already exists; otherwise a valid result.</returns>
        internal CellValidationResult ValidatePlaceFloor()
        {
            if (_contents.HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Validates removing a floor from this cell without mutating state.
        /// </summary>
        /// <returns>
        /// A no-op result when no floor exists, an invalid result when a wall blocks removal, or a valid result.
        /// </returns>
        internal CellValidationResult ValidateRemoveFloor()
        {
            if (!_contents.HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            if (_contents.HasWall)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.Blocked);
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Floor Operation Methods

        /// <summary>
        /// Places a floor after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing placement.</param>
        /// <param name="floor">The floor object to place.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell already has a floor.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="floor"/> is null.</exception>
        internal void PlaceFloor(CellValidationResult validation, Floor floor)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place floor: validation result is not valid.");
            }

            _contents.PlaceFloor(floor);
        }

        /// <summary>
        /// Removes the floor after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing removal.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell invariant is violated.</exception>
        internal void RemoveFloor(CellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove floor: validation result is not valid.");
            }

            _contents.RemoveFloor();
        }

        #endregion

        #region Wall Validation Methods

        /// <summary>
        /// Validates placing a wall in this cell without mutating state.
        /// </summary>
        /// <returns>
        /// An invalid result when no floor exists, a no-op result when a wall already exists, or a valid result.
        /// </returns>
        internal CellValidationResult ValidatePlaceWall()
        {
            if (!_contents.HasFloor)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.NoFloor);
            }

            if (_contents.HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Validates removing a wall from this cell without mutating state.
        /// </summary>
        /// <returns>A no-op result when no wall exists; otherwise a valid result.</returns>
        internal CellValidationResult ValidateRemoveWall()
        {
            if (!_contents.HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Wall Operation Methods

        /// <summary>
        /// Places a wall after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing placement.</param>
        /// <param name="wall">The wall object to place.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell invariant is violated.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wall"/> is null.</exception>
        internal void PlaceWall(CellValidationResult validation, Wall wall)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place wall: validation result is not valid.");
            }

            _contents.PlaceWall(wall);
        }

        /// <summary>
        /// Removes the wall after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing removal.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell invariant is violated.</exception>
        internal void RemoveWall(CellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove wall: validation result is not valid.");
            }

            _contents.RemoveWall();
        }

        #endregion
    }
}
