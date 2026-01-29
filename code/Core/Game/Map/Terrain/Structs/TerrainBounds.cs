using CosmosCasino.Core.Configs;

namespace CosmosCasino.Core.Game.Map.Terrain
{
    /// <summary>
    /// Internal construction and factory logic for <see cref="TerrainBounds"/>.
    /// This partial definition encapsulates creation rules and configuration-
    /// dependent invariants that are not exposed through the public API.
    /// </summary>
    public readonly partial struct TerrainBounds
    {
        #region Intialization

        /// <summary>
        /// Initializes a new <see cref="TerrainBounds"/> instance with the specified
        /// inclusive coordinate limits.
        /// </summary>
        /// <param name="minX">
        /// Minimum X coordinate included in the bounds.
        /// </param>
        /// <param name="maxX">
        /// Maximum X coordinate included in the bounds.
        /// </param>
        /// <param name="minY">
        /// Minimum Y coordinate included in the bounds.
        /// </param>
        /// <param name="maxY">
        /// Maximum Y coordinate included in the bounds.
        /// </param>
        private TerrainBounds(int minX, int maxX, int minY, int maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        #endregion

        #region Factory

        /// <summary>
        /// Creates a new set of terrain bounds based on the configured
        /// chunk count per axis.
        /// The resulting bounds are centered around the origin and form
        /// a square region of chunks.
        /// </summary>
        /// <returns>
        /// A new <see cref="TerrainBounds"/> instance representing the
        /// playable chunk area.
        /// </returns>
        /// <param name="chunkCountPerAxis">Number of chunk per axis (X,Y) to define map size.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="TerrainConfigs.ChunkCountPerAxis"/> is not an odd value.
        /// </exception>
        internal static TerrainBounds CreateNew(int chunkCountPerAxis)
        {
            if ((chunkCountPerAxis & 1) == 0)
            {
                throw new InvalidOperationException($"{nameof(TerrainConfigs.ChunkCountPerAxis)} must be odd.");
            }

            int half = chunkCountPerAxis / 2;

            var minX = -half;
            var maxX = half;
            var minY = -half;
            var maxY = half;

            return new TerrainBounds(minX, maxX, minY, maxY);
        }

        #endregion
    }
}