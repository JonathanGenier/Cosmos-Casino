namespace CosmosCasino.Core.Game.Map.Terrain.Chunk
{
    /// <summary>
    /// Represents the world-space origin of a terrain chunk used for
    /// rendering and spatial placement.
    /// This coordinate defines the centered visual anchor of the chunk
    /// and is expressed in continuous world units.
    /// </summary>
    public readonly record struct TerrainChunkWorldOrigin
    {
        #region Initialization

        /// <summary>
        /// Initializes a new world-space origin for a terrain chunk.
        /// </summary>
        /// <param name="x">
        /// The world-space X position of the chunk's visual origin.
        /// </param>
        /// <param name="y">
        /// The world-space Y position of the chunk's visual origin.
        /// </param>
        internal TerrainChunkWorldOrigin(float x, float y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the world-space X coordinate of the chunk's visual origin.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// Gets the world-space Y coordinate of the chunk's visual origin.
        /// </summary>
        public float Y { get; }

        #endregion
    }
}
