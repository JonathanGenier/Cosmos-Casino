using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Structures
{
    /// <summary>
    /// Represents the aggregate result of an authoritative structure lifecycle operation.
    /// </summary>
    internal readonly struct StructureOperationResult
    {
        #region Initialization

        private StructureOperationResult(
            StructureOperationOutcome outcome,
            StructureOperationFailureReason failureReason,
            MapCellFootprintTransactionResult? footprintResult)
        {
            Outcome = outcome;
            FailureReason = failureReason;
            FootprintResult = footprintResult;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the structure operation outcome.
        /// </summary>
        internal StructureOperationOutcome Outcome { get; }

        /// <summary>
        /// Gets the structure-level failure reason, if any.
        /// </summary>
        internal StructureOperationFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the footprint transaction result that caused this structure operation to fail, when applicable.
        /// </summary>
        internal MapCellFootprintTransactionResult? FootprintResult { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a result indicating the structure operation completed.
        /// </summary>
        /// <returns>A valid structure operation result.</returns>
        internal static StructureOperationResult Valid()
        {
            return new(StructureOperationOutcome.Valid, StructureOperationFailureReason.None, null);
        }

        /// <summary>
        /// Creates a result indicating the structure operation had no state to mutate.
        /// </summary>
        /// <returns>A no-op structure operation result.</returns>
        internal static StructureOperationResult NoOp()
        {
            return new(StructureOperationOutcome.NoOp, StructureOperationFailureReason.None, null);
        }

        /// <summary>
        /// Creates a result indicating the structure operation was rejected.
        /// </summary>
        /// <param name="failureReason">The structure-level failure reason.</param>
        /// <param name="footprintResult">The footprint transaction result that caused the failure, when applicable.</param>
        /// <returns>An invalid structure operation result.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="failureReason"/> does not describe a failure.
        /// </exception>
        internal static StructureOperationResult Invalid(
            StructureOperationFailureReason failureReason,
            MapCellFootprintTransactionResult? footprintResult = null)
        {
            if (failureReason == StructureOperationFailureReason.None)
            {
                throw new ArgumentException("Invalid structure operation results must include a failure reason.", nameof(failureReason));
            }

            return new(StructureOperationOutcome.Invalid, failureReason, footprintResult);
        }

        #endregion
    }
}
