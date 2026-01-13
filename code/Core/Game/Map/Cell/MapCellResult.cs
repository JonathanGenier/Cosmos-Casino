namespace CosmosCasino.Core.Game.Map.Cell
{
    /// <summary>
    /// Immutable result representing the outcome of a single
    /// operation applied to a map cell.
    /// </summary>
    internal readonly struct MapCellResult
    {
        #region Constructor

        /// <summary>
        /// Initializes a new map cell result with the specified
        /// outcome and failure reason.
        /// </summary>
        /// <param name="outcome">
        /// The resulting outcome of the cell operation.
        /// </param>
        /// <param name="failureReason">
        /// The reason for failure or skip, or <see cref="MapCellFailureReason.None"/> on success.
        /// </param>
        private MapCellResult(
            MapCellOutcome outcome,
            MapCellFailureReason failureReason)
        {
            Outcome = outcome;
            FailureReason = failureReason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The final outcome of the cell operation.
        /// </summary>
        internal MapCellOutcome Outcome { get; }

        /// <summary>
        /// The reason the operation failed or was skipped, if applicable.
        /// </summary>
        internal MapCellFailureReason FailureReason { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a result indicating that the operation
        /// successfully placed a new element in the cell.
        /// </summary>
        /// <returns>
        /// A successful placement result.
        /// </returns>
        internal static MapCellResult Placed()
        {
            return new(MapCellOutcome.Placed, MapCellFailureReason.None);
        }

        /// <summary>
        /// Creates a result indicating that an existing element
        /// was replaced with a new one.
        /// </summary>
        /// <returns>
        /// A successful replacement result.
        /// </returns>
        internal static MapCellResult Replaced()
        {
            return new(MapCellOutcome.Replaced, MapCellFailureReason.None);
        }

        /// <summary>
        /// Creates a result indicating that an element
        /// was successfully removed from the cell.
        /// </summary>
        /// <returns>
        /// A successful removal result.
        /// </returns>
        internal static MapCellResult Removed()
        {
            return new(MapCellOutcome.Removed, MapCellFailureReason.None);
        }

        /// <summary>
        /// Creates a result indicating that the operation failed.
        /// </summary>
        /// <param name="failureReason">
        /// The reason the operation failed.
        /// </param>
        /// <returns>
        /// A failed cell operation result.
        /// </returns>
        internal static MapCellResult Failed(MapCellFailureReason failureReason)
        {
            return new(MapCellOutcome.Failed, failureReason);
        }

        /// <summary>
        /// Creates a result indicating that the operation
        /// was intentionally skipped.
        /// </summary>
        /// <param name="failureReason">
        /// The reason the operation was skipped.
        /// </param>
        /// <returns>
        /// A skipped cell operation result.
        /// </returns>
        internal static MapCellResult Skipped(MapCellFailureReason failureReason)
        {
            return new(MapCellOutcome.Skipped, failureReason);
        }

        #endregion
    }
}
