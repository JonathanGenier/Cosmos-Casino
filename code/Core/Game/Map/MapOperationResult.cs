using CosmosCasino.Core.Game.Map.Cell;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Immutable result representing the outcome of a map-level
    /// operation applied to a specific cell coordinate.
    /// Acts as a bridge between low-level cell results and
    /// higher-level build operations.
    /// </summary>
    internal readonly struct MapOperationResult
    {
        #region Constructor

        /// <summary>
        /// Initializes a new map operation result with the specified
        /// cell coordinate, outcome, and failure reason.
        /// </summary>
        /// <param name="cell">
        /// The coordinate of the cell the operation was applied to.
        /// </param>
        /// <param name="outcome">
        /// The resulting outcome of the map operation.
        /// </param>
        /// <param name="failureReason">
        /// The reason for failure or skip, or <see cref="MapCellFailureReason.None"/> on success.
        /// </param>
        private MapOperationResult(
            MapCellCoord cell,
            MapCellOutcome outcome,
            MapCellFailureReason failureReason)
        {
            Cell = cell;
            Outcome = outcome;
            FailureReason = failureReason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The coordinate of the cell affected by the operation.
        /// </summary>
        internal MapCellCoord Cell { get; }

        /// <summary>
        /// The final outcome of the map operation.
        /// </summary>
        internal MapCellOutcome Outcome { get; }

        /// <summary>
        /// The reason the operation failed or was skipped, if applicable.
        /// </summary>
        internal MapCellFailureReason FailureReason { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a map operation result from a lower-level
        /// map cell result, preserving the outcome and failure reason
        /// while associating it with a specific cell coordinate.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the affected cell.
        /// </param>
        /// <param name="cellResult">
        /// The result produced by the cell-level operation.
        /// </param>
        /// <returns>
        /// A map operation result representing the cell outcome
        /// at the specified coordinate.
        /// </returns>
        internal static MapOperationResult FromMapCellResult(MapCellCoord coord, MapCellResult cellResult)
        {
            return new(coord, cellResult.Outcome, cellResult.FailureReason);
        }

        /// <summary>
        /// Creates a failed map operation result for the specified
        /// cell coordinate and failure reason.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the affected cell.
        /// </param>
        /// <param name="failureReason">
        /// The reason the operation failed.
        /// </param>
        /// <returns>
        /// A failed map operation result.
        /// </returns>
        internal static MapOperationResult Failed(MapCellCoord coord, MapCellFailureReason failureReason)
        {
            return new(coord, MapCellOutcome.Failed, failureReason);
        }

        /// <summary>
        /// Creates a failed map operation result for the specified
        /// cell coordinate and failure reason.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the affected cell.
        /// </param>
        /// <param name="failureReason">
        /// The reason the operation failed.
        /// </param>
        /// <returns>
        /// A failed map operation result.
        /// </returns>
        internal static MapOperationResult Skipped(MapCellCoord coord, MapCellFailureReason failureReason)
        {
            return new(coord, MapCellOutcome.Skipped, failureReason);
        }

        #endregion
    }
}
