using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Systems;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Coordinates map-level systems including terrain generation and cell-based
    /// build operations, acting as the authoritative entry point for map queries
    /// and mutations.
    /// </summary>
    public sealed class MapManager
    {
        #region Fields

        private readonly CellSystem _cellSystem;
        private readonly TerrainSystem _terrainSystem;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new map manager with internal terrain and cell systems.
        /// </summary>
        internal MapManager()
        {
            _terrainSystem = new TerrainSystem();
            _cellSystem = new CellSystem();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total number of cells currently managed by the map.
        /// </summary>
        internal int CellCount => _cellSystem.CellCount;

        #endregion

        #region Public API

        /// <summary>
        /// Attempts to retrieve the terrain tile at the specified map coordinate.
        /// </summary>
        /// <param name="coord">The map coordinate to query.</param>
        /// <param name="terrainTile">The terrain tile at the coordinate if found.</param>
        /// <returns><c>true</c> if terrain exists at the coordinate; otherwise <c>false</c>.</returns>
        public bool TryGetTerrain(MapCoord coord, [NotNullWhen(true)] out TerrainTile terrainTile)
        {
            if (TryGetCell(coord, out var cell))
            {
                terrainTile = cell.TerrainTile;
                return true;
            }

            terrainTile = default!;
            return false;
        }

        #endregion

        #region Generation

        /// <summary>
        /// Generates terrain and initializes map cells using the specified seed and size.
        /// </summary>
        /// <param name="seed">The seed used to deterministically generate terrain.</param>
        /// <param name="mapSize">The number of cells per axis to generate.</param>
        internal void GenerateMap(int seed, int mapSize)
        {
            _terrainSystem.GenerateTerrain(seed, mapSize, _cellSystem);

            var allCoords = _cellSystem.EnumerateAllCoords();
            _terrainSystem.ResolveSlopeNeighbors(allCoords, coord => TryGetTerrain(coord, out var t) ? t : null);
        }

        #endregion

        #region Cell Operations

        /// <summary>
        /// Attempts to retrieve the cell at the specified map coordinate.
        /// </summary>
        /// <param name="coord">The map coordinate to query.</param>
        /// <param name="cell">The cell at the coordinate if found.</param>
        /// <returns><c>true</c> if the cell exists; otherwise <c>false</c>.</returns>
        internal bool TryGetCell(MapCoord coord, [NotNullWhen(true)] out Cell? cell)
        {
            return _cellSystem.TryGetCell(coord, out cell);
        }

        /// <summary>
        /// Determines whether the specified build kind exists at the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to check.</param>
        /// <param name="coord">The map coordinate to query.</param>
        /// <returns><c>true</c> if the build element exists; otherwise <c>false</c>.</returns>
        internal bool Has(BuildKind buildKind, MapCoord coord)
        {
            return _cellSystem.Has(buildKind, coord);
        }

        /// <summary>
        /// Validates whether the specified build kind can be placed at the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <returns>The result of the placement validation.</returns>
        internal BuildOperationResult CanPlace(BuildKind buildKind, MapCoord coord)
        {
            return _cellSystem.CanPlace(buildKind, coord);
        }

        /// <summary>
        /// Validates whether the specified build kind can be removed from the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <returns>The result of the removal validation.</returns>
        internal BuildOperationResult CanRemove(BuildKind buildKind, MapCoord coord)
        {
            return _cellSystem.CanRemove(buildKind, coord);
        }

        /// <summary>
        /// Attempts to place the specified build kind at the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate at which to place.</param>
        /// <returns>The result of the placement operation.</returns>
        internal BuildOperationResult TryPlace(BuildKind buildKind, MapCoord coord)
        {
            return _cellSystem.TryPlace(buildKind, coord);
        }

        /// <summary>
        /// Attempts to remove the specified build kind from the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate from which to remove.</param>
        /// <returns>The result of the removal operation.</returns>
        internal BuildOperationResult TryRemove(BuildKind buildKind, MapCoord coord)
        {
            return _cellSystem.TryRemove(buildKind, coord);
        }

        #endregion
    }
}