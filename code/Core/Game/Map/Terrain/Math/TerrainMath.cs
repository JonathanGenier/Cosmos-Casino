using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using CosmosCasino.Core.Game.Map.Terrain.Tile;

namespace CosmosCasino.Core.Game.Map.Terrain.Math
{
    /// <summary>
    /// Provides low-level mathematical utilities for converting between
    /// world-space, chunk-space, and local tile-space coordinates.
    /// These operations define the authoritative spatial mapping logic
    /// for the terrain system and must remain deterministic and stable.
    /// </summary>
    internal static class TerrainMath
    {
        /// <summary>
        /// Performs floor-based integer division.
        /// Unlike default integer division, this method always rounds
        /// toward negative infinity, ensuring correct behavior for
        /// negative coordinates when mapping world positions to chunks.
        /// </summary>
        /// <param name="value">
        /// The dividend value to divide.
        /// </param>
        /// <param name="divisor">
        /// The positive divisor value.
        /// </param>
        /// <returns>
        /// The largest integer less than or equal to
        /// <paramref name="value"/> divided by <paramref name="divisor"/>.
        /// </returns>
        internal static int FloorDiv(int value, int divisor)
        {
            int result = value / divisor;
            int remainder = value % divisor;

            if (remainder != 0 && ((remainder > 0) != (divisor > 0)))
            {
                result--;
            }

            return result;
        }

        /// <summary>
        /// Converts a world-space tile coordinate into its corresponding
        /// chunk coordinate.
        /// This method uses floor-based division to ensure that negative
        /// world coordinates map to the correct chunk.
        /// </summary>
        /// <param name="worldCoord">
        /// The world-space tile coordinate to convert.
        /// </param>
        /// <returns>
        /// The chunk coordinate containing the specified world tile.
        /// </returns>
        internal static TerrainChunkGridCoord TileWorldCoordToChunkGridCoord(TerrainTileWorldCoord worldCoord)
        {
            int chunkX = FloorDiv(worldCoord.X, TerrainConfigs.ChunkSize);
            int chunkY = FloorDiv(worldCoord.Y, TerrainConfigs.ChunkSize);

            return new TerrainChunkGridCoord(chunkX, chunkY);
        }

        /// <summary>
        /// Converts a world-space tile coordinate into a chunk-local tile
        /// coordinate relative to the specified chunk.
        /// </summary>
        /// <param name="worldCoord">
        /// The world-space tile coordinate.
        /// </param>
        /// <param name="gridCoord">
        /// The chunk coordinate that owns the world tile.
        /// </param>
        /// <returns>
        /// A chunk-local tile coordinate guaranteed to be within
        /// <c>[0, ChunkSize - 1]</c> on both axes.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the calculated local coordinate falls outside the
        /// valid chunk bounds.
        /// </exception>
        internal static TerrainTileLocalCoord TileWorldCoordToLocalCoord(TerrainTileWorldCoord worldCoord, TerrainChunkGridCoord gridCoord)
        {
            int localX = worldCoord.X - (gridCoord.X * TerrainConfigs.ChunkSize);
            int localY = worldCoord.Y - (gridCoord.Y * TerrainConfigs.ChunkSize);

            return new TerrainTileLocalCoord(localX, localY);
        }

        /// <summary>
        /// Converts a chunk grid coordinate into the world-space origin
        /// of the chunk, positioned at the centered tile-aligned origin
        /// used by the terrain system.
        /// </summary>
        /// <param name="gridCoord">
        /// The logical grid coordinate of the terrain chunk.
        /// </param>
        /// <returns>
        /// The world-space coordinate representing the centered origin
        /// of the specified chunk.
        /// </returns>
        internal static TerrainChunkWorldOrigin ChunkGridCoordToWorldOrigin(TerrainChunkGridCoord gridCoord)
        {
            float tileCenter = 0.5f;
            float chunkCenter = TerrainConfigs.ChunkSize / 2;
            float worldX = (gridCoord.X * TerrainConfigs.ChunkSize) - chunkCenter - tileCenter;
            float worldY = (gridCoord.Y * TerrainConfigs.ChunkSize) - chunkCenter - tileCenter;

            return new TerrainChunkWorldOrigin(worldX, worldY);
        }
    }
}
