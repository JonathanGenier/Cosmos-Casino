using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Furniture;
using CosmosCasino.Core.Map.Cell;
using CosmosCasino.Core.Map.Grid;
using CosmosCasino.Core.Structure;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// High-level façade responsible for managing map cells and delegating
    /// floor, structure, and furniture operations to the underlying grid.
    /// </summary>
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
        /// Determines whether the specified cell currently has a floor.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and has a floor;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool HasFloor(MapCellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.HasFloor;
        }

        /// <summary>
        /// Gets the floor type assigned to the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// The floor type if the cell exists; otherwise, <c>null</c>.
        /// </returns>
        internal FloorType? GetFloorType(MapCellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return null;
            }

            return cell.Floor;
        }

        /// <summary>
        /// Attempts to place a floor of the specified type at the given cell.
        /// Creates the cell if it does not already exist.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <param name="floor">The floor type to place.</param>
        /// <returns>
        /// A map operation result describing the outcome of the placement.
        /// </returns>
        internal MapOperationResult TryPlaceFloor(MapCellCoord coord, FloorType floor)
        {
            var cell = _grid.GetOrCreateCell(coord);
            var cellResult = cell.TryPlaceFloor(floor);
            return MapOperationResult.FromMapCellResult(coord, cellResult);
        }

        /// <summary>
        /// Attempts to replace the existing floor at the specified cell
        /// with a new floor type.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <param name="floor">The new floor type to apply.</param>
        /// <returns>
        /// A map operation result describing the outcome of the replacement.
        /// </returns>
        internal MapOperationResult TryReplaceFloor(MapCellCoord coord, FloorType floor)
        {
            return WithExistingCell(coord, cell => cell.TryReplaceFloor(floor));
        }

        /// <summary>
        /// Attempts to remove the floor from the specified cell.
        /// Automatically removes the cell from the grid if it becomes empty.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <returns>
        /// A map operation result describing the outcome of the removal.
        /// </returns>
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

        #region Structure Methods

        /// <summary>
        /// Determines whether the specified cell currently has a structure.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and has a structure;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool HasStructure(MapCellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.HasStructure;
        }

        /// <summary>
        /// Gets the structure type assigned to the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// The structure type if the cell exists; otherwise, <c>null</c>.
        /// </returns>
        internal StructureType? GetStructureType(MapCellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return null;
            }

            return cell.Structure;
        }

        /// <summary>
        /// Attempts to place a structure of the specified type in the given cell.
        /// The cell must already exist and contain a floor.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <param name="structure">The structure type to place.</param>
        /// <returns>
        /// A map operation result describing the outcome of the placement.
        /// </returns>
        internal MapOperationResult TryPlaceStructure(MapCellCoord coord, StructureType structure)
        {
            return WithExistingCell(coord, cell => cell.TryPlaceStructure(structure));
        }

        /// <summary>
        /// Attempts to replace the existing structure in the given cell
        /// with a new structure type.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <param name="structure">The new structure type to apply.</param>
        /// <returns>
        /// A map operation result describing the outcome of the replacement.
        /// </returns>
        internal MapOperationResult TryReplaceStructure(MapCellCoord coord, StructureType structure)
        {
            return WithExistingCell(coord, cell => cell.TryReplaceStructure(structure));
        }

        /// <summary>
        /// Attempts to remove the structure from the specified cell.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <returns>
        /// A map operation result describing the outcome of the removal.
        /// </returns>
        internal MapOperationResult TryRemoveStructure(MapCellCoord coord)
        {
            return WithExistingCell(coord, cell => cell.TryRemoveStructure());
        }

        #endregion

        #region Furniture Methods

        /// <summary>
        /// Determines whether the specified cell currently has furniture.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and has furniture;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool HasFurniture(MapCellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.HasFurniture;
        }

        /// <summary>
        /// Gets the furniture type assigned to the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// The furniture type if the cell exists; otherwise, <c>null</c>.
        /// </returns>
        internal FurnitureType? GetFurnitureType(MapCellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return null;
            }

            return cell.Furniture;
        }

        /// <summary>
        /// Attempts to place furniture of the specified type in the given cell.
        /// The cell must already exist, contain a floor, and be otherwise unoccupied.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <param name="furniture">The furniture type to place.</param>
        /// <returns>
        /// A map operation result describing the outcome of the placement.
        /// </returns>
        internal MapOperationResult TryPlaceFurniture(MapCellCoord coord, FurnitureType furniture)
        {
            return WithExistingCell(coord, cell => cell.TryPlaceFurniture(furniture));
        }

        /// <summary>
        /// Attempts to remove the furniture from the specified cell.
        /// </summary>
        /// <param name="coord">The coordinate of the target cell.</param>
        /// <returns>
        /// A map operation result describing the outcome of the removal.
        /// </returns>
        internal MapOperationResult TryRemoveFurniture(MapCellCoord coord)
        {
            return WithExistingCell(coord, cell => cell.TryRemoveFurniture());
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
                return MapOperationResult.Failed(coord, MapCellFailureReason.NoCell);
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