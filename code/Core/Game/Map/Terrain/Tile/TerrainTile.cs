namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Represents a single terrain tile defined by corner height samples,
    /// providing slope classification and neighbor slope metadata used by
    /// higher-level terrain processing and rendering systems.
    /// </summary>
    public sealed class TerrainTile
    {
        #region Initialization

        /// <summary>
        /// Initializes a terrain tile using height samples from its four corner points.
        /// </summary>
        /// <param name="topLeftHeight">Height at the tile’s top-left corner.</param>
        /// <param name="topRightHeight">Height at the tile’s top-right corner.</param>
        /// <param name="bottomLeftHeight">Height at the tile’s bottom-left corner.</param>
        /// <param name="bottomRightHeight">Height at the tile’s bottom-right corner.</param>
        internal TerrainTile(float topLeftHeight, float topRightHeight, float bottomLeftHeight, float bottomRightHeight)
        {
            TopLeftHeight = topLeftHeight;
            TopRightHeight = topRightHeight;
            BottomLeftHeight = bottomLeftHeight;
            BottomRightHeight = bottomRightHeight;
            IsSlope = TopLeftHeight != TopRightHeight || TopLeftHeight != BottomLeftHeight || TopLeftHeight != BottomRightHeight;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The height value sampled at the tile's top-left corner.
        /// </summary>
        public float TopLeftHeight { get; private set; }

        /// <summary>
        /// The height value sampled at the tile's top-right corner.
        /// </summary>
        public float TopRightHeight { get; private set; }

        /// <summary>
        /// The height value sampled at the tile's bottom-left corner.
        /// </summary>
        public float BottomLeftHeight { get; private set; }

        /// <summary>
        /// The height value sampled at the tile's bottom-right corner.
        /// </summary>
        public float BottomRightHeight { get; private set; }

        /// <summary>
        /// Indicates whether the tile represents a slope.
        /// A tile is considered sloped if any of its corner height values
        /// differ from one another.
        /// </summary>
        public bool IsSlope { get; private set; }

        /// <summary>
        /// Bitmask describing which neighboring tiles are sloped.
        /// This property is only meaningful for flat tiles and is used to
        /// inform terrain smoothing, blending, or visual transition logic.
        /// </summary>
        public SlopeNeighborMask SlopeNeighborMask { get; private set; }

        #endregion

        #region Slope Neighbor Management

        /// <summary>
        /// Records a neighboring sloped tile in the specified direction for flat tiles.
        /// </summary>
        /// <param name="direction">The direction of the neighboring slope.</param>
        internal void AddSlopeNeighbor(SlopeNeighborMask direction)
        {
            if (IsSlope)
            {
                return;
            }

            SlopeNeighborMask |= direction;
        }

        /// <summary>
        /// Clears all recorded slope neighbor information for this tile.
        /// </summary>
        internal void ClearSlopeNeighborMask()
        {
            SlopeNeighborMask = SlopeNeighborMask.None;
        }

        #endregion
    }
}