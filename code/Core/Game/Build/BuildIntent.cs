using CosmosCasino.Core.Game.Build.Domain;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Immutable command describing one structure-oriented build action.
    /// </summary>
    public sealed partial class BuildIntent
    {
        #region Initialization

        private BuildIntent(
            BuildOperation operation,
            IReadOnlyList<StructurePlacementRequest> placementRequests,
            IReadOnlyList<StructureRemovalRequest> removalRequests)
        {
            Operation = operation;
            PlacementRequests = placementRequests;
            RemovalRequests = removalRequests;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Creates a structure placement intent after validating and snapshotting caller input.
        /// </summary>
        /// <param name="placements">The requested structure placements.</param>
        /// <returns>An immutable build intent.</returns>
        private static BuildIntent CreatePlacementIntent(IReadOnlyList<StructurePlacementRequest> placements)
        {
            ArgumentNullException.ThrowIfNull(placements);

            if (placements.Count == 0)
            {
                throw new ArgumentException("A placement intent requires at least one structure request.", nameof(placements));
            }

            var snapshot = new StructurePlacementRequest[placements.Count];

            for (int i = 0; i < placements.Count; i++)
            {
                snapshot[i] = placements[i]
                    ?? throw new ArgumentException("Placement requests cannot contain null entries.", nameof(placements));
            }

            return new BuildIntent(
                BuildOperation.Place,
                Array.AsReadOnly(snapshot),
                Array.Empty<StructureRemovalRequest>());
        }

        /// <summary>
        /// Creates a structure removal intent after validating and snapshotting caller input.
        /// </summary>
        /// <param name="removals">The requested structure removal targets.</param>
        /// <returns>An immutable build intent.</returns>
        private static BuildIntent CreateRemovalIntent(IReadOnlyList<StructureRemovalRequest> removals)
        {
            ArgumentNullException.ThrowIfNull(removals);

            if (removals.Count == 0)
            {
                throw new ArgumentException("A removal intent requires at least one target cell.", nameof(removals));
            }

            var snapshot = new StructureRemovalRequest[removals.Count];

            for (int i = 0; i < removals.Count; i++)
            {
                snapshot[i] = removals[i];
            }

            return new BuildIntent(
                BuildOperation.Remove,
                Array.Empty<StructurePlacementRequest>(),
                Array.AsReadOnly(snapshot));
        }

        #endregion
    }
}
