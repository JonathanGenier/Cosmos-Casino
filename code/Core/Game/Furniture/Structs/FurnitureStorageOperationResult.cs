using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Represents the aggregate result of an authoritative furniture storage lifecycle operation.
    /// </summary>
    internal readonly struct FurnitureStorageOperationResult
    {
        #region Initialization

        private FurnitureStorageOperationResult(
            FurnitureStorageOperationOutcome outcome,
            FurnitureStorageOperationFailureReason failureReason,
            MapCellFootprintTransactionResult? footprintResult)
        {
            Outcome = outcome;
            FailureReason = failureReason;
            FootprintResult = footprintResult;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the furniture storage operation outcome.
        /// </summary>
        internal FurnitureStorageOperationOutcome Outcome { get; }

        /// <summary>
        /// Gets the furniture storage failure reason, if any.
        /// </summary>
        internal FurnitureStorageOperationFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the footprint transaction result that caused this storage operation to fail, when applicable.
        /// </summary>
        internal MapCellFootprintTransactionResult? FootprintResult { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a result indicating the furniture storage operation completed.
        /// </summary>
        /// <returns>A valid furniture storage operation result.</returns>
        internal static FurnitureStorageOperationResult Valid()
        {
            return new(FurnitureStorageOperationOutcome.Valid, FurnitureStorageOperationFailureReason.None, null);
        }

        /// <summary>
        /// Creates a result indicating the furniture storage operation had no state to mutate.
        /// </summary>
        /// <returns>A no-op furniture storage operation result.</returns>
        internal static FurnitureStorageOperationResult NoOp()
        {
            return new(FurnitureStorageOperationOutcome.NoOp, FurnitureStorageOperationFailureReason.None, null);
        }

        /// <summary>
        /// Creates a result indicating the furniture storage operation was rejected.
        /// </summary>
        /// <param name="failureReason">The furniture storage failure reason.</param>
        /// <param name="footprintResult">The footprint transaction result that caused the failure, when applicable.</param>
        /// <returns>An invalid furniture storage operation result.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="failureReason"/> does not describe a failure.
        /// </exception>
        internal static FurnitureStorageOperationResult Invalid(
            FurnitureStorageOperationFailureReason failureReason,
            MapCellFootprintTransactionResult? footprintResult = null)
        {
            if (failureReason == FurnitureStorageOperationFailureReason.None)
            {
                throw new ArgumentException("Invalid furniture storage operation results must include a failure reason.", nameof(failureReason));
            }

            return new(FurnitureStorageOperationOutcome.Invalid, failureReason, footprintResult);
        }

        #endregion
    }
}
