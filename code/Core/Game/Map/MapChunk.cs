namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Authoritative spatial aggregate identity for one horizontal X/Z map-chunk region.
    /// </summary>
    internal sealed class MapChunk
    {
        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="MapChunk"/> with the specified chunk coordinate.
        /// </summary>
        /// <param name="coord">The global X/Z chunk coordinate identifying this map chunk.</param>
        internal MapChunk(MapChunkCoord coord)
        {
            Coord = coord;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets this chunk's global X/Z identity.
        /// </summary>
        internal MapChunkCoord Coord { get; }

        /// <summary>
        /// Gets the authoritative chunk size used by this chunk.
        /// </summary>
        internal int ChunkSize => MapChunkMetrics.ChunkSize;

        #endregion

        #region Spatial Queries

        /// <summary>
        /// Determines whether the specified global cell coordinate belongs to this chunk's X/Z region.
        /// </summary>
        /// <param name="cellCoord">The global logical cell coordinate to query.</param>
        /// <returns><c>true</c> if <paramref name="cellCoord"/> belongs to this chunk; otherwise, <c>false</c>.</returns>
        internal bool Contains(MapCellCoord cellCoord)
        {
            return MapMath.CellToChunk(cellCoord) == Coord;
        }

        #endregion
    }
}
