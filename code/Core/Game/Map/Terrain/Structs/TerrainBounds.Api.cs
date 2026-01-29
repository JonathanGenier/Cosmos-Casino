using CosmosCasino.Core.Game.Map.Terrain.Chunk;

namespace CosmosCasino.Core.Game.Map.Terrain
{
    /// <summary>
    /// Represents an axis-aligned, immutable set of terrain chunk coordinates.
    /// This type defines inclusive bounds used to determine which chunks are
    /// considered part of the active or playable terrain area.
    /// </summary>
    public readonly partial struct TerrainBounds
    {
        #region Properties

        /// <summary>
        /// Gets the minimum X coordinate included in the bounds.
        /// </summary>
        public int MinX { get; }

        /// <summary>
        /// Gets the maximum X coordinate included in the bounds.
        /// </summary>
        public int MaxX { get; }

        /// <summary>
        /// Gets the minimum Y coordinate included in the bounds.
        /// </summary>
        public int MinY { get; }

        /// <summary>
        /// Gets the maximum Y coordinate included in the bounds.
        /// </summary>
        public int MaxY { get; }

        /// <summary>
        /// Gets the width of the bounds in chunk units.
        /// The width is computed as an inclusive range.
        /// </summary>
        public int Width => MaxX - MinX + 1;

        /// <summary>
        /// Gets the height of the bounds in chunk units.
        /// The height is computed as an inclusive range.
        /// </summary>
        public int Height => MaxY - MinY + 1;

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified chunk coordinate lies within
        /// these bounds.
        /// </summary>
        /// <param name="coord">
        /// The chunk coordinate to test.
        /// </param>
        /// <returns>
        /// <c>true</c> if the coordinate is inside the bounds; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TerrainChunkGridCoord coord)
        {
            return coord.X >= MinX && coord.X <= MaxX && coord.Y >= MinY && coord.Y <= MaxY;
        }

        #endregion
    }
}