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
                    return TryPlaceFloor(coord);

                case BuildOperation.Remove:
                    return TryRemoveFloor(coord);

                default:
                    throw new NotImplementedException($"{nameof(BuildOperation)} not implemented");
            }
        }

        /// <summary>
        /// Attempts to place a new floor at the specified cell without
        /// replacing any existing floor.
        /// </summary>
        /// <param name="coord">
        /// The coordinate of the target cell.
        /// </param>
        /// <returns>
        /// A <see cref="BuildOperationResult"/> describing the outcome
        /// of the placement attempt.
        /// </returns>
        private BuildOperationResult TryPlaceFloor(MapCellCoord coord)
        {
            // TODO: Calculate cost to place said floorType
            // TODO: Check if can afford cost
            // If Yes :
            MapOperationResult mapResult = _mapManager.TryPlaceFloor(coord);
            // Else BuildOperationResult NOFUNDS

            if (mapResult.Outcome == MapCellOutcome.Placed)
            {
                // Deduct Cost (FloorType Cost)
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
        private BuildOperationResult TryRemoveFloor(MapCellCoord coord)
        {
            MapOperationResult mapResult = _mapManager.TryRemoveFloor(coord);

            if (mapResult.Outcome == MapCellOutcome.Removed)
            {
                // Refund Cost
            }

            return BuildOperationResult.FromMapOperationResult(mapResult);
        }

        #endregion

        #region Wall Methods

        private BuildOperationResult ResolveWall(BuildIntent intent, MapCellCoord coord)
        {
            switch (intent.Operation)
            {
                case BuildOperation.Place:
                    return TryPlaceWall(coord);

                case BuildOperation.Remove:
                    return TryRemoveWall(coord);

                default:
                    throw new NotImplementedException($"{nameof(BuildOperation)} not implemented");
            }
        }

        private BuildOperationResult TryPlaceWall(MapCellCoord coord)
        {
            // TODO: Calculate cost to place said floorType
            // TODO: Check if can afford cost
            // If Yes :
            MapOperationResult mapResult = _mapManager.TryPlaceWall(coord);
            // Else BuildOperationResult NOFUNDS

            if (mapResult.Outcome == MapCellOutcome.Placed)
            {
                // Deduct Cost (FloorType Cost)
            }

            return BuildOperationResult.FromMapOperationResult(mapResult);
        }

        private BuildOperationResult TryRemoveWall(MapCellCoord coord)
        {
            MapOperationResult mapResult = _mapManager.TryRemoveWall(coord);

            if (mapResult.Outcome == MapCellOutcome.Removed)
            {
                // Refund Cost
            }

            return BuildOperationResult.FromMapOperationResult(mapResult);
        }

        #endregion
    }
}
