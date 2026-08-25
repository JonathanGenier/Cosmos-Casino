using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Structures;

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

        /// <summary>
        /// Attempts to resolve the authoritative furniture identity occupying the specified global map cell.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to query.</param>
        /// <param name="furnitureId">The authoritative furniture identity when the cell is occupied by furniture.</param>
        /// <returns><c>true</c> when the cell references existing furniture; otherwise, <c>false</c>.</returns>
        public bool TryGetFurnitureIdAt(MapCellCoord coord, out FurnitureId furnitureId)
        {
            if (TryGetFurnitureAt(coord, out var furniture))
            {
                furnitureId = furniture.Id;
                return true;
            }

            furnitureId = default;
            return false;
        }

        /// <summary>
        /// Gets immutable domain snapshots for all authoritative structures in deterministic identity order.
        /// </summary>
        /// <returns>The current authoritative structure snapshots.</returns>
        public IReadOnlyList<StructureSnapshot> GetStructureSnapshots()
        {
            StructureSnapshot[] snapshots = _structures.Values
                .OrderBy(static structure => structure.Id.Value)
                .Select(static structure => new StructureSnapshot(
                    structure.Id,
                    structure.Definition,
                    structure.Anchor,
                    structure.Rotation))
                .ToArray();

            return Array.AsReadOnly(snapshots);
        }

        /// <summary>
        /// Attempts to resolve an immutable domain snapshot for the structure occupying the specified global map cell.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to query.</param>
        /// <param name="snapshot">The authoritative structure snapshot when the cell is occupied by a structure.</param>
        /// <returns><c>true</c> when the cell references an existing structure; otherwise, <c>false</c>.</returns>
        public bool TryGetStructureSnapshotAt(MapCellCoord coord, out StructureSnapshot snapshot)
        {
            if (TryGetStructureAt(coord, out var structure))
            {
                snapshot = new StructureSnapshot(
                    structure.Id,
                    structure.Definition,
                    structure.Anchor,
                    structure.Rotation);
                return true;
            }

            snapshot = default;
            return false;
        }

        /// <summary>
        /// Gets immutable domain snapshots for all authoritative furniture in deterministic identity order.
        /// </summary>
        /// <returns>The current authoritative furniture snapshots.</returns>
        public IReadOnlyList<FurnitureSnapshot> GetFurnitureSnapshots()
        {
            FurnitureSnapshot[] snapshots = _furniture.Values
                .OrderBy(static furniture => furniture.Id.Value)
                .Select(static furniture => new FurnitureSnapshot(
                    furniture.Id,
                    furniture.Definition,
                    furniture.Anchor,
                    furniture.Rotation))
                .ToArray();

            return Array.AsReadOnly(snapshots);
        }

        /// <summary>
        /// Attempts to resolve an immutable domain snapshot for the furniture occupying the specified global map cell.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to query.</param>
        /// <param name="snapshot">The authoritative furniture snapshot when the cell is occupied by furniture.</param>
        /// <returns><c>true</c> when the cell references existing furniture; otherwise, <c>false</c>.</returns>
        public bool TryGetFurnitureSnapshotAt(MapCellCoord coord, out FurnitureSnapshot snapshot)
        {
            if (TryGetFurnitureAt(coord, out var furniture))
            {
                snapshot = new FurnitureSnapshot(
                    furniture.Id,
                    furniture.Definition,
                    furniture.Anchor,
                    furniture.Rotation);
                return true;
            }

            snapshot = default;
            return false;
        }
    }
}
