using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Systems;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Coordinates map-level systems including terrain generation and cell-based
    /// build operations, acting as the authoritative entry point for map queries
    /// and mutations. Map chunks are resolved by this manager as global X/Z
    /// spatial ownership regions and own terrain storage for their local region.
    /// </summary>
    public sealed partial class MapManager
    {
        #region Fields

        private readonly Dictionary<MapChunkCoord, MapChunk> _chunks = new();
        private readonly CellSystem _cellSystem;
        private readonly TerrainTileSink _terrainSink;
        private readonly TerrainSystem _terrainSystem;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new map manager with internal terrain and cell systems.
        /// </summary>
        internal MapManager()
        {
            _terrainSystem = new TerrainSystem();
            _terrainSink = new TerrainTileSink(this);
            _cellSystem = new CellSystem();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total number of cells currently managed by the map.
        /// </summary>
        internal int CellCount => _cellSystem.CellCount;

        /// <summary>
        /// Gets the number of map chunk identities currently resolved by the map manager.
        /// </summary>
        internal int ChunkCount => _chunks.Count;

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
            return TryGetTerrain(new TerrainTileWorldCoord(coord.X, coord.Y), out terrainTile);
        }

        /// <summary>
        /// Attempts to retrieve the terrain tile at the specified terrain world-tile coordinate.
        /// </summary>
        /// <param name="coord">The terrain world-tile coordinate to query.</param>
        /// <param name="terrainTile">The terrain tile at the coordinate if found.</param>
        /// <returns><c>true</c> if terrain exists at the coordinate; otherwise <c>false</c>.</returns>
        public bool TryGetTerrain(TerrainTileWorldCoord coord, [NotNullWhen(true)] out TerrainTile terrainTile)
        {
            MapChunkCoord chunkCoord = ResolveTerrainChunkCoord(coord);

            if (!TryGetChunk(chunkCoord, out var chunk))
            {
                terrainTile = default!;
                return false;
            }

            MapChunkLocalCoord localCoord = ResolveTerrainChunkLocalCoord(coord);
            return chunk.TryGetTerrain(localCoord, out terrainTile);
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
            _terrainSystem.GenerateTerrain(seed, mapSize, _terrainSink);
            _terrainSystem.ResolveSlopeNeighbors(EnumerateTerrainCoords(), coord => TryGetTerrain(coord, out var t) ? t : null);
        }

        #endregion

        #region Terrain Operations

        /// <summary>
        /// Stores generated terrain at the specified global terrain world-tile coordinate.
        /// </summary>
        /// <param name="coord">The global terrain tile coordinate to store.</param>
        /// <param name="terrainTile">The generated terrain tile to store.</param>
        internal void StoreGeneratedTerrain(TerrainTileWorldCoord coord, TerrainTile terrainTile)
        {
            ArgumentNullException.ThrowIfNull(terrainTile);

            MapChunkCoord chunkCoord = ResolveTerrainChunkCoord(coord);
            MapChunkLocalCoord localCoord = ResolveTerrainChunkLocalCoord(coord);
            MapChunk chunk = GetOrCreateChunk(chunkCoord);

            chunk.StoreGeneratedTerrain(localCoord, terrainTile);
            _cellSystem.CreateCell(ToMapCoord(coord));
        }

        /// <summary>
        /// Replaces existing terrain at the specified global terrain world-tile coordinate.
        /// </summary>
        /// <param name="coord">The global terrain tile coordinate to mutate.</param>
        /// <param name="terrainTile">The replacement terrain tile.</param>
        /// <returns>
        /// <c>true</c> when authoritative terrain existed and was replaced; otherwise, <c>false</c>.
        /// </returns>
        internal bool TryReplaceTerrain(TerrainTileWorldCoord coord, TerrainTile terrainTile)
        {
            ArgumentNullException.ThrowIfNull(terrainTile);

            MapChunkCoord chunkCoord = ResolveTerrainChunkCoord(coord);

            if (!TryGetChunk(chunkCoord, out var chunk))
            {
                return false;
            }

            MapChunkLocalCoord localCoord = ResolveTerrainChunkLocalCoord(coord);
            return chunk.TryReplaceTerrain(localCoord, terrainTile);
        }

        #endregion

        #region Chunk Operations

        /// <summary>
        /// Resolves the chunk coordinate that owns the specified global logical cell coordinate.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to resolve.</param>
        /// <returns>The owning map chunk coordinate.</returns>
        internal MapChunkCoord ResolveChunkCoord(MapCellCoord coord)
        {
            return MapMath.CellToChunk(coord);
        }

        /// <summary>
        /// Resolves the chunk-local coordinate for the specified global logical cell coordinate.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to resolve.</param>
        /// <returns>The chunk-local X/Z coordinate inside the owning map chunk.</returns>
        internal MapChunkLocalCoord ResolveChunkLocalCoord(MapCellCoord coord)
        {
            return MapMath.CellToChunkLocal(coord);
        }

        /// <summary>
        /// Retrieves an existing map chunk or creates the identity for the chunk owning the specified global cell.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to resolve.</param>
        /// <returns>The owning map chunk identity.</returns>
        internal MapChunk GetOrCreateChunk(MapCellCoord coord)
        {
            return GetOrCreateChunk(ResolveChunkCoord(coord));
        }

        /// <summary>
        /// Retrieves an existing map chunk or creates the identity for the specified map chunk coordinate.
        /// </summary>
        /// <param name="coord">The global X/Z map chunk coordinate to resolve.</param>
        /// <returns>The resolved map chunk identity.</returns>
        internal MapChunk GetOrCreateChunk(MapChunkCoord coord)
        {
            if (!_chunks.TryGetValue(coord, out var chunk))
            {
                chunk = new MapChunk(coord);
                _chunks.Add(coord, chunk);
            }

            return chunk;
        }

        /// <summary>
        /// Attempts to retrieve an already resolved map chunk identity.
        /// </summary>
        /// <param name="coord">The global X/Z map chunk coordinate to query.</param>
        /// <param name="chunk">The resolved map chunk, if present.</param>
        /// <returns><c>true</c> if the chunk has been resolved; otherwise, <c>false</c>.</returns>
        internal bool TryGetChunk(MapChunkCoord coord, [NotNullWhen(true)] out MapChunk? chunk)
        {
            return _chunks.TryGetValue(coord, out chunk);
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
        /// Determines whether the specified build kind exists at the terrain base elevation of the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to check.</param>
        /// <param name="coord">The map coordinate to query.</param>
        /// <returns><c>true</c> if the build element exists; otherwise <c>false</c>.</returns>
        internal bool Has(BuildKind buildKind, MapCoord coord)
        {
            if (buildKind is not BuildKind.Floor and not BuildKind.Wall)
            {
                throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported.");
            }

            return TryGetTerrainBaseElevation(coord, out var elevation)
                && _cellSystem.Has(buildKind, coord, elevation);
        }

        /// <summary>
        /// Determines whether the specified build kind exists at the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to check.</param>
        /// <param name="coord">The map coordinate to query.</param>
        /// <param name="elevation">The elevation to query.</param>
        /// <returns><c>true</c> if the build element exists; otherwise <c>false</c>.</returns>
        internal bool Has(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            return _cellSystem.Has(buildKind, coord, elevation);
        }

        /// <summary>
        /// Validates whether the specified build kind can be placed at the terrain base elevation of the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <returns>The result of the placement validation.</returns>
        internal BuildOperationResult CanPlace(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetTerrainBaseElevation(coord, out var elevation))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return _cellSystem.CanPlace(buildKind, coord, elevation);
        }

        /// <summary>
        /// Validates whether the specified build kind can be placed at the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <param name="elevation">The elevation to validate.</param>
        /// <returns>The result of the placement validation.</returns>
        internal BuildOperationResult CanPlace(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            return _cellSystem.CanPlace(buildKind, coord, elevation);
        }

        /// <summary>
        /// Validates whether the specified build kind can be removed from the terrain base elevation of the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <returns>The result of the removal validation.</returns>
        internal BuildOperationResult CanRemove(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetTerrainBaseElevation(coord, out var elevation))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return _cellSystem.CanRemove(buildKind, coord, elevation);
        }

        /// <summary>
        /// Validates whether the specified build kind can be removed from the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <param name="elevation">The elevation to validate.</param>
        /// <returns>The result of the removal validation.</returns>
        internal BuildOperationResult CanRemove(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            return _cellSystem.CanRemove(buildKind, coord, elevation);
        }

        /// <summary>
        /// Attempts to place the specified build kind at the terrain base elevation of the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate at which to place.</param>
        /// <returns>The result of the placement operation.</returns>
        internal BuildOperationResult TryPlace(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetTerrainBaseElevation(coord, out var elevation))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return _cellSystem.TryPlace(buildKind, coord, elevation);
        }

        /// <summary>
        /// Attempts to place the specified build kind at the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate at which to place.</param>
        /// <param name="elevation">The elevation at which to place.</param>
        /// <returns>The result of the placement operation.</returns>
        internal BuildOperationResult TryPlace(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            return _cellSystem.TryPlace(buildKind, coord, elevation);
        }

        /// <summary>
        /// Attempts to remove the specified build kind from the terrain base elevation of the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate from which to remove.</param>
        /// <returns>The result of the removal operation.</returns>
        internal BuildOperationResult TryRemove(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetTerrainBaseElevation(coord, out var elevation))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return _cellSystem.TryRemove(buildKind, coord, elevation);
        }

        /// <summary>
        /// Attempts to remove the specified build kind from the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate from which to remove.</param>
        /// <param name="elevation">The elevation from which to remove.</param>
        /// <returns>The result of the removal operation.</returns>
        internal BuildOperationResult TryRemove(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            return _cellSystem.TryRemove(buildKind, coord, elevation);
        }

        #endregion

        #region Helpers

        private static MapCoord ToMapCoord(TerrainTileWorldCoord coord)
        {
            return new MapCoord(coord.X, coord.Y);
        }

        private static MapChunkCoord ResolveTerrainChunkCoord(TerrainTileWorldCoord coord)
        {
            return MapMath.GlobalToChunk(coord.X, coord.Y);
        }

        private static MapChunkLocalCoord ResolveTerrainChunkLocalCoord(TerrainTileWorldCoord coord)
        {
            return MapMath.GlobalToChunkLocal(coord.X, coord.Y);
        }

        private IEnumerable<TerrainTileWorldCoord> EnumerateTerrainCoords()
        {
            foreach (var chunk in _chunks.Values)
            {
                foreach (var local in chunk.EnumerateTerrainLocals())
                {
                    MapCellCoord cellCoord = MapMath.ChunkLocalToCell(chunk.Coord, local, y: 0);
                    yield return new TerrainTileWorldCoord(cellCoord.X, cellCoord.Z);
                }
            }
        }

        private void ReceiveGeneratedTerrainTile(TerrainTileWorldCoord coord, TerrainTile terrainTile)
        {
            StoreGeneratedTerrain(coord, terrainTile);
        }

        #endregion

        #region Terrain Sink

        private sealed class TerrainTileSink : ITerrainTileSink
        {
            private readonly MapManager _mapManager;

            internal TerrainTileSink(MapManager mapManager)
            {
                _mapManager = mapManager;
            }

            public void ReceiveTerrainTile(TerrainTileWorldCoord coord, TerrainTile terrainTile)
            {
                _mapManager.ReceiveGeneratedTerrainTile(coord, terrainTile);
            }
        }

        #endregion
    }
}
