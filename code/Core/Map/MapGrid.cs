namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Manages a sparse collection of map cells indexed by 3D coordinates.
    /// Cells are created on demand and removed automatically when empty.
    /// </summary>
    internal sealed class MapGrid
    {
        #region FIELDS

        private readonly Dictionary<CellCoord, MapCell> _cells = new();

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
        internal MapCell? GetCell(CellCoord coord)
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
        internal MapCell GetOrCreateCell(CellCoord coord)
        {
            if (!_cells.TryGetValue(coord, out var cell))
            {
                cell = new MapCell();
                _cells.Add(coord, cell);
            }

            return cell;
        }

        /// <summary>
        /// Attempts to remove the cell at the given coordinate.
        /// A cell can only be removed if it exists and is completely empty.
        /// </summary>
        /// <param name="coord">The coordinate of the cell to remove.</param>
        /// <returns>
        /// <c>true</c> if the cell was removed; otherwise <c>false</c>.
        /// </returns>
        internal bool TryRemoveCell(CellCoord coord)
        {
            if (!_cells.TryGetValue(coord, out var cell))
            {
                return false;
            }

            if (!cell.IsEmpty)
            {
                return false;
            }

            _cells.Remove(coord);
            return true;
        }

        #endregion
    }
}
