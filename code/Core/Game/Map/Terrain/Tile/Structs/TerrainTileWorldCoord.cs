using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;

namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Represents a terrain tile coordinate in world-space.
    /// This type provides an immutable, integer-based mapping from
    /// chunk-local or explicit world positions into a unified
    /// world tile coordinate system.
    /// </summary>
    public readonly record struct TerrainTileWorldCoord
    {
        /// <summary>
        /// Initializes a new world-space tile coordinate using explicit
        /// world tile values.
        /// </summary>
        /// <param name="x">
        /// The world-space X tile coordinate.
        /// </param>
        /// <param name="y">
        /// The world-space Y tile coordinate.
        /// </param>
        internal TerrainTileWorldCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new world-space tile coordinate from a chunk
        /// coordinate and a chunk-local tile coordinate.
        /// </summary>
        /// <param name="gridCoord">
        /// The chunk coordinate containing the tile.
        /// </param>
        /// <param name="tileLocalCoord">
        /// The tile's local coordinate within the chunk.
        /// </param>
        /// <remarks>
        /// The resulting world coordinate is computed by offsetting
        /// the chunk origin by the chunk size and adding the local
        /// tile position.
        /// </remarks>
        internal TerrainTileWorldCoord(TerrainChunkGridCoord gridCoord, TerrainTileLocalCoord tileLocalCoord)
        {
            X = (gridCoord.X * TerrainConfigs.ChunkSize) + (int)tileLocalCoord.X;
            Y = (gridCoord.Y * TerrainConfigs.ChunkSize) + (int)tileLocalCoord.Y;
        }

        #region Properties

        /// <summary>
        /// Gets the world-space X tile coordinate.
        /// </summary>
        internal int X { get; }

        /// <summary>
        /// Gets the world-space Y tile coordinate.
        /// </summary>
        internal int Y { get; }

        #endregion
    }
}