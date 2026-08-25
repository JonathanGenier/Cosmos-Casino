using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Orchestrates furniture placement and removal requests against the authoritative map.
    /// </summary>
    public sealed class FurnitureManager
    {
        #region Fields

        private readonly MapManager _mapManager;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new furniture manager bound to the specified map manager.
        /// </summary>
        /// <param name="mapManager">The map manager responsible for authoritative furniture storage and occupancy.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapManager"/> is null.</exception>
        internal FurnitureManager(MapManager mapManager)
        {
            ArgumentNullException.ThrowIfNull(mapManager);

            _mapManager = mapManager;
        }

        #endregion

        #region Evaluation

        /// <summary>
        /// Evaluates the specified furniture placement request without mutating authoritative state.
        /// </summary>
        /// <param name="request">The furniture placement request to evaluate.</param>
        /// <returns>A furniture operation result produced by the shared placement planner.</returns>
        public FurnitureOperationResult EvaluatePlacement(FurniturePlacementRequest request)
        {
            return FurnitureResultFromPlan(PlanPlacement(request));
        }

        /// <summary>
        /// Evaluates the specified furniture removal request without mutating authoritative state.
        /// </summary>
        /// <param name="request">The furniture removal request to evaluate.</param>
        /// <returns>A furniture operation result produced by the shared removal planner.</returns>
        public FurnitureOperationResult EvaluateRemoval(FurnitureRemovalRequest request)
        {
            return FurnitureResultFromPlan(PlanRemoval(request));
        }

        #endregion

        #region Commit

        /// <summary>
        /// Re-evaluates and places the specified furniture against current authoritative map state.
        /// </summary>
        /// <param name="request">The furniture placement request to execute.</param>
        /// <returns>A furniture operation result for the plan evaluated immediately before commit.</returns>
        public FurnitureOperationResult Place(FurniturePlacementRequest request)
        {
            FurniturePlan plan = PlanPlacement(request);

            if (plan.Outcome == FurnitureOperationOutcome.Valid)
            {
                Commit(plan);
            }

            return FurnitureResultFromPlan(plan);
        }

        /// <summary>
        /// Re-evaluates and removes the furniture occupying the specified target cell.
        /// </summary>
        /// <param name="request">The furniture removal request to execute.</param>
        /// <returns>A furniture operation result for the plan evaluated immediately before commit.</returns>
        public FurnitureOperationResult Remove(FurnitureRemovalRequest request)
        {
            FurniturePlan plan = PlanRemoval(request);

            if (plan.Outcome == FurnitureOperationOutcome.Valid)
            {
                Commit(plan);
            }

            return FurnitureResultFromPlan(plan);
        }

        #endregion

        #region Planning

        private static FurniturePlan CreateValidPlan(
            FurnitureOperation operation,
            PlannedFurnitureOperation change)
        {
            return new FurniturePlan(
                operation,
                FurnitureOperationOutcome.Valid,
                FurnitureFailureReason.None,
                null,
                null,
                change);
        }

        private static FurniturePlan CreateNoOpPlan(FurnitureOperation operation)
        {
            return new FurniturePlan(
                operation,
                FurnitureOperationOutcome.NoOp,
                FurnitureFailureReason.None,
                null,
                null,
                null);
        }

        private static FurniturePlan CreateInvalidPlan(
            FurnitureOperation operation,
            FurnitureFailureReason failureReason,
            MapCellCoord? failedCell = null,
            FurnitureDefinitionId? failedDefinitionId = null)
        {
            return new FurniturePlan(
                operation,
                FurnitureOperationOutcome.Invalid,
                failureReason,
                failedCell,
                failedDefinitionId,
                null);
        }

        private FurniturePlan PlanPlacement(FurniturePlacementRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!_mapManager.TryPreviewNextFurnitureIds(1, out var candidateIds))
            {
                return CreateInvalidPlan(
                    FurnitureOperation.Place,
                    FurnitureFailureReason.FurnitureIdAllocationExhausted,
                    failedDefinitionId: request.Definition.Id);
            }

            FurnitureId furnitureId = candidateIds[0];
            IReadOnlyList<MapCellCoord> affectedCells;

            try
            {
                affectedCells = request.Definition.Footprint.Resolve(request.Anchor, request.Rotation);
            }
            catch (ArgumentOutOfRangeException)
            {
                return CreateInvalidPlan(
                    FurnitureOperation.Place,
                    FurnitureFailureReason.FootprintCoordinateOverflow,
                    failedDefinitionId: request.Definition.Id);
            }

            MapCellFootprintTransactionResult validation = _mapManager.ValidateReserveFurnitureFootprint(
                request.Anchor,
                request.Definition.Footprint,
                request.Rotation,
                furnitureId);

            if (validation.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return CreateInvalidPlan(
                    FurnitureOperation.Place,
                    ToFurnitureFailureReason(validation),
                    validation.FailedCoord,
                    request.Definition.Id);
            }

            return CreateValidPlan(
                FurnitureOperation.Place,
                new PlannedFurnitureOperation(
                    FurnitureChangeResultKind.Created,
                    furnitureId,
                    request.Definition,
                    request.Anchor,
                    request.Rotation,
                    affectedCells));
        }

        private FurniturePlan PlanRemoval(FurnitureRemovalRequest request)
        {
            Furniture? furniture;

            try
            {
                if (!_mapManager.TryGetFurnitureAt(request.TargetCell, out furniture))
                {
                    return CreateNoOpPlan(FurnitureOperation.Remove);
                }
            }
            catch (InvalidOperationException)
            {
                return CreateInvalidPlan(
                    FurnitureOperation.Remove,
                    FurnitureFailureReason.FurnitureStateInconsistent,
                    request.TargetCell);
            }

            MapCellFootprintTransactionResult validation = _mapManager.ValidateReleaseFurnitureFootprint(
                furniture.Anchor,
                furniture.Definition.Footprint,
                furniture.Rotation,
                furniture.Id);

            if (validation.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return CreateInvalidPlan(
                    FurnitureOperation.Remove,
                    ToFurnitureFailureReason(validation),
                    validation.FailedCoord ?? request.TargetCell,
                    furniture.Definition.Id);
            }

            return CreateValidPlan(
                FurnitureOperation.Remove,
                new PlannedFurnitureOperation(
                    FurnitureChangeResultKind.Removed,
                    furniture.Id,
                    furniture.Definition,
                    furniture.Anchor,
                    furniture.Rotation,
                    furniture.ResolveOccupiedCells()));
        }

        private FurnitureFailureReason ToFurnitureFailureReason(MapCellFootprintTransactionResult result)
        {
            return result.FailureReason switch
            {
                MapCellFootprintTransactionFailureReason.CoordinateOverflow => FurnitureFailureReason.FootprintCoordinateOverflow,
                MapCellFootprintTransactionFailureReason.OutsideGeneratedWorld => FurnitureFailureReason.OutsideGeneratedWorld,
                MapCellFootprintTransactionFailureReason.OccupancyConflict => ToOccupancyFailureReason(result.OccupancyFailureReason),
                MapCellFootprintTransactionFailureReason.InconsistentReservationState => FurnitureFailureReason.InconsistentReservationState,
                _ => FurnitureFailureReason.FurnitureStateInconsistent
            };
        }

        private FurnitureFailureReason ToOccupancyFailureReason(CellOccupancyFailureReason failureReason)
        {
            return failureReason switch
            {
                CellOccupancyFailureReason.StructurePresent => FurnitureFailureReason.StructurePresent,
                CellOccupancyFailureReason.FurniturePresent => FurnitureFailureReason.FurniturePresent,
                _ => FurnitureFailureReason.OccupancyConflict
            };
        }

        #endregion

        #region Commit

        private void Commit(FurniturePlan plan)
        {
            if (plan.Change == null)
            {
                throw new InvalidOperationException("Cannot commit furniture plan without a furniture change.");
            }

            switch (plan.Operation)
            {
                case FurnitureOperation.Place:
                    CommitPlacement(plan.Change);
                    break;

                case FurnitureOperation.Remove:
                    CommitRemoval(plan.Change);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported furniture operation: {plan.Operation}");
            }
        }

        private void CommitPlacement(PlannedFurnitureOperation change)
        {
            if (!_mapManager.TryConsumeNextFurnitureIds(new[] { change.FurnitureId }))
            {
                throw new InvalidOperationException("Cannot commit furniture placement: planned furniture identity is no longer current.");
            }

            FurnitureStorageOperationResult result = _mapManager.TryCreateFurniture(
                change.FurnitureId,
                change.Definition,
                change.Anchor,
                change.Rotation);

            if (result.Outcome != FurnitureStorageOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot commit furniture placement: authoritative creation failed after valid planning.");
            }
        }

        private void CommitRemoval(PlannedFurnitureOperation change)
        {
            FurnitureStorageOperationResult result = _mapManager.TryRemoveFurniture(change.FurnitureId);

            if (result.Outcome != FurnitureStorageOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot commit furniture removal: authoritative removal failed after valid planning.");
            }
        }

        #endregion

        #region Result Projection

        private FurnitureOperationResult FurnitureResultFromPlan(FurniturePlan plan)
        {
            FurnitureChangeResult[] changes = plan.Change == null
                ? Array.Empty<FurnitureChangeResult>()
                : new[] { plan.Change.ToResult() };

            return FurnitureOperationResult.Done(
                plan.Operation,
                plan.Outcome,
                plan.FailureReason,
                plan.FailedCell,
                plan.FailedDefinitionId,
                changes);
        }

        #endregion

        #region Plan Types

        private sealed class FurniturePlan
        {
            /// <summary>
            /// Initializes a new immutable furniture plan.
            /// </summary>
            /// <param name="operation">The planned furniture operation.</param>
            /// <param name="outcome">The aggregate plan outcome.</param>
            /// <param name="failureReason">The aggregate failure reason.</param>
            /// <param name="failedCell">The first failed cell, when available.</param>
            /// <param name="failedDefinitionId">The failed furniture definition identity, when available.</param>
            /// <param name="change">The planned furniture change, if any.</param>
            internal FurniturePlan(
                FurnitureOperation operation,
                FurnitureOperationOutcome outcome,
                FurnitureFailureReason failureReason,
                MapCellCoord? failedCell,
                FurnitureDefinitionId? failedDefinitionId,
                PlannedFurnitureOperation? change)
            {
                Operation = operation;
                Outcome = outcome;
                FailureReason = failureReason;
                FailedCell = failedCell;
                FailedDefinitionId = failedDefinitionId;
                Change = change;
            }

            internal FurnitureOperation Operation { get; }

            internal FurnitureOperationOutcome Outcome { get; }

            internal FurnitureFailureReason FailureReason { get; }

            internal MapCellCoord? FailedCell { get; }

            internal FurnitureDefinitionId? FailedDefinitionId { get; }

            internal PlannedFurnitureOperation? Change { get; }
        }

        private sealed class PlannedFurnitureOperation
        {
            /// <summary>
            /// Initializes a new planned furniture operation.
            /// </summary>
            /// <param name="kind">Whether the operation creates or removes furniture.</param>
            /// <param name="furnitureId">The authoritative furniture identity.</param>
            /// <param name="definition">The furniture definition.</param>
            /// <param name="anchor">The authoritative furniture anchor.</param>
            /// <param name="rotation">The furniture footprint rotation.</param>
            /// <param name="affectedCells">The affected cells in deterministic order.</param>
            internal PlannedFurnitureOperation(
                FurnitureChangeResultKind kind,
                FurnitureId furnitureId,
                FurnitureDefinition definition,
                MapCellCoord anchor,
                FootprintRotation rotation,
                IReadOnlyList<MapCellCoord> affectedCells)
            {
                ArgumentNullException.ThrowIfNull(definition);
                ArgumentNullException.ThrowIfNull(affectedCells);

                Kind = kind;
                FurnitureId = furnitureId;
                Definition = definition;
                Anchor = anchor;
                Rotation = rotation;
                AffectedCells = affectedCells.ToArray();
            }

            internal FurnitureChangeResultKind Kind { get; }

            internal FurnitureId FurnitureId { get; }

            internal FurnitureDefinition Definition { get; }

            internal MapCellCoord Anchor { get; }

            internal FootprintRotation Rotation { get; }

            internal IReadOnlyList<MapCellCoord> AffectedCells { get; }

            internal FurnitureChangeResult ToResult()
            {
                return new FurnitureChangeResult(
                    Kind,
                    FurnitureId,
                    Definition.Id,
                    Anchor,
                    Rotation,
                    AffectedCells);
            }
        }

        #endregion
    }
}
