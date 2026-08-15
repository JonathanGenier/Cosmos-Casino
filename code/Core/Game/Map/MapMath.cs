namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Authoritative map coordinate conversion utilities.
    /// A map cell (x, y) is centered at world (x, y) and occupies [x - 0.5, x + 0.5) by [y - 0.5, y + 0.5).
    /// </summary>
    public static class MapMath
    {
        #region World To Cell

        /// <summary>
        /// Returns the map cell containing the specified horizontal world position.
        /// Uses half-open centered cell bounds so boundary ties resolve to the cell with the greater coordinate.
        /// </summary>
        /// <param name="position">
        /// The horizontal world position to resolve.
        /// </param>
        /// <returns>
        /// The map cell containing <paramref name="position"/>.
        /// </returns>
        public static MapCoord WorldToCell(WorldCoord position)
        {
            return WorldToCell(position.X, position.Y);
        }

        /// <summary>
        /// Returns the map cell containing the specified horizontal world coordinates.
        /// Uses half-open centered cell bounds so boundary ties resolve to the cell with the greater coordinate.
        /// </summary>
        /// <param name="worldX">
        /// Horizontal world X coordinate.
        /// </param>
        /// <param name="worldY">
        /// Horizontal world Y coordinate.
        /// </param>
        /// <returns>
        /// The map cell containing the specified world coordinates.
        /// </returns>
        public static MapCoord WorldToCell(float worldX, float worldY)
        {
            return new MapCoord(
                FloorToInt((worldX + WorldGridMetrics.HalfGridUnitSize) / WorldGridMetrics.GridUnitSize),
                FloorToInt((worldY + WorldGridMetrics.HalfGridUnitSize) / WorldGridMetrics.GridUnitSize)
            );
        }

        #endregion

        #region Cell To World

        /// <summary>
        /// Returns the world-space origin of a map cell.
        /// For cell (x, y), this is the inclusive minimum corner (x - 0.5, y - 0.5).
        /// </summary>
        /// <param name="cell">
        /// The map cell to resolve.
        /// </param>
        /// <returns>
        /// The world-space origin of <paramref name="cell"/>.
        /// </returns>
        public static WorldCoord CellToWorldOrigin(MapCoord cell)
        {
            return new WorldCoord(
                (cell.X * WorldGridMetrics.GridUnitSize) - WorldGridMetrics.HalfGridUnitSize,
                (cell.Y * WorldGridMetrics.GridUnitSize) - WorldGridMetrics.HalfGridUnitSize
            );
        }

        /// <summary>
        /// Returns the world-space center of a map cell.
        /// For cell (x, y), this is (x, y).
        /// </summary>
        /// <param name="cell">
        /// The map cell to resolve.
        /// </param>
        /// <returns>
        /// The world-space center of <paramref name="cell"/>.
        /// </returns>
        public static WorldCoord CellToWorldCenter(MapCoord cell)
        {
            return new WorldCoord(
                cell.X * WorldGridMetrics.GridUnitSize,
                cell.Y * WorldGridMetrics.GridUnitSize
            );
        }

        #endregion

        #region Helpers

        private static int FloorToInt(float value)
        {
            return (int)MathF.Floor(value);
        }

        #endregion
    }
}
