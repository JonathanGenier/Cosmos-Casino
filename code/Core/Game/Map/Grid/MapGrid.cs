using CosmosCasino.Core.Game.Map.Cell;

namespace CosmosCasino.Core.Game.Map.Grid
{
    /// <summary>
    /// Manages a sparse collection of map cells indexed by 3D coordinates.
    /// Cells are created on demand and removed automatically when empty.
    /// </summary>
    public sealed partial class MapGrid
    {
        #region FIELDS

        private readonly Dictionary<MapCellCoord, MapCell> _cells = new();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the number of cells currently stored in the grid.
        /// </summary>
        internal int CellCount => _cells.Count;

        #endregion

        #region METHODS

        /// <summary>
        /// Retrieves an existing cell at the given coordinate if it exists.
        /// </summary>
        /// <param name="coord">The coordinate of the cell to retrieve.</param>
        /// <returns>
        /// The existing <see cref="MapCell"/> if found; otherwise <c>null</c>.
        /// </returns>
        internal MapCell? GetCell(MapCellCoord coord)
        {
            _cells.TryGetValue(coord, out var cell);
            return cell;
        }

        /// <summary>
        /// Retrieves the cell at the given coordinate, creating it if it does not exist.
        /// </summary>
        /// <param name="coord">The coordinate of the cell to retrieve or create.</param>
        /// <returns>
        /// A non-null <see cref="MapCell"/> associated with the given coordinate.
        /// </returns>
        internal MapCell GetOrCreateCell(MapCellCoord coord)
        {
            if (!_cells.TryGetValue(coord, out var cell))
            {
                cell = new MapCell();
                _cells.Add(coord, cell);
            }

            return cell;
        }

        /// <summary>
        /// Removes the cell at the specified coordinate from the map. Throws an exception if the cell does not exist or
        /// is not empty.
        /// </summary>
        /// <param name="coord">The coordinate of the cell to remove.</param>
        /// <exception cref="InvalidOperationException">Thrown if the cell at the specified coordinate does not exist or is not empty.</exception>
        internal void RemoveCell(MapCellCoord coord)
        {
            if (!_cells.TryGetValue(coord, out var cell))
            {
                throw new InvalidOperationException($"Cell cleanup failed after floor removal at {coord}. Cell was expected to exists.");
            }

            if (!cell.IsEmpty)
            {
                throw new InvalidOperationException($"Cell cleanup failed after floor removal at {coord}. Cell was expected to be empty.");
            }

            _cells.Remove(coord);
        }

        #endregion
    }
}