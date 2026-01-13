using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Cell;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Represents the outcome of applying a single build operation
    /// to a specific map cell after resolving build rules and state.
    /// </summary>
    public readonly struct BuildOperationResult
    {
        #region Constructor

        /// <summary>
        /// Creates a build operation result with the specified cell,
        /// outcome, and failure reason.
        /// </summary>
        /// <param name="cell">
        /// The target cell coordinate the operation was applied to,
        /// or <c>null</c> if no specific cell is associated.
        /// </param>
        /// <param name="outcome">
        /// The high-level outcome of the build operation.
        /// </param>
        /// <param name="failureReason">
        /// The reason for failure, or <see cref="BuildOperationFailureReason.None"/>
        /// if the operation succeeded.
        /// </param>
        internal BuildOperationResult(
            MapCellCoord? cell,
            BuildOperationOutcome outcome,
            BuildOperationFailureReason failureReason)
        {
            Cell = cell;
            Outcome = outcome;
            FailureReason = failureReason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The map cell coordinate targeted by this build operation.
        /// </summary>
        public MapCellCoord? Cell { get; }

        /// <summary>
        /// The resolved outcome of the build operation after
        /// applying all validation and map rules.
        /// </summary>
        public BuildOperationOutcome Outcome { get; }

        /// <summary>
        /// The reason the build operation failed or was blocked,
        /// or <see cref="BuildOperationFailureReason.None"/> if successful.
        /// </summary>
        public BuildOperationFailureReason FailureReason { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a build operation result by translating a low-level
        /// map operation result into a build-level representation.
        /// </summary>
        /// <param name="mapResult">
        /// The result produced by the map layer.
        /// </param>
        /// <returns>
        /// A build operation result representing the translated outcome.
        /// </returns>
        internal static BuildOperationResult FromMapOperationResult(
            MapOperationResult mapResult)
        {
            return new BuildOperationResult(
                mapResult.Cell,
                TranslateOutcomeFromMapOperationResult(mapResult.Outcome),
                TranslateFailureFromMapOperationResult(mapResult.FailureReason));
        }

        /// <summary>
        /// Creates a failed build operation result for the specified cell
        /// and failure reason.
        /// </summary>
        /// <param name="cell">
        /// The cell associated with the failure, if applicable.
        /// </param>
        /// <param name="failureReason">
        /// The reason the build operation failed.
        /// </param>
        /// <returns>
        /// A failed build operation result.
        /// </returns>
        internal static BuildOperationResult Failed(
            MapCellCoord? cell,
            BuildOperationFailureReason failureReason)
        {
            return new BuildOperationResult(
                cell,
                BuildOperationOutcome.Failed,
                failureReason);
        }

        #endregion

        #region Translate From Map Operations

        /// <summary>
        /// Translates a map-level cell outcome into a build-level
        /// operation outcome.
        /// </summary>
        /// <param name="outcome">
        /// The map cell outcome to translate.
        /// </param>
        /// <returns>
        /// The corresponding build operation outcome.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the map cell outcome is not recognized.
        /// </exception>
        private static BuildOperationOutcome TranslateOutcomeFromMapOperationResult(MapCellOutcome outcome)
        {
            return outcome switch
            {
                MapCellOutcome.Placed => BuildOperationOutcome.Placed,
                MapCellOutcome.Replaced => BuildOperationOutcome.Replaced,
                MapCellOutcome.Removed => BuildOperationOutcome.Removed,
                MapCellOutcome.Skipped => BuildOperationOutcome.Skipped,
                MapCellOutcome.Failed => BuildOperationOutcome.Failed,
                _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, $"Unhandled {nameof(MapCellOutcome)}")
            };
        }

        /// <summary>
        /// Translates a map-level failure reason into a build-level
        /// failure reason.
        /// </summary>
        /// <param name="failureReason">
        /// The map cell failure reason to translate.
        /// </param>
        /// <returns>
        /// The corresponding build operation failure reason.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the map cell failure reason is not recognized.
        /// </exception>
        private static BuildOperationFailureReason TranslateFailureFromMapOperationResult(MapCellFailureReason failureReason)
        {
            return failureReason switch
            {
                MapCellFailureReason.None => BuildOperationFailureReason.None,
                MapCellFailureReason.Blocked => BuildOperationFailureReason.Blocked,
                MapCellFailureReason.NoFloor => BuildOperationFailureReason.NoFloor,
                MapCellFailureReason.NoStructure => BuildOperationFailureReason.NoStructure,
                MapCellFailureReason.NoFurniture => BuildOperationFailureReason.NoFurniture,
                MapCellFailureReason.SameType => BuildOperationFailureReason.SameType,
                MapCellFailureReason.NoCell => BuildOperationFailureReason.NoCell,
                MapCellFailureReason.InternalError => BuildOperationFailureReason.InternalError,
                _ => throw new ArgumentOutOfRangeException(nameof(failureReason), failureReason, $"Unhandled {nameof(MapCellFailureReason)}")
            };
        }

        #endregion
    }
}