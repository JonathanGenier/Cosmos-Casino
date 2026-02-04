using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;

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

        private BuildOperationResult ExecuteOperationOnFloor(BuildOperation buildOperation, MapCoord coord)
        {
            return buildOperation switch
            {
                BuildOperation.Place => TryPlaceFloor(coord),
                BuildOperation.Remove => TryRemoveFloor(coord),
                _ => throw new NotImplementedException($"{nameof(buildOperation)} not implemented")
            };
        }

        private BuildOperationResult TryPlaceFloor(MapCoord coord)
        {
            // TODO: Calculate cost to place said floorType
            // TODO: Check if can afford cost
            // If Yes :
            BuildOperationResult actionResult = _mapManager.TryPlace(BuildKind.Floor, coord);

            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Deduct Cost (FloorType Cost)
            }

            // Else BuildOperationResult NOFUNDS

            return actionResult;
        }


        private BuildOperationResult TryRemoveFloor(MapCoord coord)
        {
            BuildOperationResult actionResult = _mapManager.TryRemove(BuildKind.Floor, coord);

            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Refund Cost
            }

            return actionResult;
        }

        #endregion

        #region Wall Methods

        private BuildOperationResult ExecuteOperationOnWall(BuildOperation buildOperation, MapCoord coord)
        {
            return buildOperation switch
            {
                BuildOperation.Place => TryPlaceWall(coord),
                BuildOperation.Remove => TryRemoveWall(coord),
                _ => throw new NotImplementedException($"{nameof(buildOperation)} not implemented")
            };
        }

        private BuildOperationResult TryPlaceWall(MapCoord coord)
        {
            // TODO: Calculate cost to place said floorType
            // TODO: Check if can afford cost
            // If Yes :
            BuildOperationResult actionResult = _mapManager.TryPlace(BuildKind.Wall, coord);

            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Deduct Cost (FloorType Cost)

            }

            // Else BuildOperationResult NOFUNDS

            return actionResult;
        }

        private BuildOperationResult TryRemoveWall(MapCoord coord)
        {
            BuildOperationResult actionResult = _mapManager.TryRemove(BuildKind.Wall, coord);

            if (actionResult.Outcome == BuildOperationOutcome.Valid)
            {
                // Refund Cost
            }

            return actionResult;
        }

        #endregion
    }
}
