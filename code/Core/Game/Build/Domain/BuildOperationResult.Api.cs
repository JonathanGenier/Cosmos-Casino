using CosmosCasino.Core.Game.Map.Cell;

namespace CosmosCasino.Core.Game.Build.Domain
{
    /// <summary>
    /// Represents the result of a build operation for a specific map cell, including its outcome and any associated
    /// failure reason.
    /// </summary>
    /// <remarks>Use this struct to determine whether a build operation was valid, invalid, or resulted in no
    /// action for a given cell. The outcome and failure reason provide details about the operation's success or failure
    /// state.</remarks>
    public readonly struct BuildOperationResult
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the BuildOperationResult class with the specified outcome, failure reason, and
        /// cell location.
        /// </summary>
        /// <param name="outcome">The result of the build operation, indicating success or failure.</param>
        /// <param name="failureReason">The reason for failure if the build operation did not succeed. Ignored if the outcome indicates success.</param>
        /// <param name="cell">The map cell associated with the build operation.</param>
        private BuildOperationResult(BuildOperationOutcome outcome, BuildOperationFailureReason failureReason, MapCellCoord cell)
        {
            Outcome = outcome;
            FailureReason = failureReason;
            Cell = cell;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the outcome of the build operation.
        /// </summary>
        public BuildOperationOutcome Outcome { get; }

        /// <summary>
        /// Gets the reason for the build operation failure.
        /// </summary>
        public BuildOperationFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the coordinates of the map cell associated with this instance.
        /// </summary>
        public MapCellCoord Cell { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a result indicating a valid build operation at the specified cell.
        /// </summary>
        /// <param name="cell">The map cell coordinate where the build operation is considered valid.</param>
        /// <returns>A <see cref="BuildOperationResult"/> representing a successful build operation at the given cell.</returns>
        internal static BuildOperationResult Valid(MapCellCoord cell)
        {
            return new(BuildOperationOutcome.Valid, BuildOperationFailureReason.None, cell);
        }

        /// <summary>
        /// Creates a result indicating that no build operation was performed for the specified cell.
        /// </summary>
        /// <param name="cell">The coordinates of the map cell for which the no-operation result applies.</param>
        /// <returns>A <see cref="BuildOperationResult"/> representing a no-operation outcome for the specified cell.</returns>
        internal static BuildOperationResult NoOp(MapCellCoord cell)
        {
            return new(BuildOperationOutcome.NoOp, BuildOperationFailureReason.None, cell);
        }

        /// <summary>
        /// Creates a result indicating that a build operation is invalid for the specified cell and failure reason.
        /// </summary>
        /// <param name="cell">The coordinates of the map cell where the build operation was attempted.</param>
        /// <param name="reason">The reason why the build operation is considered invalid.</param>
        /// <returns>A <see cref="BuildOperationResult"/> representing an invalid build operation for the specified cell and
        /// reason.</returns>
        internal static BuildOperationResult Invalid(MapCellCoord cell, BuildOperationFailureReason reason)
        {
            return new(BuildOperationOutcome.Invalid, reason, cell);
        }

        /// <summary>
        /// Creates a new BuildOperationResult based on the outcome of a map cell validation and the specified cell
        /// coordinates.
        /// </summary>
        /// <param name="validationResult">The result of validating a map cell, including the outcome and any failure reason.</param>
        /// <param name="cell">The coordinates of the map cell associated with the validation result.</param>
        /// <returns>A BuildOperationResult that reflects the outcome and failure reason from the validation result, associated
        /// with the specified cell.</returns>
        internal static BuildOperationResult FromMapCellValidationResult(MapCellValidationResult validationResult, MapCellCoord cell)
        {
            return new(validationResult.Outcome, validationResult.FailureReason, cell);
        }

        #endregion
    }
}