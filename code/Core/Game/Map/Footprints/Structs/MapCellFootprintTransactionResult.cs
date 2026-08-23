namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents the aggregate result of validating or applying a map-cell footprint transaction.
    /// </summary>
    internal readonly struct MapCellFootprintTransactionResult
    {
        #region Initialization

        private MapCellFootprintTransactionResult(
            MapCellFootprintTransactionOutcome outcome,
            MapCellFootprintTransactionFailureReason failureReason,
            MapCellCoord? failedCoord,
            CellOccupancyFailureReason occupancyFailureReason)
        {
            Outcome = outcome;
            FailureReason = failureReason;
            FailedCoord = failedCoord;
            OccupancyFailureReason = occupancyFailureReason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the aggregate transaction outcome.
        /// </summary>
        internal MapCellFootprintTransactionOutcome Outcome { get; }

        /// <summary>
        /// Gets the map-level failure reason, if any.
        /// </summary>
        internal MapCellFootprintTransactionFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the first coordinate associated with a failed transaction, when available.
        /// </summary>
        internal MapCellCoord? FailedCoord { get; }

        /// <summary>
        /// Gets the cell-level occupancy failure reason, if a local occupancy conflict failed the transaction.
        /// </summary>
        internal CellOccupancyFailureReason OccupancyFailureReason { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a result indicating every resolved cell can be mutated.
        /// </summary>
        /// <returns>A valid footprint transaction result.</returns>
        internal static MapCellFootprintTransactionResult Valid()
        {
            return new(
                MapCellFootprintTransactionOutcome.Valid,
                MapCellFootprintTransactionFailureReason.None,
                null,
                CellOccupancyFailureReason.None);
        }

        /// <summary>
        /// Creates a result indicating every resolved cell already reflects the requested state.
        /// </summary>
        /// <returns>A no-op footprint transaction result.</returns>
        internal static MapCellFootprintTransactionResult NoOp()
        {
            return new(
                MapCellFootprintTransactionOutcome.NoOp,
                MapCellFootprintTransactionFailureReason.None,
                null,
                CellOccupancyFailureReason.None);
        }

        /// <summary>
        /// Creates a result indicating the footprint transaction cannot be applied.
        /// </summary>
        /// <param name="failureReason">The map-level failure reason.</param>
        /// <param name="failedCoord">The first coordinate associated with the failure, when available.</param>
        /// <param name="occupancyFailureReason">The cell-level occupancy failure reason, if applicable.</param>
        /// <returns>An invalid footprint transaction result.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="failureReason"/> does not describe a failure.
        /// </exception>
        internal static MapCellFootprintTransactionResult Invalid(
            MapCellFootprintTransactionFailureReason failureReason,
            MapCellCoord? failedCoord = null,
            CellOccupancyFailureReason occupancyFailureReason = CellOccupancyFailureReason.None)
        {
            if (failureReason == MapCellFootprintTransactionFailureReason.None)
            {
                throw new ArgumentException("Invalid transaction results must include a failure reason.", nameof(failureReason));
            }

            return new(
                MapCellFootprintTransactionOutcome.Invalid,
                failureReason,
                failedCoord,
                occupancyFailureReason);
        }

        #endregion
    }
}
