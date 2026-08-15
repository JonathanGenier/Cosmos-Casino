namespace CosmosCasino.Core.Game.Map.Terrain
{
    /// <summary>
    /// Terrain coordinate conversion utilities for signed world tiles, signed chunk-grid coordinates,
    /// and zero-based chunk-local coordinates.
    /// </summary>
    public static class TerrainMath
    {
        #region Tile Coordinates

        /// <summary>
        /// Converts a zero-based tile index on one axis into a signed, centered world-tile coordinate.
        /// </summary>
        /// <param name="index">The zero-based tile index.</param>
        /// <param name="tileCount">The total tile count along the axis.</param>
        /// <returns>The signed world-tile coordinate represented by <paramref name="index"/>.</returns>
        public static int TileIndexToWorldCoord(int index, int tileCount)
        {
            ValidatePositiveOddCount(tileCount, nameof(tileCount));
            ValidateIndex(index, tileCount, nameof(index));

            return index - (tileCount / 2);
        }

        /// <summary>
        /// Converts zero-based tile indexes into a signed, centered world-tile coordinate.
        /// </summary>
        /// <param name="xIndex">The zero-based tile X index.</param>
        /// <param name="yIndex">The zero-based tile Y index.</param>
        /// <param name="tileCount">The total tile count along each axis.</param>
        /// <returns>The signed world-tile coordinate represented by the indexes.</returns>
        public static TerrainTileWorldCoord TileIndexToWorldCoord(int xIndex, int yIndex, int tileCount)
        {
            return new TerrainTileWorldCoord(
                TileIndexToWorldCoord(xIndex, tileCount),
                TileIndexToWorldCoord(yIndex, tileCount)
            );
        }

        /// <summary>
        /// Returns the world-space center of a terrain tile.
        /// </summary>
        /// <param name="coord">The signed terrain world-tile coordinate.</param>
        /// <returns>The world-space center of <paramref name="coord"/>.</returns>
        public static WorldCoord TileToWorldCenter(TerrainTileWorldCoord coord)
        {
            return new WorldCoord(
                coord.X * WorldGridMetrics.GridUnitSize,
                coord.Y * WorldGridMetrics.GridUnitSize
            );
        }

        /// <summary>
        /// Returns the world-space origin of a terrain tile.
        /// </summary>
        /// <param name="coord">The signed terrain world-tile coordinate.</param>
        /// <returns>The minimum world-space corner of <paramref name="coord"/>.</returns>
        public static WorldCoord TileToWorldOrigin(TerrainTileWorldCoord coord)
        {
            return new WorldCoord(
                (coord.X * WorldGridMetrics.GridUnitSize) - WorldGridMetrics.HalfGridUnitSize,
                (coord.Y * WorldGridMetrics.GridUnitSize) - WorldGridMetrics.HalfGridUnitSize
            );
        }

        #endregion

        #region Chunk Coordinates

        /// <summary>
        /// Converts a zero-based chunk index on one axis into a signed, centered chunk-grid coordinate.
        /// </summary>
        /// <param name="index">The zero-based chunk index.</param>
        /// <param name="chunkCount">The total chunk count along the axis.</param>
        /// <returns>The signed chunk-grid coordinate represented by <paramref name="index"/>.</returns>
        public static int ChunkIndexToGridCoord(int index, int chunkCount)
        {
            ValidatePositiveOddCount(chunkCount, nameof(chunkCount));
            ValidateIndex(index, chunkCount, nameof(index));

            return index - (chunkCount / 2);
        }

        /// <summary>
        /// Converts zero-based chunk indexes into a signed, centered chunk-grid coordinate.
        /// </summary>
        /// <param name="xIndex">The zero-based chunk X index.</param>
        /// <param name="yIndex">The zero-based chunk Y index.</param>
        /// <param name="chunkCount">The total chunk count along each axis.</param>
        /// <returns>The signed chunk-grid coordinate represented by the indexes.</returns>
        public static TerrainChunkGridCoord ChunkIndexToGridCoord(int xIndex, int yIndex, int chunkCount)
        {
            return new TerrainChunkGridCoord(
                ChunkIndexToGridCoord(xIndex, chunkCount),
                ChunkIndexToGridCoord(yIndex, chunkCount)
            );
        }

        /// <summary>
        /// Converts a signed chunk-grid coordinate and zero-based chunk-local coordinate into a signed world-tile coordinate.
        /// </summary>
        /// <param name="chunkCoord">The signed chunk-grid coordinate.</param>
        /// <param name="localCoord">The zero-based local tile coordinate inside the chunk.</param>
        /// <param name="chunkSize">The number of tiles along one axis of a chunk.</param>
        /// <returns>The signed world-tile coordinate represented by the chunk and local coordinates.</returns>
        public static TerrainTileWorldCoord ChunkLocalToWorldTileCoord(
            TerrainChunkGridCoord chunkCoord,
            TerrainChunkLocalCoord localCoord,
            int chunkSize)
        {
            ValidatePositiveOddCount(chunkSize, nameof(chunkSize));
            ValidateIndex(localCoord.X, chunkSize, nameof(localCoord));
            ValidateIndex(localCoord.Y, chunkSize, nameof(localCoord));

            int chunkMinX = (chunkCoord.X * chunkSize) - (chunkSize / 2);
            int chunkMinY = (chunkCoord.Y * chunkSize) - (chunkSize / 2);

            return new TerrainTileWorldCoord(
                chunkMinX + localCoord.X,
                chunkMinY + localCoord.Y
            );
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates that the count can produce one unique center tile or chunk.
        /// </summary>
        /// <param name="count">The count to validate.</param>
        /// <param name="paramName">The parameter name used when throwing an exception.</param>
        public static void ValidatePositiveOddCount(int count, string paramName)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, count, "Count must be greater than zero.");
            }

            if (count % 2 == 0)
            {
                throw new ArgumentException("Count must be odd to provide a unique center coordinate.", paramName);
            }
        }

        private static void ValidateIndex(int index, int count, string paramName)
        {
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException(paramName, index, $"Index must be within [0, {count}).");
            }
        }

        #endregion
    }
}
