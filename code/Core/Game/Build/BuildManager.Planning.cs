using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Provides shared planning and commit behavior for structure build intents.
    /// </summary>
    public sealed partial class BuildManager
    {
        #region Planning

        private BuildPlan CreateValidPlan(
            BuildOperation operation,
            IReadOnlyList<PlannedStructureOperation> operations)
        {
            return new BuildPlan(
                operation,
                BuildOperationOutcome.Valid,
                BuildFailureReason.None,
                null,
                null,
                operations.ToArray());
        }

        private BuildPlan CreateNoOpPlan(BuildOperation operation)
        {
            return new BuildPlan(
                operation,
                BuildOperationOutcome.NoOp,
                BuildFailureReason.None,
                null,
                null,
                Array.Empty<PlannedStructureOperation>());
        }

        private BuildPlan CreateInvalidPlan(
            BuildOperation operation,
            BuildFailureReason failureReason,
            MapCellCoord? failedCell = null,
            StructureDefinitionId? failedDefinitionId = null)
        {
            return new BuildPlan(
                operation,
                BuildOperationOutcome.Invalid,
                failureReason,
                failedCell,
                failedDefinitionId,
                Array.Empty<PlannedStructureOperation>());
        }

        private BuildPlan Plan(BuildIntent intent)
        {
            ArgumentNullException.ThrowIfNull(intent);

            return intent.Operation switch
            {
                BuildOperation.Place => PlanPlacement(intent),
                BuildOperation.Remove => PlanRemoval(intent),
                _ => throw new InvalidOperationException($"Unsupported build operation: {intent.Operation}")
            };
        }

        private BuildPlan PlanPlacement(BuildIntent intent)
        {
            IReadOnlyList<StructurePlacementRequest> requests = intent.PlacementRequests;

            if (!_mapManager.TryPreviewNextStructureIds(requests.Count, out var candidateIds))
            {
                return CreateInvalidPlan(
                    BuildOperation.Place,
                    BuildFailureReason.StructureIdAllocationExhausted);
            }

            var plannedOperations = new List<PlannedStructureOperation>(requests.Count);
            var claimedCells = new HashSet<MapCellCoord>();

            for (int i = 0; i < requests.Count; i++)
            {
                StructurePlacementRequest request = requests[i];
                StructureId structureId = candidateIds[i];
                IReadOnlyList<MapCellCoord> affectedCells;

                try
                {
                    affectedCells = request.Definition.Footprint.Resolve(request.Anchor, request.Rotation);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return CreateInvalidPlan(
                        BuildOperation.Place,
                        BuildFailureReason.FootprintCoordinateOverflow,
                        failedDefinitionId: request.Definition.Id);
                }

                foreach (MapCellCoord cell in affectedCells)
                {
                    if (!claimedCells.Add(cell))
                    {
                        return CreateInvalidPlan(
                            BuildOperation.Place,
                            BuildFailureReason.IntraBatchFootprintOverlap,
                            cell,
                            request.Definition.Id);
                    }
                }

                MapCellFootprintTransactionResult validation = _mapManager.ValidateReserveStructureFootprint(
                    request.Anchor,
                    request.Definition.Footprint,
                    request.Rotation,
                    structureId);

                if (validation.Outcome != MapCellFootprintTransactionOutcome.Valid)
                {
                    return CreateInvalidPlan(
                        BuildOperation.Place,
                        ToBuildFailureReason(validation),
                        validation.FailedCoord,
                        request.Definition.Id);
                }

                plannedOperations.Add(new PlannedStructureOperation(
                    BuildStructureResultKind.Created,
                    structureId,
                    request.Definition,
                    request.Anchor,
                    request.Rotation,
                    affectedCells));
            }

            return CreateValidPlan(BuildOperation.Place, plannedOperations);
        }

        private BuildPlan PlanRemoval(BuildIntent intent)
        {
            var plannedOperations = new List<PlannedStructureOperation>();
            var seenStructureIds = new HashSet<StructureId>();

            foreach (StructureRemovalRequest request in intent.RemovalRequests)
            {
                Structure? structure;

                try
                {
                    if (!_mapManager.TryGetStructureAt(request.TargetCell, out structure))
                    {
                        continue;
                    }
                }
                catch (InvalidOperationException)
                {
                    return CreateInvalidPlan(
                        BuildOperation.Remove,
                        BuildFailureReason.StructureStateInconsistent,
                        request.TargetCell);
                }

                if (!seenStructureIds.Add(structure.Id))
                {
                    continue;
                }

                MapCellFootprintTransactionResult validation = _mapManager.ValidateReleaseStructureFootprint(
                    structure.Anchor,
                    structure.Definition.Footprint,
                    structure.Rotation,
                    structure.Id);

                if (validation.Outcome != MapCellFootprintTransactionOutcome.Valid)
                {
                    return CreateInvalidPlan(
                        BuildOperation.Remove,
                        ToBuildFailureReason(validation),
                        validation.FailedCoord ?? request.TargetCell,
                        structure.Definition.Id);
                }

                plannedOperations.Add(new PlannedStructureOperation(
                    BuildStructureResultKind.Removed,
                    structure.Id,
                    structure.Definition,
                    structure.Anchor,
                    structure.Rotation,
                    structure.ResolveOccupiedCells()));
            }

            return plannedOperations.Count == 0
                ? CreateNoOpPlan(BuildOperation.Remove)
                : CreateValidPlan(BuildOperation.Remove, plannedOperations);
        }

        private BuildFailureReason ToBuildFailureReason(MapCellFootprintTransactionResult result)
        {
            return result.FailureReason switch
            {
                MapCellFootprintTransactionFailureReason.CoordinateOverflow => BuildFailureReason.FootprintCoordinateOverflow,
                MapCellFootprintTransactionFailureReason.OutsideGeneratedWorld => BuildFailureReason.OutsideGeneratedWorld,
                MapCellFootprintTransactionFailureReason.OccupancyConflict => BuildFailureReason.OccupancyConflict,
                MapCellFootprintTransactionFailureReason.InconsistentReservationState => BuildFailureReason.InconsistentReservationState,
                _ => BuildFailureReason.StructureStateInconsistent
            };
        }

        #endregion

        #region Commit

        private void Commit(BuildPlan plan)
        {
            switch (plan.Operation)
            {
                case BuildOperation.Place:
                    CommitPlacement(plan);
                    break;

                case BuildOperation.Remove:
                    CommitRemoval(plan);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported build operation: {plan.Operation}");
            }
        }

        private void CommitPlacement(BuildPlan plan)
        {
            StructureId[] ids = plan.Operations.Select(operation => operation.StructureId).ToArray();

            if (!_mapManager.TryConsumeNextStructureIds(ids))
            {
                throw new InvalidOperationException("Cannot commit structure placement: planned structure identities are no longer current.");
            }

            foreach (PlannedStructureOperation operation in plan.Operations)
            {
                StructureOperationResult result = _mapManager.TryCreateStructure(
                    operation.StructureId,
                    operation.Definition,
                    operation.Anchor,
                    operation.Rotation);

                if (result.Outcome != StructureOperationOutcome.Valid)
                {
                    throw new InvalidOperationException("Cannot commit structure placement: authoritative creation failed after valid planning.");
                }
            }
        }

        private void CommitRemoval(BuildPlan plan)
        {
            foreach (PlannedStructureOperation operation in plan.Operations)
            {
                StructureOperationResult result = _mapManager.TryRemoveStructure(operation.StructureId);

                if (result.Outcome != StructureOperationOutcome.Valid)
                {
                    throw new InvalidOperationException("Cannot commit structure removal: authoritative removal failed after valid planning.");
                }
            }
        }

        #endregion

        #region Result Projection

        private BuildResult BuildResultFromPlan(BuildIntent intent, BuildPlan plan)
        {
            BuildStructureResult[] structures = plan.Operations
                .Select(operation => operation.ToResult())
                .ToArray();

            return BuildResult.Done(
                intent,
                plan.Outcome,
                plan.FailureReason,
                plan.FailedCell,
                plan.FailedDefinitionId,
                structures);
        }

        #endregion

        #region Plan Types

        private sealed class BuildPlan
        {
            /// <summary>
            /// Initializes a new immutable build plan.
            /// </summary>
            /// <param name="operation">The planned build operation.</param>
            /// <param name="outcome">The aggregate plan outcome.</param>
            /// <param name="failureReason">The aggregate failure reason.</param>
            /// <param name="failedCell">The first failed cell, when available.</param>
            /// <param name="failedDefinitionId">The failed structure definition identity, when available.</param>
            /// <param name="operations">The planned structure operations in deterministic order.</param>
            internal BuildPlan(
                BuildOperation operation,
                BuildOperationOutcome outcome,
                BuildFailureReason failureReason,
                MapCellCoord? failedCell,
                StructureDefinitionId? failedDefinitionId,
                IReadOnlyList<PlannedStructureOperation> operations)
            {
                Operation = operation;
                Outcome = outcome;
                FailureReason = failureReason;
                FailedCell = failedCell;
                FailedDefinitionId = failedDefinitionId;
                Operations = operations;
            }

            internal BuildOperation Operation { get; }

            internal BuildOperationOutcome Outcome { get; }

            internal BuildFailureReason FailureReason { get; }

            internal MapCellCoord? FailedCell { get; }

            internal StructureDefinitionId? FailedDefinitionId { get; }

            internal IReadOnlyList<PlannedStructureOperation> Operations { get; }
        }

        private sealed class PlannedStructureOperation
        {
            /// <summary>
            /// Initializes a new planned structure operation.
            /// </summary>
            /// <param name="kind">Whether the operation creates or removes a structure.</param>
            /// <param name="structureId">The authoritative structure identity.</param>
            /// <param name="definition">The structure definition.</param>
            /// <param name="anchor">The authoritative structure anchor.</param>
            /// <param name="rotation">The structure footprint rotation.</param>
            /// <param name="affectedCells">The affected cells in deterministic order.</param>
            internal PlannedStructureOperation(
                BuildStructureResultKind kind,
                StructureId structureId,
                StructureDefinition definition,
                MapCellCoord anchor,
                FootprintRotation rotation,
                IReadOnlyList<MapCellCoord> affectedCells)
            {
                Kind = kind;
                StructureId = structureId;
                Definition = definition;
                Anchor = anchor;
                Rotation = rotation;
                AffectedCells = affectedCells.ToArray();
            }

            internal BuildStructureResultKind Kind { get; }

            internal StructureId StructureId { get; }

            internal StructureDefinition Definition { get; }

            internal MapCellCoord Anchor { get; }

            internal FootprintRotation Rotation { get; }

            internal IReadOnlyList<MapCellCoord> AffectedCells { get; }

            internal BuildStructureResult ToResult()
            {
                return new BuildStructureResult(
                    Kind,
                    BuildOperationOutcome.Valid,
                    StructureId,
                    Definition.Id,
                    Anchor,
                    Rotation,
                    AffectedCells);
            }
        }

        #endregion
    }
}
