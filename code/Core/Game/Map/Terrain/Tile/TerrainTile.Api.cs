namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Public API for a single terrain tile.
    /// A terrain tile represents a unit of terrain at tile resolution and exposes
    /// read-only geometric and slope-related information derived from terrain
    /// height generation. All values are populated during the tile generation
    /// phase and remain immutable thereafter.
    /// </summary>
    public sealed partial class TerrainTile
    {
        #region Properties

        /// <summary>
        /// The tile's coordinate in local (chunk-relative) space.
        /// This coordinate is used for indexing and adjacency operations
        /// within a single terrain chunk and does not represent world-scale
        /// positioning.
        /// </summary>
        public TerrainTileLocalCoord LocalCoord { get; }

        /// <summary>
        /// The tile's coordinate in world space.
        /// This coordinate uniquely identifies the tile within the terrain map
        /// and is used as the basis for height sampling and world-level queries.
        /// </summary>
        public TerrainTileWorldCoord WorldCoord { get; }

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
        public SlopeNeighborMask SlopeNeighbors { get; private set; }

        /// <summary>
        /// Indicates whether the tile has at least one neighboring slope.
        /// </summary>
        public bool HasAnySlopeNeighbor => SlopeNeighbors != SlopeNeighborMask.None;

        #endregion
    }
}
