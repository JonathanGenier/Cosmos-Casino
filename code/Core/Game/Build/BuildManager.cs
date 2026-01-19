using CosmosCasino.Core.Game.Build.Domain;
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

        private BuildOperationResult EvaluateOperationOnFloor(BuildOperation buildOperation, MapCellCoord coord)
        {
            return buildOperation switch
            {
                BuildOperation.Place => _mapManager.CanPlaceFloor(coord),
                BuildOperation.Remove => _mapManager.CanRemoveFloor(coord),
                _ => throw new NotImplementedException($"{nameof(buildOperation)} not implemented")
            };
        }

        private BuildOperationResult ExecuteOperationOnFloor(BuildOperation buildOperation, MapCellCoord coord)
        {
            return buildOperation switch
            {
                BuildOperation.Place => TryPlaceFloor(coord),
                BuildOperation.Remove => TryRemoveFloor(coord),
                _ => throw new NotImplementedException($"{nameof(buildOperation)} not implemented")
            };
        }

        private BuildOperationResult TryPlaceFloor(MapCellCoord coord)
        {
            // TODO: Calculate cost to place said floorType
            // TODO: Check if can afford cost
            // If Yes :
            BuildOperationResult actionResult = _mapManager.TryPlaceFloor(coord);


            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Deduct Cost (FloorType Cost)
            }

            // Else BuildOperationResult NOFUNDS

            return actionResult;
        }


        private BuildOperationResult TryRemoveFloor(MapCellCoord coord)
        {
            BuildOperationResult actionResult = _mapManager.TryRemoveFloor(coord);

            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Refund Cost
            }

            return actionResult;
        }

        #endregion

        #region Wall Methods

        private BuildOperationResult EvaluateOperationOnWall(BuildOperation buildOperation, MapCellCoord coord)
        {
            return buildOperation switch
            {
                BuildOperation.Place => _mapManager.CanPlaceWall(coord),
                BuildOperation.Remove => _mapManager.CanRemoveWall(coord),
                _ => throw new NotImplementedException($"{nameof(buildOperation)} not implemented")
            };
        }

        private BuildOperationResult ExecuteOperationOnWall(BuildOperation buildOperation, MapCellCoord coord)
        {
            return buildOperation switch
            {
                BuildOperation.Place => TryPlaceWall(coord),
                BuildOperation.Remove => TryRemoveWall(coord),
                _ => throw new NotImplementedException($"{nameof(buildOperation)} not implemented")
            };
        }

        private BuildOperationResult TryPlaceWall(MapCellCoord coord)
        {
            // TODO: Calculate cost to place said floorType
            // TODO: Check if can afford cost
            // If Yes :
            BuildOperationResult actionResult = _mapManager.TryPlaceWall(coord);

            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Deduct Cost (FloorType Cost)

            }

            // Else BuildOperationResult NOFUNDS

            return actionResult;
        }

        private BuildOperationResult TryRemoveWall(MapCellCoord coord)
        {
            BuildOperationResult actionResult = _mapManager.TryRemoveWall(coord);

            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Refund Cost
            }

            return actionResult;
        }

        #endregion
    }
}
