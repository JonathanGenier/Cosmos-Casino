namespace CosmosCasino.Core.Game.Map.Terrain.Chunk
{
    /// <summary>
    /// Represents an immutable coordinate identifying a terrain chunk
    /// within the chunk grid.
    /// This type is used for logical addressing, bounds checks, and
    /// chunk-based spatial indexing.
    /// </summary>
    public readonly record struct TerrainChunkGridCoord
    {
        #region Initialization

        /// <summary>
        /// Initializes a new terrain chunk coordinate with the specified
        /// grid-space values.
        /// </summary>
        /// <param name="x">
        /// Chunk X coordinate.
        /// </param>
        /// <param name="y">
        /// Chunk Y coordinate.
        /// </param>
        public TerrainChunkGridCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the X coordinate of the chunk within the chunk grid.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y coordinate of the chunk within the chunk grid.
        /// </summary>
        public int Y { get; }

        #endregion
    }
}