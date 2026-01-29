using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using CosmosCasino.Core.Game.Map.Terrain.Generation;
using CosmosCasino.Core.Game.Map.Terrain.Math;
using CosmosCasino.Core.Game.Map.Terrain.Tile;

namespace CosmosCasino.Core.Game.Map.Terrain
{
    /// <summary>
    /// Central manager responsible for terrain chunk generation, storage,
    /// and tile-level spatial queries within the terrain system.
    /// Handles deterministic terrain creation, chunk lookup, and
    /// slope-neighbor resolution logic.
    /// </summary>
    public sealed class TerrainManager
    {
        #region Fields

        private static readonly (SlopeNeighborMask Mask, int Dx, int Dy)[] NeighborOffsets =
        {
            (SlopeNeighborMask.North, 0, -1),
            (SlopeNeighborMask.NorthEast, 1, -1),
            (SlopeNeighborMask.East, 1,  0),
            (SlopeNeighborMask.SouthEast, 1,  1),
            (SlopeNeighborMask.South, 0,  1),
            (SlopeNeighborMask.SouthWest, -1,  1),
            (SlopeNeighborMask.West, -1,  0),
            (SlopeNeighborMask.NorthWest, -1, -1),
        };

        private readonly Dictionary<TerrainChunkGridCoord, TerrainChunk> _chunks;
        private readonly TerrainBounds _terrainBounds;
        private bool _generated;

        #endregion

        #region Initialization

