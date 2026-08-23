using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;
using System.Reflection;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal sealed class BuildManagerTests
    {
        #region Fields

        private MapManager _mapManager = null!;
        private BuildManager _buildManager = null!;

        #endregion

        #region Test Data

        public static IEnumerable<TestCaseData> CrossChunkPlacementCases()
        {
            int size = MapChunkMetrics.ChunkSize;

            yield return new TestCaseData(
                new MapCellCoord(size - 1, 0, 0),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(1, 0, 0) })
                .SetName("PositiveX");

            yield return new TestCaseData(
                new MapCellCoord(0, 0, size - 1),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(0, 0, 1) })
                .SetName("PositiveZ");

            yield return new TestCaseData(
                new MapCellCoord(size - 1, 0, size - 1),
                new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(0, 0, 1),
                    new MapCellOffset(1, 0, 1)
                })
                .SetName("PositiveXZCorner");

            yield return new TestCaseData(
                new MapCellCoord(-size, 0, 0),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(-1, 0, 0) })
                .SetName("NegativeX");

            yield return new TestCaseData(
                new MapCellCoord(0, 0, -size),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(0, 0, -1) })
                .SetName("NegativeZ");
        }

        #endregion

        #region Setup

        [SetUp]
        public void Setup()
        {
            _mapManager = new MapManager();
            _buildManager = new BuildManager(_mapManager);
        }

        #endregion

        #region Placement

        [Test]
        public void Evaluate_ValidSingleCellPlacement_IsPureAndPredictsStableCandidateId()
        {
            StructureDefinition definition = SingleCellDefinition();
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForStructure(definition, anchor, FootprintRotation.Deg0);
            BuildIntent intent = BuildIntent.PlaceStructure(definition, anchor, FootprintRotation.Deg0);

            BuildResult first = _buildManager.Evaluate(intent);
            BuildResult second = _buildManager.Evaluate(intent);

            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(first.Structures, Has.Count.EqualTo(1));
            Assert.That(first.Structures[0].Kind, Is.EqualTo(BuildStructureResultKind.Created));
            Assert.That(first.Structures[0].StructureId, Is.EqualTo(new StructureId(1)));
            Assert.That(first.Structures[0].DefinitionId, Is.EqualTo(definition.Id));
            Assert.That(first.Structures[0].Anchor, Is.EqualTo(anchor));
            Assert.That(first.Structures[0].Rotation, Is.EqualTo(FootprintRotation.Deg0));
            Assert.That(first.Structures[0].AffectedCells, Is.EqualTo(new[] { anchor }));
            AssertEquivalentBuildResults(first, second);
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void Execute_ValidSingleCellPlacement_CreatesStructureAndMatchesEvaluatePlan()
        {
            StructureDefinition definition = SingleCellDefinition();
            var anchor = new MapCellCoord(1, 0, 1);
            StoreTerrainForStructure(definition, anchor, FootprintRotation.Deg0);
            BuildIntent intent = BuildIntent.PlaceStructure(definition, anchor, FootprintRotation.Deg0);
            BuildResult evaluation = _buildManager.Evaluate(intent);

            BuildResult execution = _buildManager.Execute(intent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            AssertStructureStored(execution.Structures.Single(), definition);
            AssertStructureReserved(execution.Structures.Single());
        }

        [Test]
        public void Execute_ValidBatchPlacement_AssignsDeterministicUniqueIdsAndDoesNotReuseRemovedIds()
        {
            StructureDefinition definition = SingleCellDefinition();
            var firstAnchor = new MapCellCoord(0, 0, 0);
            var secondAnchor = new MapCellCoord(1, 0, 0);
            StoreTerrainForCoords(new[] { firstAnchor, secondAnchor });
            BuildIntent batchIntent = BuildIntent.PlaceStructures(new[]
            {
                Placement(definition, firstAnchor),
                Placement(definition, secondAnchor)
            });

            BuildResult batchResult = _buildManager.Execute(batchIntent);

            Assert.That(batchResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(batchResult.Structures.Select(result => result.StructureId), Is.EqualTo(new[]
            {
                new StructureId(1),
                new StructureId(2)
            }));

            _buildManager.Execute(BuildIntent.RemoveStructureAt(firstAnchor));
            var thirdAnchor = new MapCellCoord(2, 0, 0);
            StoreTerrainForCoords(new[] { thirdAnchor });

            BuildResult thirdResult = _buildManager.Execute(BuildIntent.PlaceStructure(
                definition,
                thirdAnchor,
                FootprintRotation.Deg0));

            Assert.That(thirdResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(thirdResult.Structures.Single().StructureId, Is.EqualTo(new StructureId(3)));
        }

        [Test]
        public void Execute_BatchPlacement_WithLaterConflict_MutatesNothing()
        {
            StructureDefinition definition = SingleCellDefinition();
            var firstAnchor = new MapCellCoord(0, 0, 0);
            var secondAnchor = new MapCellCoord(1, 0, 0);
            var conflictAnchor = new MapCellCoord(2, 0, 0);
            StoreTerrainForCoords(new[] { firstAnchor, secondAnchor, conflictAnchor });
            ReserveFurniture(conflictAnchor, new FurnitureId(9));
            int cellCount = _mapManager.CellCount;
            BuildIntent intent = BuildIntent.PlaceStructures(new[]
            {
                Placement(definition, firstAnchor),
                Placement(definition, secondAnchor),
                Placement(definition, conflictAnchor)
            });

            BuildResult result = _buildManager.Execute(intent);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(conflictAnchor));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(cellCount));
            Assert.That(_mapManager.TryGetCell(firstAnchor, out _), Is.False);
            Assert.That(_mapManager.TryGetCell(secondAnchor, out _), Is.False);
            Assert.That(_mapManager.TryGetCell(conflictAnchor, out var conflictCell), Is.True);
            Assert.That(conflictCell!.FurnitureId, Is.EqualTo(new FurnitureId(9)));
        }

        [Test]
        public void EvaluateAndExecute_IntraBatchOverlap_AreInvalidAndPure()
        {
            StructureDefinition definition = TwoCellDefinition();
            var firstAnchor = new MapCellCoord(0, 0, 0);
            var secondAnchor = new MapCellCoord(1, 0, 0);
            StoreTerrainForCoords(new[]
            {
                new MapCellCoord(0, 0, 0),
                new MapCellCoord(1, 0, 0),
                new MapCellCoord(2, 0, 0)
            });
            BuildIntent intent = BuildIntent.PlaceStructures(new[]
            {
                Placement(definition, firstAnchor),
                Placement(definition, secondAnchor)
            });

            BuildResult evaluation = _buildManager.Evaluate(intent);
            BuildResult execution = _buildManager.Execute(intent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(execution.FailureReason, Is.EqualTo(BuildFailureReason.IntraBatchFootprintOverlap));
            Assert.That(execution.FailedCell, Is.EqualTo(new MapCellCoord(1, 0, 0)));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void EvaluateAndExecute_InvalidMultiCellConflict_DoNotAllocateIdOrPartiallyReserveFootprint()
        {
            StructureDefinition conflictDefinition = ThreeCellDefinition();
            var conflictAnchor = new MapCellCoord(0, 0, 0);
            IReadOnlyList<MapCellCoord> conflictCells = conflictDefinition.Footprint.Resolve(conflictAnchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(conflictCells);
            ReserveFurniture(conflictCells[2], new FurnitureId(7));
            int cellCount = _mapManager.CellCount;
            BuildIntent invalidIntent = BuildIntent.PlaceStructure(conflictDefinition, conflictAnchor, FootprintRotation.Deg0);

            BuildResult evaluation = _buildManager.Evaluate(invalidIntent);
            BuildResult execution = _buildManager.Execute(invalidIntent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(execution.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(cellCount));
            Assert.That(_mapManager.TryGetCell(conflictCells[0], out _), Is.False);
            Assert.That(_mapManager.TryGetCell(conflictCells[1], out _), Is.False);

            StructureDefinition validDefinition = SingleCellDefinition(22);
            var validAnchor = new MapCellCoord(5, 0, 5);
            StoreTerrainForStructure(validDefinition, validAnchor, FootprintRotation.Deg0);

            BuildResult validResult = _buildManager.Execute(BuildIntent.PlaceStructure(
                validDefinition,
                validAnchor,
                FootprintRotation.Deg0));

            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(validResult.Structures.Single().StructureId, Is.EqualTo(new StructureId(1)));
        }

        [Test]
        public void EvaluateAndExecute_OutsideGeneratedWorld_AreInvalidWithoutSparseCells()
        {
            StructureDefinition definition = new(
                new StructureDefinitionId(30),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(99, 0, 0)
                }));
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(new[] { anchor });
            BuildIntent intent = BuildIntent.PlaceStructure(definition, anchor, FootprintRotation.Deg0);

            BuildResult evaluation = _buildManager.Evaluate(intent);
            BuildResult execution = _buildManager.Execute(intent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(execution.FailureReason, Is.EqualTo(BuildFailureReason.OutsideGeneratedWorld));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void EvaluateAndExecute_RotatedMultiCellPlacement_UseCompleteFootprint()
        {
            StructureDefinition definition = LShapeDefinition();
            var anchor = new MapCellCoord(2, 0, 2);
            MapCellCoord[] expected =
            {
                new MapCellCoord(2, 0, 2),
                new MapCellCoord(2, 0, 1),
                new MapCellCoord(3, 0, 1)
            };
            StoreTerrainForCoords(expected);
            BuildIntent intent = BuildIntent.PlaceStructure(definition, anchor, FootprintRotation.Deg90);

            BuildResult evaluation = _buildManager.Evaluate(intent);
            BuildResult execution = _buildManager.Execute(intent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(execution.Structures.Single().AffectedCells, Is.EqualTo(expected));
            AssertStructureReserved(execution.Structures.Single());
        }

        [TestCaseSource(nameof(CrossChunkPlacementCases))]
        public void Execute_CrossChunkPlacement_UsesMapManagerStructureLifecycle(
            MapCellCoord anchor,
            MapCellOffset[] offsets)
        {
            StructureDefinition definition = new(new StructureDefinitionId(40), new MapCellFootprint(offsets));
            StoreTerrainForStructure(definition, anchor, FootprintRotation.Deg0);
            BuildIntent intent = BuildIntent.PlaceStructure(definition, anchor, FootprintRotation.Deg0);

            BuildResult result = _buildManager.Execute(intent);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            AssertStructureStored(result.Structures.Single(), definition);
            AssertStructureReserved(result.Structures.Single());
        }

        [Test]
        public void Execute_ReplansAgainstCurrentStateInsteadOfTrustingPriorEvaluate()
        {
            StructureDefinition definition = SingleCellDefinition();
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForStructure(definition, anchor, FootprintRotation.Deg0);
            BuildIntent intent = BuildIntent.PlaceStructure(definition, anchor, FootprintRotation.Deg0);
            BuildResult evaluation = _buildManager.Evaluate(intent);
            Assert.That(evaluation.Structures.Single().StructureId, Is.EqualTo(new StructureId(1)));

            BuildResult blocker = _buildManager.Execute(BuildIntent.PlaceStructure(
                definition,
                anchor,
                FootprintRotation.Deg0));
            Assert.That(blocker.Structures.Single().StructureId, Is.EqualTo(new StructureId(1)));

            BuildResult staleExecution = _buildManager.Execute(intent);

            Assert.That(staleExecution.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(staleExecution.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            _buildManager.Execute(BuildIntent.RemoveStructureAt(anchor));

            BuildResult retried = _buildManager.Execute(intent);

            Assert.That(retried.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(retried.Structures.Single().StructureId, Is.EqualTo(new StructureId(2)));
        }

        [Test]
        public void EvaluateAndExecute_PlacementBatchOverflow_ReturnsInvalidWithoutConsumingLastRepresentableId()
        {
            SetNextStructureIdValue(_mapManager, int.MaxValue);
            StructureDefinition definition = SingleCellDefinition();
            var firstAnchor = new MapCellCoord(0, 0, 0);
            var secondAnchor = new MapCellCoord(1, 0, 0);
            StoreTerrainForCoords(new[] { firstAnchor, secondAnchor });
            BuildIntent overflowingBatch = BuildIntent.PlaceStructures(new[]
            {
                Placement(definition, firstAnchor),
                Placement(definition, secondAnchor)
            });

            BuildResult evaluation = _buildManager.Evaluate(overflowingBatch);
            BuildResult execution = _buildManager.Execute(overflowingBatch);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(execution.FailureReason, Is.EqualTo(BuildFailureReason.StructureIdAllocationExhausted));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));

            BuildResult finalIdResult = _buildManager.Execute(BuildIntent.PlaceStructure(
                definition,
                firstAnchor,
                FootprintRotation.Deg0));

            Assert.That(finalIdResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(finalIdResult.Structures.Single().StructureId, Is.EqualTo(new StructureId(int.MaxValue)));
        }

        #endregion

        #region Removal

        [Test]
        public void EvaluateAndExecute_RemovalFromNonAnchorOccupiedCell_ResolvesAndRemovesCompleteStructure()
        {
            StructureDefinition definition = LShapeDefinition();
            var anchor = new MapCellCoord(0, 0, 0);
            BuildStructureResult placement = ExecutePlacement(definition, anchor, FootprintRotation.Deg0);
            MapCellCoord target = placement.AffectedCells[1];
            BuildIntent removalIntent = BuildIntent.RemoveStructureAt(target);

            BuildResult evaluation = _buildManager.Evaluate(removalIntent);

            Assert.That(evaluation.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(evaluation.Structures.Single().StructureId, Is.EqualTo(placement.StructureId));
            Assert.That(evaluation.Structures.Single().DefinitionId, Is.EqualTo(definition.Id));
            Assert.That(evaluation.Structures.Single().Anchor, Is.EqualTo(anchor));
            Assert.That(evaluation.Structures.Single().AffectedCells, Is.EqualTo(placement.AffectedCells));
            Assert.That(_mapManager.TryGetStructure(placement.StructureId, out _), Is.True);

            BuildResult execution = _buildManager.Execute(removalIntent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.TryGetStructure(placement.StructureId, out _), Is.False);
            AssertCellsMissing(placement.AffectedCells);
            AssertTerrainStillExists(placement.AffectedCells);
        }

        [TestCaseSource(nameof(CrossChunkPlacementCases))]
        public void Execute_RemovalFromCrossChunkOccupiedCell_RemovesCompleteStructure(
            MapCellCoord anchor,
            MapCellOffset[] offsets)
        {
            StructureDefinition definition = new(new StructureDefinitionId(50), new MapCellFootprint(offsets));
            BuildStructureResult placement = ExecutePlacement(definition, anchor, FootprintRotation.Deg0);
            MapCellCoord target = placement.AffectedCells.Last();

            BuildResult result = _buildManager.Execute(BuildIntent.RemoveStructureAt(target));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures.Single().StructureId, Is.EqualTo(placement.StructureId));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            AssertCellsMissing(placement.AffectedCells);
            AssertTerrainStillExists(placement.AffectedCells);
        }

        [Test]
        public void EvaluateAndExecute_RemovalOnlyEmptyTargets_ReturnsNoOpWithoutMutation()
        {
            var target = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(new[] { target });
            BuildIntent intent = BuildIntent.RemoveStructureAt(target);

            BuildResult evaluation = _buildManager.Evaluate(intent);
            BuildResult execution = _buildManager.Execute(intent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(execution.Structures, Is.Empty);
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void Execute_RemovalBatch_WithEmptyTargets_RemovesResolvedStructures()
        {
            StructureDefinition definition = SingleCellDefinition();
            var occupied = new MapCellCoord(0, 0, 0);
            var emptyBefore = new MapCellCoord(-1, 0, 0);
            var emptyAfter = new MapCellCoord(1, 0, 0);
            StoreTerrainForCoords(new[] { emptyBefore, emptyAfter });
            BuildStructureResult placement = ExecutePlacement(definition, occupied, FootprintRotation.Deg0);

            BuildResult result = _buildManager.Execute(BuildIntent.RemoveStructuresAt(new[]
            {
                emptyBefore,
                occupied,
                emptyAfter
            }));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(1));
            Assert.That(result.Structures.Single().StructureId, Is.EqualTo(placement.StructureId));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
        }

        [Test]
        public void Execute_RemovalBatch_DeduplicatesTargetsByStructureId()
        {
            StructureDefinition definition = FourCellDefinition();
            var anchor = new MapCellCoord(0, 0, 0);
            BuildStructureResult placement = ExecutePlacement(definition, anchor, FootprintRotation.Deg0);

            BuildResult evaluation = _buildManager.Evaluate(BuildIntent.RemoveStructuresAt(placement.AffectedCells));
            BuildResult execution = _buildManager.Execute(BuildIntent.RemoveStructuresAt(placement.AffectedCells));

            Assert.That(evaluation.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(evaluation.Structures, Has.Count.EqualTo(1));
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(execution.Structures, Has.Count.EqualTo(1));
            Assert.That(execution.Structures.Single().StructureId, Is.EqualTo(placement.StructureId));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            AssertCellsMissing(placement.AffectedCells);
        }

        [Test]
        public void Execute_MultiStructureRemovalBatch_WithInconsistentSecondStructure_RemovesNone()
        {
            StructureDefinition definition = TwoCellDefinition();
            BuildStructureResult first = ExecutePlacement(definition, new MapCellCoord(0, 0, 0), FootprintRotation.Deg0);
            BuildStructureResult second = ExecutePlacement(definition, new MapCellCoord(3, 0, 0), FootprintRotation.Deg0);
            BuildStructureResult third = ExecutePlacement(definition, new MapCellCoord(6, 0, 0), FootprintRotation.Deg0);
            ReleaseStructureReservationWithoutSparseCleanup(second.AffectedCells[1], second.StructureId);
            BuildIntent intent = BuildIntent.RemoveStructuresAt(new[]
            {
                first.AffectedCells[0],
                second.AffectedCells[0],
                third.AffectedCells[0]
            });

            BuildResult result = _buildManager.Execute(intent);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.InconsistentReservationState));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(3));
            Assert.That(_mapManager.TryGetStructure(first.StructureId, out _), Is.True);
            Assert.That(_mapManager.TryGetStructure(second.StructureId, out _), Is.True);
            Assert.That(_mapManager.TryGetStructure(third.StructureId, out _), Is.True);
            AssertStructureReserved(first);
            AssertStructureReserved(third);
            Assert.That(_mapManager.TryGetCell(second.AffectedCells[0], out var retainedSecondCell), Is.True);
            Assert.That(retainedSecondCell!.StructureId, Is.EqualTo(second.StructureId));
        }

        #endregion

        #region Helpers

        private BuildStructureResult ExecutePlacement(
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            StoreTerrainForStructure(definition, anchor, rotation);
            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(definition, anchor, rotation));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            return result.Structures.Single();
        }

        private StructurePlacementRequest Placement(
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation = FootprintRotation.Deg0)
        {
            return new StructurePlacementRequest(definition, anchor, rotation);
        }

        private StructureDefinition SingleCellDefinition(int id = 10)
        {
            return new StructureDefinition(
                new StructureDefinitionId(id),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0)
                }));
        }

        private StructureDefinition TwoCellDefinition(int id = 11)
        {
            return new StructureDefinition(
                new StructureDefinitionId(id),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0)
                }));
        }

        private StructureDefinition ThreeCellDefinition()
        {
            return new StructureDefinition(
                new StructureDefinitionId(12),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 0, 1)
                }));
        }

        private StructureDefinition FourCellDefinition()
        {
            return new StructureDefinition(
                new StructureDefinitionId(13),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(0, 0, 1),
                    new MapCellOffset(1, 0, 1)
                }));
        }

        private StructureDefinition LShapeDefinition()
        {
            return new StructureDefinition(
                new StructureDefinitionId(14),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 0, 1)
                }));
        }

        private void StoreTerrainForStructure(
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            StoreTerrainForCoords(definition.Footprint.Resolve(anchor, rotation));
        }

        private void StoreTerrainForCoords(IEnumerable<MapCellCoord> coords)
        {
            var terrainCoords = new HashSet<TerrainTileWorldCoord>();

            foreach (MapCellCoord coord in coords)
            {
                var terrainCoord = new TerrainTileWorldCoord(coord.X, coord.Z);

                if (terrainCoords.Add(terrainCoord))
                {
                    _mapManager.StoreGeneratedTerrain(terrainCoord, FlatTile());
                }
            }
        }

        private void ReserveFurniture(MapCellCoord coord, FurnitureId furnitureId)
        {
            Cell cell = _mapManager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveFurniture(furnitureId);
            Assert.That(validation.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            cell.ReserveFurniture(validation, furnitureId);
        }

        private void ReleaseStructureReservationWithoutSparseCleanup(MapCellCoord coord, StructureId structureId)
        {
            Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True);
            CellOccupancyValidationResult validation = cell!.ValidateReleaseStructure(structureId);
            Assert.That(validation.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            cell.ReleaseStructure(validation, structureId);
        }

        private TerrainTile FlatTile()
        {
            return new TerrainTile(0f, 0f, 0f, 0f);
        }

        private void AssertStructureStored(BuildStructureResult result, StructureDefinition definition)
        {
            Assert.That(_mapManager.TryGetStructure(result.StructureId, out var structure), Is.True);
            Assert.That(structure!.Id, Is.EqualTo(result.StructureId));
            Assert.That(structure.Definition, Is.SameAs(definition));
            Assert.That(structure.Anchor, Is.EqualTo(result.Anchor));
            Assert.That(structure.Rotation, Is.EqualTo(result.Rotation));
            Assert.That(structure.ResolveOccupiedCells(), Is.EqualTo(result.AffectedCells));
        }

        private void AssertStructureReserved(BuildStructureResult result)
        {
            foreach (MapCellCoord coord in result.AffectedCells)
            {
                Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
                Assert.That(cell!.StructureId, Is.EqualTo(result.StructureId), coord.ToString());
            }
        }

        private void AssertCellsMissing(IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(_mapManager.TryGetCell(coord, out _), Is.False, coord.ToString());
            }
        }

        private void AssertTerrainStillExists(IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(_mapManager.TryGetTerrain(new TerrainTileWorldCoord(coord.X, coord.Z), out _), Is.True, coord.ToString());
            }
        }

        private void AssertEquivalentBuildResults(BuildResult expected, BuildResult actual)
        {
            Assert.That(actual.Outcome, Is.EqualTo(expected.Outcome));
            Assert.That(actual.FailureReason, Is.EqualTo(expected.FailureReason));
            Assert.That(actual.FailedCell, Is.EqualTo(expected.FailedCell));
            Assert.That(actual.FailedDefinitionId, Is.EqualTo(expected.FailedDefinitionId));
            Assert.That(actual.Structures, Has.Count.EqualTo(expected.Structures.Count));

            for (int i = 0; i < expected.Structures.Count; i++)
            {
                Assert.That(actual.Structures[i].Kind, Is.EqualTo(expected.Structures[i].Kind));
                Assert.That(actual.Structures[i].Outcome, Is.EqualTo(expected.Structures[i].Outcome));
                Assert.That(actual.Structures[i].StructureId, Is.EqualTo(expected.Structures[i].StructureId));
                Assert.That(actual.Structures[i].DefinitionId, Is.EqualTo(expected.Structures[i].DefinitionId));
                Assert.That(actual.Structures[i].Anchor, Is.EqualTo(expected.Structures[i].Anchor));
                Assert.That(actual.Structures[i].Rotation, Is.EqualTo(expected.Structures[i].Rotation));
                Assert.That(actual.Structures[i].AffectedCells, Is.EqualTo(expected.Structures[i].AffectedCells));
            }
        }

        private void SetNextStructureIdValue(MapManager manager, long nextValue)
        {
            FieldInfo? field = typeof(MapManager).GetField(
                "_nextStructureIdValue",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field!.SetValue(manager, nextValue);
        }

        #endregion
    }
}
