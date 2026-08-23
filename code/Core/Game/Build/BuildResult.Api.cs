using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Immutable result representing the aggregate outcome of a structure build attempt.
    /// </summary>
    public sealed partial class BuildResult
    {
        #region Properties

        /// <summary>
        /// Gets the build intent that was evaluated or executed.
        /// </summary>
        public BuildIntent Intent { get; }

        /// <summary>
        /// Gets the aggregate build outcome for the complete intent.
        /// </summary>
        public BuildOperationOutcome Outcome { get; }

        /// <summary>
        /// Gets the aggregate build failure reason, if the intent was invalid.
        /// </summary>
        public BuildFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the first failed target cell, when Core can identify one deterministically.
        /// </summary>
        public MapCellCoord? FailedCell { get; }

        /// <summary>
        /// Gets the first failed structure definition identity, when the failure is tied to a definition.
        /// </summary>
        public StructureDefinitionId? FailedDefinitionId { get; }

        /// <summary>
        /// Gets the structure-level effects produced by the build plan.
        /// </summary>
        public IReadOnlyList<BuildStructureResult> Structures { get; }

        #endregion
    }
}
