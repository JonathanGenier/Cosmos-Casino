using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// High-level façade responsible for managing map cells and delegating
    /// floor, structure, and furniture operations to the underlying grid.
    /// </summary>
    internal sealed class MapManager
    {
        #region FIELDS

        private readonly MapGrid _grid = new();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the total number of cells currently stored in the grid.
        /// </summary>
        internal int CellCount => _grid.CellCount;

        #endregion

        #region METHODS

        // ===================================================================================
        // FLOOR

        /// <summary>
        /// Determines whether the floor can be removed from the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and its floor can be removed;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool CanRemoveFloor(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.CanRemoveFloor();
        }

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
        internal bool HasFloor(CellCoord coord)
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
        internal FloorType? GetFloorType(CellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return null;
            }

            return cell.Floor;
        }

        /// <summary>
        /// Attempts to set or replace the floor for the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <param name="floor">
        /// The floor type to assign.
        /// </param>
        /// <returns>
        /// <c>true</c> if the floor was set or replaced;
        /// <c>false</c> if the floor type was already present.
        /// </returns>
        internal bool TrySetFloor(CellCoord coord, FloorType floor)
        {
            var cell = _grid.GetOrCreateCell(coord);
            return cell.TrySetFloor(floor);
        }

        /// <summary>
        /// Attempts to remove the floor from the specified cell and
        /// remove the cell from the grid if it becomes empty.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the floor and cell were removed;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool TryRemoveFloor(CellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return false;
            }

            if (!cell.TryRemoveFloor())
            {
                return false;
            }

            return _grid.TryRemoveCell(coord);
        }

        // ===================================================================================
        // STRUCTURE

        /// <summary>
        /// Determines whether a structure can be placed in the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and can accept a structure;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool CanPlaceStructure(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.CanPlaceStructure();
        }

        /// <summary>
        /// Determines whether the structure in the specified cell can be replaced.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and its structure can be replaced;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool CanReplaceStructure(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.CanReplaceStructure();
        }

        /// <summary>
        /// Determines whether the structure in the specified cell can be removed.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and has a removable structure;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool CanRemoveStructure(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.CanRemoveStructure();
        }

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
        internal bool HasStructure(CellCoord coord)
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
        internal StructureType? GetStructureType(CellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return null;
            }

            return cell.Structure;
        }

        /// <summary>
        /// Attempts to place a structure in the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <param name="structure">
        /// The structure type to place.
        /// </param>
        /// <returns>
        /// <c>true</c> if the structure was placed;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool TryPlaceStructure(CellCoord coord, StructureType structure)
        {
            return TryGetCell(coord, out var cell) && cell.TryPlaceStructure(structure);
        }

        /// <summary>
        /// Attempts to replace the structure in the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <param name="structure">
        /// The new structure type.
        /// </param>
        /// <returns>
        /// <c>true</c> if the structure was replaced;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool TryReplaceStructure(CellCoord coord, StructureType structure)
        {
            return TryGetCell(coord, out var cell) && cell.TryReplaceStructure(structure);
        }

        /// <summary>
        /// Attempts to remove the structure from the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the structure was removed;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool TryRemoveStructure(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.TryRemoveStructure();
        }

        // ===================================================================================
        // FURNITURE

        /// <summary>
        /// Determines whether furniture can be placed in the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and can accept furniture;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool CanPlaceFurniture(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.CanPlaceFurniture();
        }

        /// <summary>
        /// Determines whether furniture can be removed from the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cell exists and has removable furniture;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool CanRemoveFurniture(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.CanRemoveFurniture();
        }

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
        internal bool HasFurniture(CellCoord coord)
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
        internal FurnitureType? GetFurnitureType(CellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return null;
            }

            return cell.Furniture;
        }

        /// <summary>
        /// Attempts to place furniture in the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <param name="furniture">
        /// The furniture type to place.
        /// </param>
        /// <returns>
        /// <c>true</c> if the furniture was placed;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool TryPlaceFurniture(CellCoord coord, FurnitureType furniture)
        {
            return TryGetCell(coord, out var cell) && cell.TryPlaceFurniture(furniture);
        }

        /// <summary>
        /// Attempts to remove furniture from the specified cell.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the furniture was removed;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool TryRemoveFurniture(CellCoord coord)
        {
            return TryGetCell(coord, out var cell) && cell.TryRemoveFurniture();
        }

        // ===================================================================================
        // HELPERS

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
        private bool TryGetCell(CellCoord coord, [NotNullWhen(true)] out MapCell? cell)
        {
            cell = _grid.GetCell(coord);
            return cell != null;
        }

        #endregion
    }
}
