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
        /// <returns>true if the cell at the specified coordinates contains a floor; otherwise, false.</returns>
        internal bool HasFloor(MapCellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.HasFloor;
        }

        /// <summary>
        /// Attempts to place a floor at the specified map cell coordinate.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell where the floor should be placed.</param>
        /// <returns>A MapOperationResult indicating the outcome of the floor placement operation at the specified coordinate.</returns>
        internal MapOperationResult TryPlaceFloor(MapCellCoord coord)
        {
            var cell = _grid.GetOrCreateCell(coord);
            var cellResult = cell.TryPlaceFloor();
            return MapOperationResult.FromMapCellResult(coord, cellResult);
        }

        /// <summary>
        /// Attempts to remove the floor from the specified map cell, if it exists.
        /// </summary>
        /// <param name="coord">The coordinates of the map cell from which to remove the floor.</param>
        /// <returns>A MapOperationResult indicating the outcome of the operation, including whether the floor was removed.</returns>
        internal MapOperationResult TryRemoveFloor(MapCellCoord coord)
        {
            var result = WithExistingCell(coord, cell => cell.TryRemoveFloor());

            if (result.Outcome == MapCellOutcome.Removed)
            {
                CleanupEmptyCell(coord);
            }

            return result;
        }

        #endregion

        #region Wall Methods

        /// <summary>
        /// Determines whether the specified cell contains a wall.
        /// </summary>
        /// <param name="coord">The coordinates of the cell to check for a wall.</param>
        /// <returns><see langword="true"/> if the cell at the specified coordinates contains a wall; otherwise, <see
        /// langword="false"/>.</returns>
        internal bool HasWall(MapCellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.HasWall;
        }

        /// <summary>
        /// Attempts to place a wall at the specified map cell coordinate.
        /// </summary>
        /// <param name="coord">The coordinate of the map cell where the wall should be placed.</param>
        /// <returns>A MapOperationResult indicating the outcome of the wall placement attempt.</returns>
        internal MapOperationResult TryPlaceWall(MapCellCoord coord)
        {
            return WithExistingCell(coord, cell => cell.TryPlaceWall());
        }

        /// <summary>
        /// Attempts to remove a wall from the cell at the specified coordinates.
        /// </summary>
        /// <param name="coord">The coordinates of the cell from which to attempt to remove the wall.</param>
        /// <returns>A MapOperationResult indicating whether the wall was successfully removed and providing additional
        /// information about the operation.</returns>
        internal MapOperationResult TryRemoveWall(MapCellCoord coord)
        {
            return WithExistingCell(coord, cell => cell.TryRemoveWall());
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines whether a cell exists at the specified coordinate.
        /// </summary>
        /// <param name="coord">The coordinate to check.</param>
        /// <returns>
        /// <c>true</c> if a cell exists at the coordinate; otherwise, <c>false</c>.
        /// </returns>
        internal bool CellExists(MapCellCoord coord)
        {
            return TryGetCell(coord, out var _);
        }

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

        /// <summary>
        /// Executes an operation against an existing cell, returning a failure
        /// result if the cell does not exist.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <param name="operation">The operation to execute on the cell.</param>
        /// <returns>
        /// A map operation result describing the outcome.
        /// </returns>
        private MapOperationResult WithExistingCell(MapCellCoord coord, Func<MapCell, MapCellResult> operation)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return MapOperationResult.Skipped(coord, MapCellFailureReason.NoCell);
            }

            var cellResult = operation(cell);
            return MapOperationResult.FromMapCellResult(coord, cellResult);
        }

        /// <summary>
        /// Removes the specified cell from the grid if it is empty.
        /// This is used to keep the grid sparse after destructive operations.
        /// </summary>
        /// <param name="coord">The coordinate of the cell to clean up.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown in debug builds if the cell could not be removed when expected.
        /// </exception>
        private void CleanupEmptyCell(MapCellCoord coord)
        {
            if (!_grid.TryRemoveCell(coord))
            {
#if DEBUG
                throw new InvalidOperationException(
                    $"{nameof(CleanupEmptyCell)}: Cell cleanup failed after floor removal at {coord}. Cell was expected to be empty.");
#else
                    ConsoleLog.Error(
                        nameof(MapManager),
                        $"{nameof(CleanupEmptyCell)}: Cell cleanup failed after floor removal at {coord}. Cell was expected to be empty."
                    );
#endif
            }
        }

        #endregion
    }
}