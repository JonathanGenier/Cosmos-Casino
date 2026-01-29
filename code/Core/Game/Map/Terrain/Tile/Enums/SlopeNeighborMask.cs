namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Bitmask describing which neighboring tiles around a terrain tile
    /// are sloped.
    /// This mask is primarily used for terrain smoothing, blending, and
    /// shader decisions based on adjacent slope relationships.
    /// </summary>
    [Flags]
    public enum SlopeNeighborMask : byte
    {
        /// <summary>
        /// Indicates that no neighboring tiles are sloped.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates a sloped neighbor to the north.
        /// </summary>
        North = 1 << 0,

        /// <summary>
        /// Indicates a sloped neighbor to the northeast.
        /// </summary>
        NorthEast = 1 << 1,

        /// <summary>
        /// Indicates a sloped neighbor to the east.
        /// </summary>
        East = 1 << 2,

        /// <summary>
        /// Indicates a sloped neighbor to the southeast.
        /// </summary>
        SouthEast = 1 << 3,

        /// <summary>
        /// Indicates a sloped neighbor to the south.
        /// </summary>
        South = 1 << 4,

        /// <summary>
        /// Indicates a sloped neighbor to the southwest.
        /// </summary>
        SouthWest = 1 << 5,

        /// <summary>
        /// Indicates a sloped neighbor to the west.
        /// </summary>
        West = 1 << 6,

        /// <summary>
        /// Indicates a sloped neighbor to the northwest.
        /// </summary>
        NorthWest = 1 << 7,

        /// <summary>
        /// Indicates that all neighboring tiles are sloped.
        /// </summary>
        All = North | NorthEast | East | SouthEast | South | SouthWest | West | NorthWest
    }
}