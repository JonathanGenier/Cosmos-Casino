using CosmosCasino.Core.Game.Build.Domain;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents the result of validating a map cell for a build operation, including the outcome and any associated
    /// failure reason.
    /// </summary>
    internal readonly struct CellValidationResult
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the MapCellValidationResult class with the specified outcome and failure
        /// reason.
        /// </summary>
        /// <param name="outcome">The result of the build operation for the map cell.</param>
        /// <param name="failureReason">The reason for the build operation failure, if applicable.</param>
        private CellValidationResult(BuildOperationOutcome outcome, BuildOperationFailureReason failureReason)
        {
            Outcome = outcome;
            FailureReason = failureReason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the outcome of the build operation.
        /// </summary>
        internal BuildOperationOutcome Outcome { get; }

        /// <summary>
        /// Gets the reason for the build operation failure, if any.
        /// </summary>
        internal BuildOperationFailureReason FailureReason { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a MapCellValidationResult that represents a valid operation with no failure reason.
        /// </summary>
        /// <returns>A MapCellValidationResult indicating a valid outcome and no failure reason.</returns>
        internal static CellValidationResult Valid()
        {
            return new(BuildOperationOutcome.Valid, BuildOperationFailureReason.None);
        }

        /// <summary>
        /// Returns a validation result indicating that no operation was performed.
        /// </summary>
        /// <returns>A <see cref="CellValidationResult"/> representing a no-op outcome with no failure reason.</returns>
        internal static CellValidationResult NoOp()
        {
            return new(BuildOperationOutcome.NoOp, BuildOperationFailureReason.None);
        }

        /// <summary>
        /// Creates a validation result that indicates the build operation is invalid for the specified failure reason.
        /// </summary>
        /// <param name="failureReason">The reason why the build operation is considered invalid.</param>
        /// <returns>A <see cref="CellValidationResult"/> representing an invalid build operation with the specified failure
        /// reason.</returns>
        internal static CellValidationResult Invalid(BuildOperationFailureReason failureReason)
        {
            return new(BuildOperationOutcome.Invalid, failureReason);
        }

        #endregion
    }
}