        /// <summary>
        /// Central manager responsible for terrain chunk generation, storage,
        /// and tile-level spatial queries within the terrain system.
        /// Handles deterministic terrain creation, chunk lookup, and
        /// slope-neighbor resolution logic.
        /// </summary>
        /// <param name="chunkCountPerAxis">Number of chunk per axis (X,Y) to define map size.</param>
        internal TerrainManager(int chunkCountPerAxis = TerrainConfigs.ChunkCountPerAxis)
        {
            _terrainBounds = TerrainBounds.CreateNew(chunkCountPerAxis);
            _chunks = new Dictionary<TerrainChunkGridCoord, TerrainChunk>(_terrainBounds.Width * _terrainBounds.Height);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the logical bounds of the terrain in chunk coordinates.
        /// These bounds define which chunks are generated and accessible.
        /// </summary>
        public TerrainBounds Bounds => _terrainBounds;

        #endregion

        #region Accessors

        /// <summary>
        /// Attempts to retrieve a terrain chunk by its chunk coordinate.
        /// </summary>
        /// <param name="gridCoord">
        /// The logical coordinate of the chunk to retrieve.
        /// </param>
        /// <param name="outChunk">
        /// When this method returns <c>true</c>, contains the requested terrain chunk.
        /// When this method returns <c>false</c>, contains <c>default</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the terrain has been generated and the chunk exists;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetChunk(TerrainChunkGridCoord gridCoord, out TerrainChunk outChunk)
        {
            if (!_generated)
            {
                outChunk = default!;
                return false;
            }

            if (!_chunks.TryGetValue(gridCoord, out var chunk))
            {
                outChunk = default!;
                return false;
            }

            outChunk = chunk;
            return true;
        }

        /// <summary>
        /// Attempts to retrieve the terrain chunk containing the specified world tile coordinate.
        /// </summary>
        /// <param name="worldCoord">
        /// The world-space tile coordinate used to determine the owning chunk.
        /// </param>
        /// <param name="outChunk">
        /// When this method returns <c>true</c>, contains the corresponding terrain chunk.
        /// When this method returns <c>false</c>, contains <c>default</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the terrain has been generated and the corresponding chunk exists;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetChunkFromWorldCoord(TerrainTileWorldCoord worldCoord, out TerrainChunk outChunk)
        {
            var gridCoord = TerrainMath.TileWorldCoordToChunkGridCoord(worldCoord);

            return TryGetChunk(gridCoord, out outChunk);
        }

        /// <summary>
        /// Attempts to retrieve a terrain tile at the specified world coordinate.
        /// </summary>
        /// <param name="worldCoord">
        /// The world-space tile coordinate to query.
        /// </param>
        /// <param name="outTile">
        /// When this method returns <c>true</c>, contains the requested terrain tile.
        /// When this method returns <c>false</c>, contains <c>default</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the terrain has been generated and the tile exists;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetTileFromWorldCoord(TerrainTileWorldCoord worldCoord, out TerrainTile outTile)
        {
            if (!_generated)
            {
                outTile = default!;
                return false;
            }

            if (!TryGetChunkFromWorldCoord(worldCoord, out var chunk))
            {
                outTile = default!;
                return false;
            }

            var localCoord = TerrainMath.TileWorldCoordToLocalCoord(worldCoord, chunk.GridCoord);

            if (!chunk.TryGetTile(localCoord, out var tile))
            {
                outTile = default!;
                return false;
            }

            outTile = tile;
            return true;
        }

        #endregion

        #region Generation

        /// <summary>
        /// Generates the initial terrain layout within the configured terrain bounds.
        /// Creates all terrain chunks, generates height data deterministically using the provided seed,
        /// and resolves slope neighbor relationships between tiles.
        /// This method may only be called once during the lifetime of the manager.
        /// </summary>
        /// <param name="seed">
        /// Seed value used to initialize deterministic terrain height generation.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if terrain generation has already been performed.
        /// </exception>
        internal void GenerateInitialTerrain(int seed)
        {
            if (_generated)
            {
                throw new InvalidOperationException("Terrain has already been generated.");
            }

            var terrainHeightGenerator = new TerrainHeightGenerator(seed);

            _chunks.Clear();

            for (int y = Bounds.MinY; y <= Bounds.MaxY; y++)
            {
                for (int x = Bounds.MinX; x <= Bounds.MaxX; x++)
                {
                    var gridCoord = new TerrainChunkGridCoord(x, y);
                    var chunk = GenerateChunk(gridCoord, terrainHeightGenerator);
                    _chunks.Add(gridCoord, chunk);
                }
            }

            _generated = true;

            ResolveSlopeNeighbors();
        }

        /// <summary>
        /// Generates a single terrain chunk at the specified chunk coordinate.
        /// Populates all tiles within the chunk and generates their height data.
        /// </summary>
        /// <param name="gridCoord">
        /// The logical coordinate of the chunk to generate.
        /// </param>
        /// <param name="heightGenerator">
        /// Height generator used to sample terrain heights deterministically.
        /// </param>
        /// <returns>
        /// A fully populated and validated <see cref="TerrainChunk"/> instance.
        /// </returns>
        private TerrainChunk GenerateChunk(TerrainChunkGridCoord gridCoord, TerrainHeightGenerator heightGenerator)
        {
            List<TerrainTile> tiles = new(TerrainConfigs.ChunkSize * TerrainConfigs.ChunkSize);

            for (int y = 0; y < TerrainConfigs.ChunkSize; y++)
            {
                for (int x = 0; x < TerrainConfigs.ChunkSize; x++)
                {
                    var localCoord = new TerrainTileLocalCoord(x, y);
                    var worldCoord = new TerrainTileWorldCoord(gridCoord, localCoord);
                    var tile = new TerrainTile(localCoord, worldCoord);

                    tile.GenerateHeights(heightGenerator);
                    tiles.Add(tile);
                }
            }

            return new TerrainChunk(gridCoord, tiles);
        }

        /// <summary>
        /// Resolves slope-neighbor relationships for all terrain tiles.
        /// For each flat tile, detects adjacent sloped tiles and records
        /// directional slope neighbor information.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if terrain generation has not yet been completed.
        /// </exception>
        private void ResolveSlopeNeighbors()
        {
            if (!_generated)
            {
                throw new InvalidOperationException("Terrain must be generated before resolving slope neighbors.");
            }

            foreach (var chunk in _chunks.Values)
            {
                foreach (var currentTile in chunk.Tiles)
                {
                    currentTile.ClearSlopeNeighbors();

                    // We only care about flat tiles
                    if (currentTile.IsSlope)
                    {
                        continue;
                    }

                    foreach (var (mask, dx, dy) in NeighborOffsets)
                    {
                        var neighborWorldCoord = new TerrainTileWorldCoord(currentTile.WorldCoord.X + dx, currentTile.WorldCoord.Y + dy);

                        if (!TryGetTileFromWorldCoord(neighborWorldCoord, out var neighborTile))
                        {
                            continue;
                        }

                        if (neighborTile.IsSlope)
                        {
                            currentTile.AddSlopeNeighbor(mask);
                        }
                    }
                }
            }
        }

        #endregion
    }
}