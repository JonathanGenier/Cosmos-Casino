using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Math;
using CosmosCasino.Core.Game.Map.Terrain.Tile;

namespace CosmosCasino.Core.Game.Map.Terrain.Chunk
{
    /// <summary>
    /// Internal representation of a terrain chunk.
    /// Represents a fixed-size chunk of terrain tiles.
    /// A terrain chunk owns a contiguous grid of tiles with strict structural
    /// invariants regarding size, ordering, and local coordinates. These
    /// invariants are validated at construction time to ensure deterministic
    /// behavior and safe access throughout the terrain system.
    /// </summary>
    public sealed partial class TerrainChunk
    {
        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="TerrainChunk"/> with the specified chunk
        /// coordinates and tile collection.
        /// </summary>
        /// <param name="gridCoord">
        /// The logical chunk coordinate identifying this chunk within the terrain.
        /// </param>
        /// <param name="tiles">
        /// A read-only collection of terrain tiles representing a complete
        /// <see cref="TerrainConfigs.ChunkSize"/> × <see cref="TerrainConfigs.ChunkSize"/>
        /// grid. Tiles must be ordered in row-major order and have matching local
        /// coordinates.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="tiles"/> is <c>null</c>.
        /// </exception>
        internal TerrainChunk(TerrainChunkGridCoord gridCoord, IReadOnlyList<TerrainTile> tiles)
        {
            ArgumentNullException.ThrowIfNull(tiles);

            ValidateTiles(tiles);

            GridCoord = gridCoord;
            WorldOrigin = TerrainMath.ChunkGridCoordToWorldOrigin(gridCoord);
            Tiles = tiles;
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Attempts to retrieve a terrain tile at the specified chunk-local
        /// coordinate.
        /// </summary>
        /// <param name="localCoord">
        /// The tile's local coordinate within this chunk.
        /// </param>
        /// <param name="tile">
        /// When this method returns <c>true</c>, contains the terrain tile
        /// located at <paramref name="localCoord"/>.  
        /// When this method returns <c>false</c>, contains <c>default</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a tile exists at the specified local coordinate;
        /// otherwise, <c>false</c>.
        /// </returns>
        internal bool TryGetTile(TerrainTileLocalCoord localCoord, out TerrainTile tile)
        {
            int index = localCoord.Index;

            if ((uint)index >= (uint)Tiles.Count)
            {
                tile = default!;
                return false;
            }

            tile = Tiles[index];
            return true;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates the structural integrity of the provided tile collection.
        /// Ensures the correct tile count, ordering, and local coordinate mapping
        /// for a terrain chunk.
        /// </summary>
        /// <param name="tiles">
        /// The tile collection to validate.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the tile count is incorrect or if any tile has a local
        /// coordinate that does not match its expected position.
        /// </exception>
        private static void ValidateTiles(IReadOnlyList<TerrainTile> tiles)
        {
            if (tiles.Count != TerrainConfigs.ChunkSize * TerrainConfigs.ChunkSize)
            {
                throw new ArgumentException($"Expected {TerrainConfigs.ChunkSize * TerrainConfigs.ChunkSize} tiles, got {tiles.Count}", nameof(tiles));
            }

            for (int y = 0; y < TerrainConfigs.ChunkSize; y++)
            {
                for (int x = 0; x < TerrainConfigs.ChunkSize; x++)
                {
                    var currentLocalCoord = new TerrainTileLocalCoord(x, y);
                    var index = currentLocalCoord.Index;

                    if (tiles[index].LocalCoord != currentLocalCoord)
                    {
                        throw new ArgumentException($"Tile at index {index} has invalid LocalCoord", nameof(tiles));
                    }
                }
            }
        }

        #endregion
    }
}