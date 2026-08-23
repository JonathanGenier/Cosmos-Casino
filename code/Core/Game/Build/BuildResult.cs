using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Internal construction surface for immutable build results.
    /// </summary>
    public sealed partial class BuildResult
    {
        #region Initialization

        private BuildResult(
            BuildIntent intent,
            BuildOperationOutcome outcome,
            BuildFailureReason failureReason,
            MapCellCoord? failedCell,
            StructureDefinitionId? failedDefinitionId,
            IReadOnlyList<BuildStructureResult> structures)
        {
            ArgumentNullException.ThrowIfNull(intent);
            ArgumentNullException.ThrowIfNull(structures);

            Intent = intent;
            Outcome = outcome;
            FailureReason = failureReason;
            FailedCell = failedCell;
            FailedDefinitionId = failedDefinitionId;
            Structures = structures.ToArray();
        }

        #endregion

        #region Factories

        /// <summary>
        /// Creates an immutable build result from a completed planning or execution pass.
        /// </summary>
        /// <param name="intent">The build intent that was evaluated or executed.</param>
        /// <param name="outcome">The aggregate build outcome.</param>
        /// <param name="failureReason">The aggregate failure reason.</param>
        /// <param name="failedCell">The first failed cell, when available.</param>
        /// <param name="failedDefinitionId">The failed structure definition identity, when available.</param>
        /// <param name="structures">The structure-level effects in deterministic order.</param>
        /// <returns>An immutable build result.</returns>
        /// <exception cref="ArgumentException">Thrown when outcome and failure reason are inconsistent.</exception>
        internal static BuildResult Done(
            BuildIntent intent,
            BuildOperationOutcome outcome,
            BuildFailureReason failureReason,
            MapCellCoord? failedCell,
            StructureDefinitionId? failedDefinitionId,
            IReadOnlyList<BuildStructureResult> structures)
        {
            if (outcome != BuildOperationOutcome.Invalid && failureReason != BuildFailureReason.None)
            {
                throw new ArgumentException("Successful or no-op build results cannot include a failure reason.", nameof(failureReason));
            }

            if (outcome == BuildOperationOutcome.Invalid && failureReason == BuildFailureReason.None)
            {
                throw new ArgumentException("Invalid build results must include a failure reason.", nameof(failureReason));
            }

            return new BuildResult(
                intent,
                outcome,
                failureReason,
                failedCell,
                failedDefinitionId,
                structures);
        }

        #endregion
    }
}
