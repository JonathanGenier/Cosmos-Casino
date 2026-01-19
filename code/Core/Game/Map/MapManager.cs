using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using CosmosCasino.Core.Game.Map.Grid;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Provides methods for managing map cells, including operations for placing and removing floors and walls, and
    /// querying cell state within a grid-based map.
    /// </summary>
    /// <remarks>The MapManager type is intended for internal use and encapsulates logic for manipulating the
    /// contents of a map grid. It supports sparse storage by cleaning up empty cells after destructive operations. All
    /// operations are performed relative to map cell coordinates. Thread safety is not guaranteed; callers should
    /// ensure appropriate synchronization if used concurrently.</remarks>
    internal sealed class MapManager
    {
        #region Fields

        private readonly MapGrid _grid = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total number of cells currently stored in the grid.
        /// </summary>
        internal int CellCount => _grid.CellCount;

        #endregion

        #region Floor Methods

        /// <summary>
        /// Determines whether the specified map cell contains a floor.
        /// </summary>
        /// <param name="coord">The coordinates of the map cell to check for a floor.</param>
        /// <returns>true if the cell at the specified coordinates exists and contains a floor; otherwise, false.</returns>
        internal bool HasFloor(MapCellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.HasFloor;
        }

        /// <summary>
        /// Determines whether a floor can be placed at the specified map cell coordinate.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell to check for floor placement eligibility.</param>
        /// <returns>A BuildOperationResult indicating whether a floor can be placed at the specified coordinate, including
        /// validation details if placement is not allowed.</returns>
        internal BuildOperationResult CanPlaceFloor(MapCellCoord coord)
        {
            if (TryGetCell(coord, out var cell))
            {
                var validationResult = cell.ValidatePlaceFloor();
                return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
            }

            return BuildOperationResult.Valid(coord);
        }

        /// <summary>
        /// Attempts to place a floor at the specified map cell coordinate.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell where the floor should be placed.</param>
        /// <returns>A BuildOperationResult indicating the outcome of the floor placement attempt, including validation details.</returns>
        internal BuildOperationResult TryPlaceFloor(MapCellCoord coord)
        {
            var cell = _grid.GetOrCreateCell(coord);
            var validationResult = cell.ValidatePlaceFloor();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.PlaceFloor(validationResult);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
        }

        /// <summary>
        /// Determines whether the floor can be removed at the specified map cell coordinate and returns the result of
        /// the validation.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell to check for floor removal eligibility.</param>
        /// <returns>A BuildOperationResult indicating whether the floor can be removed at the specified coordinate. Returns a
        /// result with details if the operation is valid or not, or a no-op result if the cell does not exist.</returns>
        internal BuildOperationResult CanRemoveFloor(MapCellCoord coord)
        {
            if (TryGetCell(coord, out var cell))
            {
                var validationResult = cell.ValidateRemoveFloor();
                return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
            }

            return BuildOperationResult.NoOp(coord);
        }

        /// <summary>
        /// Attempts to remove the floor from the specified map cell coordinate.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell from which to attempt floor removal.</param>
        /// <returns>A result indicating the outcome of the remove floor operation for the specified cell. Returns a no-op result
        /// if the cell does not exist at the given coordinate.</returns>
        internal BuildOperationResult TryRemoveFloor(MapCellCoord coord)
        {
            if (TryGetCell(coord, out var cell))
            {
                var validationResult = cell.ValidateRemoveFloor();

                if (validationResult.Outcome == BuildOperationOutcome.Valid)
                {
                    cell.RemoveFloor(validationResult);
                    _grid.RemoveCell(coord);
                }

                return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
            }

            return BuildOperationResult.NoOp(coord);
        }

        #endregion

        #region Wall Methods

        /// <summary>
        /// Determines whether the specified cell contains a wall.
        /// </summary>
        /// <param name="coord">The coordinates of the cell to check for a wall.</param>
        /// <returns>true if the cell at the specified coordinates contains a wall; otherwise, false.</returns>
        internal bool HasWall(MapCellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.HasWall;
        }

        /// <summary>
        /// Determines whether a wall can be placed at the specified map cell coordinate and returns the result of the
        /// validation.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell where the wall placement is to be validated.</param>
        /// <returns>A BuildOperationResult indicating whether the wall can be placed at the specified coordinate. The result
        /// includes the reason for failure if the operation is not valid.</returns>
        internal BuildOperationResult CanPlaceWall(MapCellCoord coord)
        {
            if (TryGetCell(coord, out var cell))
            {
                var validationResult = cell.ValidatePlaceWall();
                return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
            }

            return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
        }

        /// <summary>
        /// Attempts to place a wall at the specified map cell coordinate and returns the result of the operation.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell where the wall should be placed.</param>
        /// <returns>A BuildOperationResult indicating whether the wall placement was successful, including the outcome and
        /// failure reason if applicable.</returns>
        internal BuildOperationResult TryPlaceWall(MapCellCoord coord)
        {
            if (TryGetCell(coord, out var cell))
            {
                var validationResult = cell.ValidatePlaceWall();

                if (validationResult.Outcome == BuildOperationOutcome.Valid)
                {
                    cell.PlaceWall(validationResult);
                }

                return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
            }

            return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
        }

        /// <summary>
        /// Determines whether a wall can be removed at the specified map cell coordinate and returns the result of the
        /// validation.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell to check for wall removal eligibility.</param>
        /// <returns>A BuildOperationResult indicating whether the wall can be removed at the specified coordinate. The result
        /// contains details about the validation outcome.</returns>
        internal BuildOperationResult CanRemoveWall(MapCellCoord coord)
        {
            if (TryGetCell(coord, out var cell))
            {
                var validationResult = cell.ValidateRemoveWall();
                return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
            }

            return BuildOperationResult.NoOp(coord);
        }

        /// <summary>
        /// Attempts to remove a wall from the specified map cell and returns the result of the operation.
        /// </summary>
        /// <param name="coord">The coordinates of the map cell from which to attempt to remove the wall.</param>
        /// <returns>A BuildOperationResult indicating the outcome of the wall removal attempt. Returns a result with outcome
        /// NoOp if the specified cell does not exist.</returns>
        internal BuildOperationResult TryRemoveWall(MapCellCoord coord)
        {
            if (TryGetCell(coord, out var cell))
            {
                var validationResult = cell.ValidateRemoveWall();

                if (validationResult.Outcome == BuildOperationOutcome.Valid)
                {
                    cell.RemoveWall(validationResult);
                }

                return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
            }

            return BuildOperationResult.NoOp(coord);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Attempts to retrieve the cell at the specified coordinate.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell to retrieve.
        /// </param>
        /// <param name="cell">
        /// When this method returns <c>true</c>, contains the retrieved cell;
        /// otherwise, <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists; otherwise, <c>false</c>.
        /// </returns>
        private bool TryGetCell(MapCellCoord coord, [NotNullWhen(true)] out MapCell? cell)
        {
            cell = _grid.GetCell(coord);
            return cell != null;
        }

        #endregion
    }
}