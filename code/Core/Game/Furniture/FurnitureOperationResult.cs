using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Immutable result representing the aggregate outcome of a furniture operation.
    /// </summary>
    public sealed class FurnitureOperationResult
    {
        #region Initialization

        private FurnitureOperationResult(
            FurnitureOperation operation,
            FurnitureOperationOutcome outcome,
            FurnitureFailureReason failureReason,
            MapCellCoord? failedCell,
            FurnitureDefinitionId? failedDefinitionId,
            IReadOnlyList<FurnitureChangeResult> changes)
        {
            ArgumentNullException.ThrowIfNull(changes);

            Operation = operation;
            Outcome = outcome;
            FailureReason = failureReason;
            FailedCell = failedCell;
            FailedDefinitionId = failedDefinitionId;
            Changes = changes.ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the furniture operation that was evaluated or executed.
        /// </summary>
        public FurnitureOperation Operation { get; }

        /// <summary>
        /// Gets the aggregate furniture operation outcome.
        /// </summary>
        public FurnitureOperationOutcome Outcome { get; }

        /// <summary>
        /// Gets the aggregate furniture failure reason, if the operation was invalid.
        /// </summary>
        public FurnitureFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the first failed target cell, when Core can identify one deterministically.
        /// </summary>
        public MapCellCoord? FailedCell { get; }

        /// <summary>
        /// Gets the failed furniture definition identity, when the failure is tied to a definition.
        /// </summary>
        public FurnitureDefinitionId? FailedDefinitionId { get; }

        /// <summary>
        /// Gets the furniture-level effects produced by the operation.
        /// </summary>
        public IReadOnlyList<FurnitureChangeResult> Changes { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates an immutable furniture result from a completed planning or execution pass.
        /// </summary>
        /// <param name="operation">The operation that was evaluated or executed.</param>
        /// <param name="outcome">The aggregate outcome.</param>
        /// <param name="failureReason">The aggregate failure reason.</param>
        /// <param name="failedCell">The first failed cell, when available.</param>
        /// <param name="failedDefinitionId">The failed furniture definition identity, when available.</param>
        /// <param name="changes">The furniture-level effects in deterministic order.</param>
        /// <returns>An immutable furniture operation result.</returns>
        /// <exception cref="ArgumentException">Thrown when outcome and failure reason are inconsistent.</exception>
        internal static FurnitureOperationResult Done(
            FurnitureOperation operation,
            FurnitureOperationOutcome outcome,
            FurnitureFailureReason failureReason,
            MapCellCoord? failedCell,
            FurnitureDefinitionId? failedDefinitionId,
            IReadOnlyList<FurnitureChangeResult> changes)
        {
            if (outcome != FurnitureOperationOutcome.Invalid && failureReason != FurnitureFailureReason.None)
            {
                throw new ArgumentException("Successful or no-op furniture results cannot include a failure reason.", nameof(failureReason));
            }

            if (outcome == FurnitureOperationOutcome.Invalid && failureReason == FurnitureFailureReason.None)
            {
                throw new ArgumentException("Invalid furniture results must include a failure reason.", nameof(failureReason));
            }

            return new FurnitureOperationResult(
                operation,
                outcome,
                failureReason,
                failedCell,
                failedDefinitionId,
                changes);
        }

        #endregion
    }
}
