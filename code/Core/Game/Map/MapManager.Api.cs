namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Coordinates map-level systems including terrain generation and cell-based build operations.
    /// </summary>
    public sealed partial class MapManager
    {
        /// <summary>
        /// Attempts to retrieve the authoritative terrain base elevation at the specified map coordinate.
        /// </summary>
        /// <param name="coord">The map coordinate to query.</param>
        /// <param name="elevation">The terrain base elevation when the coordinate exists.</param>
        /// <returns><c>true</c> if terrain exists at the coordinate; otherwise <c>false</c>.</returns>
        public bool TryGetTerrainBaseElevation(MapCoord coord, out Elevation elevation)
        {
            if (TryGetTerrain(coord, out var terrainTile))
            {
                elevation = terrainTile.BaseElevation;
                return true;
            }

            elevation = default;
            return false;
        }

        /// <summary>
        /// Attempts to resolve the authoritative structure identity occupying the specified global map cell.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to query.</param>
        /// <param name="structureId">The authoritative structure identity when the cell is occupied by a structure.</param>
        /// <returns><c>true</c> when the cell references an existing structure; otherwise, <c>false</c>.</returns>
        public bool TryGetStructureIdAt(MapCellCoord coord, out StructureId structureId)
        {
            if (TryGetStructureAt(coord, out var structure))
            {
                structureId = structure.Id;
                return true;
            }

            structureId = default;
            return false;
        }
    }
}
