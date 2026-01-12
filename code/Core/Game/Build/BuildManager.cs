using CosmosCasino.Core.Game.Floor;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Cell;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Orchestrates the execution of a build intent by applying the
    /// requested build operation to each target cell and aggregating
    /// the per-cell outcomes into a single build result.
    /// </summary>
    public sealed partial class BuildManager
    {
        #region FIELDS

        private readonly MapManager _mapManager;
        // private readonly EconomyManager _economyManager; // later

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="BuildManager"/> bound to the
        /// specified map manager, which is used to apply all build
        /// operations to the authoritative map state.
        /// </summary>
        /// <param name="mapManager">
        /// The map manager responsible for cell creation, mutation,
        /// and cleanup during build operations.
        /// </param>
        internal BuildManager(MapManager mapManager)
        {
            _mapManager = mapManager;
        }

        #endregion

        #region Floor Methods

        /// <summary>
        /// Resolves a floor-related build operation for a single target cell,
        /// dispatching to the appropriate placement, replacement, or removal
        /// logic based on the build intent.
        /// </summary>
        /// <param name="intent">
        /// The build intent describing the requested floor operation.
        /// </param>
        /// <param name="coord">
        /// The coordinate of the cell to process.
        /// </param>
        /// <returns>
        /// A <see cref="BuildOperationResult"/> describing the outcome
        /// of the operation for the specified cell.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the specified build operation is not supported.
        /// </exception>
        private BuildOperationResult ResolveFloor(BuildIntent intent, MapCellCoord coord)
        {
            switch (intent.Operation)
            {
                case BuildOperation.Place:
                    return TryPlaceOrReplaceFloor(coord, intent.FloorType!.Value);

                case BuildOperation.Remove:
                    return ApplyRemoveFloor(coord);

                default:
                    throw new NotImplementedException($"{nameof(BuildOperation)} not implemented");
            }
        }

        /// <summary>
        /// Attempts to place a floor at the specified cell, falling back to
        /// a replacement operation if placement is blocked by an existing
        /// floor of a different type.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <param name="floor">
        /// The floor type to place or replace.
        /// </param>
        /// <returns>
        /// A <see cref="BuildOperationResult"/> representing the final
        /// outcome of the placement or replacement attempt.
        /// </returns>
        private BuildOperationResult TryPlaceOrReplaceFloor(MapCellCoord coord, FloorType floor)
        {
            var mapResult = TryPlaceFloor(coord, floor);

            if (mapResult.Outcome == BuildOperationOutcome.Failed
                && mapResult.FailureReason == BuildOperationFailureReason.Blocked)
            {
                return TryReplaceFloor(coord, floor);
            }

            return mapResult;
        }

        /// <summary>
        /// Attempts to place a new floor at the specified cell without
        /// replacing any existing floor.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <param name="floor">
        /// The floor type to place.
        /// </param>
        /// <returns>
        /// A <see cref="BuildOperationResult"/> describing the outcome
        /// of the placement attempt.
        /// </returns>
        private BuildOperationResult TryPlaceFloor(MapCellCoord coord, FloorType floor)
        {
            // TODO: Calculate cost to place said floorType
            // TODO: Check if can afford cost
            // If Yes :
            MapOperationResult mapResult = _mapManager.TryPlaceFloor(coord, floor);
            // Else BuildOperationResult NOFUNDS

            if (mapResult.Outcome == MapCellOutcome.Placed)
            {
                // Deduct Cost (FloorType Cost)
            }

            return BuildOperationResult.FromMapOperationResult(mapResult);
        }

        /// <summary>
        /// Attempts to replace an existing floor at the specified cell
        /// with a new floor type.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <param name="floor">
        /// The new floor type to apply.
        /// </param>
        /// <returns>
        /// A <see cref="BuildOperationResult"/> describing the outcome
        /// of the replacement attempt.
        /// </returns>
        private BuildOperationResult TryReplaceFloor(MapCellCoord coord, FloorType floor)
        {
            // TODO: Calculate cost to replace said floorType
            // TODO: Check if can afford cost
            // If Yes :
            MapOperationResult mapResult = _mapManager.TryReplaceFloor(coord, floor);
            // Else BuildOperationResult NOFUNDS

            if (mapResult.Outcome == MapCellOutcome.Replaced)
            {
                // Deduct Cost
            }

            return BuildOperationResult.FromMapOperationResult(mapResult);
        }

        /// <summary>
        /// Attempts to remove the floor from the specified cell, triggering
        /// cell cleanup if the removal succeeds and the cell becomes empty.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <returns>
        /// A <see cref="BuildOperationResult"/> describing the outcome
        /// of the removal attempt.
        /// </returns>
        private BuildOperationResult ApplyRemoveFloor(MapCellCoord coord)
        {
            MapOperationResult mapResult = _mapManager.TryRemoveFloor(coord);

            if (mapResult.Outcome == MapCellOutcome.Removed)
            {
                // Refund Cost
            }

            return BuildOperationResult.FromMapOperationResult(mapResult);
        }

        #endregion
    }
}
