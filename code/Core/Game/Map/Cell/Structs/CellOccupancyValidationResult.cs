namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents the result of validating a local map cell occupancy reservation.
    /// </summary>
    internal readonly struct CellOccupancyValidationResult
    {
        #region Initialization

        private CellOccupancyValidationResult(
            CellOccupancyOutcome outcome,
            CellOccupancyFailureReason failureReason)
        {
            Outcome = outcome;
            FailureReason = failureReason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the validation outcome.
        /// </summary>
        internal CellOccupancyOutcome Outcome { get; }

        /// <summary>
        /// Gets the reason validation failed, if any.
        /// </summary>
        internal CellOccupancyFailureReason FailureReason { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a result indicating the occupancy reservation can be applied.
        /// </summary>
        /// <returns>A valid occupancy validation result.</returns>
        internal static CellOccupancyValidationResult Valid()
        {
            return new(CellOccupancyOutcome.Valid, CellOccupancyFailureReason.None);
        }

        /// <summary>
        /// Creates a result indicating the requested occupancy reservation already exists.
        /// </summary>
        /// <returns>A no-op occupancy validation result.</returns>
        internal static CellOccupancyValidationResult NoOp()
        {
            return new(CellOccupancyOutcome.NoOp, CellOccupancyFailureReason.None);
        }

        /// <summary>
        /// Creates a result indicating the occupancy reservation conflicts with existing state.
        /// </summary>
        /// <param name="failureReason">The occupancy conflict reason.</param>
        /// <returns>An invalid occupancy validation result.</returns>
        internal static CellOccupancyValidationResult Invalid(CellOccupancyFailureReason failureReason)
        {
            return new(CellOccupancyOutcome.Invalid, failureReason);
        }

        #endregion
    }
}
