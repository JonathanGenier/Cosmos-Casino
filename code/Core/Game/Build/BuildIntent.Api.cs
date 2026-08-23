using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Immutable command describing a structure placement or structure-removal request.
    /// </summary>
    public sealed partial class BuildIntent
    {
        #region Properties

        /// <summary>
        /// Gets the requested aggregate build operation.
        /// </summary>
        public BuildOperation Operation { get; }

        /// <summary>
        /// Gets the structure placement requests when this is a placement intent.
        /// </summary>
        public IReadOnlyList<StructurePlacementRequest> PlacementRequests { get; }

        /// <summary>
        /// Gets the structure removal target requests when this is a removal intent.
        /// </summary>
        public IReadOnlyList<StructureRemovalRequest> RemovalRequests { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a build intent to place a batch of structures.
        /// </summary>
        /// <param name="placements">The structure placement requests to evaluate or execute.</param>
        /// <returns>An immutable structure placement intent.</returns>
        public static BuildIntent PlaceStructures(IReadOnlyList<StructurePlacementRequest> placements)
        {
            return CreatePlacementIntent(placements);
        }

        /// <summary>
        /// Creates a build intent to place one structure.
        /// </summary>
        /// <param name="definition">The structure definition to place.</param>
        /// <param name="anchor">The authoritative map-cell anchor.</param>
        /// <param name="rotation">The footprint rotation.</param>
        /// <returns>An immutable single-structure placement intent.</returns>
        public static BuildIntent PlaceStructure(
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            return PlaceStructures(new[]
            {
                new StructurePlacementRequest(definition, anchor, rotation)
            });
        }

        /// <summary>
        /// Creates a build intent to remove the structures occupying the specified target cells.
        /// </summary>
        /// <param name="targetCells">The target cells Core should resolve to authoritative structures.</param>
        /// <returns>An immutable structure-removal intent.</returns>
        public static BuildIntent RemoveStructuresAt(IReadOnlyList<MapCellCoord> targetCells)
        {
            ArgumentNullException.ThrowIfNull(targetCells);

            var removals = new StructureRemovalRequest[targetCells.Count];

            for (int i = 0; i < targetCells.Count; i++)
            {
                removals[i] = new StructureRemovalRequest(targetCells[i]);
            }

            return CreateRemovalIntent(Array.AsReadOnly(removals));
        }

        /// <summary>
        /// Creates a build intent to remove the structure occupying one target cell.
        /// </summary>
        /// <param name="targetCell">The target cell Core should resolve to an authoritative structure.</param>
        /// <returns>An immutable single-target structure-removal intent.</returns>
        public static BuildIntent RemoveStructureAt(MapCellCoord targetCell)
        {
            return RemoveStructuresAt(new[] { targetCell });
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a human-readable representation of the build intent,
        /// suitable for debugging and logging.
        /// </summary>
        /// <returns>
        /// A string describing the build action, target cell count, and shared logical build elevation.
        /// </returns>
        public override string ToString()
        {
            return Operation switch
            {
                BuildOperation.Place => $"Place {PlacementRequests.Count} structures",
                BuildOperation.Remove => $"Remove structures from {RemovalRequests.Count} target cells",
                _ => $"{Operation} structure intent"
            };
        }

        #endregion
    }
}
