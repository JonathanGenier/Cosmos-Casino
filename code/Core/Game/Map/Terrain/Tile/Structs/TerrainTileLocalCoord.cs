using CosmosCasino.Core.Configs;

namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Represents a chunk-local terrain tile coordinate.
    /// This coordinate identifies a tile within the bounds of a single
    /// terrain chunk and is guaranteed to be valid for indexing
    /// chunk-owned tile storage.
    /// </summary>
    /// <remarks>
    /// This type is part of the internal terrain coordinate system and
    /// should never represent world-space positions. All instances are
    /// range-validated at construction time to preserve structural
    /// invariants across the terrain system.
    /// </remarks>
    public readonly partial record struct TerrainTileLocalCoord
    {
        #region Initialization

        /// <summary>
        /// Initializes a new chunk-local tile coordinate using unsigned values.
        /// </summary>
        /// <param name="x">
        /// The local X coordinate within the chunk.
        /// </param>
        /// <param name="y">
        /// The local Y coordinate within the chunk.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if either coordinate is greater than or equal to
        /// <see cref="TerrainConfigs.ChunkSize"/>.
        /// </exception>
        internal TerrainTileLocalCoord(uint x, uint y)
        {
            if (x >= TerrainConfigs.ChunkSize || y >= TerrainConfigs.ChunkSize)
            {
                throw new ArgumentOutOfRangeException($"Tile's Local Coord out of range: {x},{y}");
            }

            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new chunk-local tile coordinate using signed values.
        /// </summary>
        /// <param name="x">
        /// The local X coordinate within the chunk.
        /// </param>
        /// <param name="y">
        /// The local Y coordinate within the chunk.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if either coordinate is negative or greater than or equal to
        /// <see cref="TerrainConfigs.ChunkSize"/>.
        /// </exception>
        internal TerrainTileLocalCoord(int x, int y)
        {
            if (x < 0 || y < 0 || x >= TerrainConfigs.ChunkSize || y >= TerrainConfigs.ChunkSize)
            {
                throw new ArgumentOutOfRangeException($"Tile's Local coord out of range: {x},{y}");
            }

            X = (uint)x;
            Y = (uint)y;
        }

        #endregion
    }
}