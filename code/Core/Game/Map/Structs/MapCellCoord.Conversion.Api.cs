namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Provides public conversion helpers for authoritative map-cell coordinates.
    /// </summary>
    public readonly partial struct MapCellCoord
    {
        #region Conversion

        /// <summary>
        /// Converts a legacy horizontal map coordinate and logical elevation into a global map-cell coordinate.
        /// </summary>
        /// <param name="coord">The legacy horizontal coordinate.</param>
        /// <param name="elevation">The logical elevation.</param>
        /// <returns>The equivalent authoritative X/Y/Z map-cell coordinate.</returns>
        public static MapCellCoord FromMapCoord(MapCoord coord, Elevation elevation)
        {
            return new MapCellCoord(coord.X, elevation.MapCellY, coord.Y);
        }

        /// <summary>
        /// Converts this map-cell coordinate to the legacy horizontal coordinate used by current terrain and visuals.
        /// </summary>
        /// <returns>The horizontal map coordinate represented by this cell.</returns>
        public MapCoord ToMapCoord()
        {
            return new MapCoord(X, Z);
        }

        /// <summary>
        /// Converts this coordinate's vertical cell component to a logical elevation.
        /// </summary>
        /// <returns>The elevation represented by this coordinate's Y value.</returns>
        public Elevation ToElevation()
        {
            return Elevation.FromMapCellY(Y);
        }

        #endregion
    }
}
