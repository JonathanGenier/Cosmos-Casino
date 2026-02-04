using CosmosCasino.Core.Game.Map.Terrain.Tile;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Manages a sparse collection of map cells indexed by 3D coordinates.
    /// Cells are created on demand and removed automatically when empty.
    /// </summary>
    public sealed class Grid
    {
        #region Fields

        private readonly Dictionary<MapCoord, Cell> _cells = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of cells currently stored in the grid.
        /// </summary>
        internal int CellCount => _cells.Count;

        /// <summary>
        /// Gets an enumerable view of all map coordinates currently stored in the grid.
        /// </summary>
        internal IEnumerable<MapCoord> AllCoords => _cells.Keys;

        #endregion

        #region Cell Management

        /// <summary>
        /// Creates a new cell at the specified coordinate if one does not already exist.
        /// </summary>
        /// <param name="coord">The map coordinate at which to create the cell.</param>
        /// <param name="terrainTile">The terrain tile associated with the new cell.</param>
        internal void CreateCell(MapCoord coord, TerrainTile terrainTile)
        {
            if (!_cells.ContainsKey(coord))
            {
                var cell = new Cell(coord, terrainTile);
                _cells.Add(coord, cell);
            }
        }

        #endregion

        #region Cells

        /// <summary>
        /// Retrieves an existing cell at the given coordinate if it exists.
        /// </summary>
        /// <param name="coord">The coordinate of the cell to retrieve.</param>
        /// <returns>
        /// The existing <see cref="Cell.Cell"/> if found; otherwise <c>null</c>.
        /// </returns>
        internal Cell? GetCell(MapCoord coord)
        {
            _cells.TryGetValue(coord, out var cell);
            return cell;
        }

        #endregion
    }
}