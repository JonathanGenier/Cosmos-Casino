using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapManagerStructureTests
    {
        #region Creation

        [Test]
        public void TryCreateStructure_SingleCellStructure_StoresAggregateAndReservesCell()
        {
            var manager = new MapManager();
            StructureDefinition definition = SingleCellDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.StructureCount, Is.EqualTo(1));
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            Assert.That(structure!.Id, Is.EqualTo(id));
            Assert.That(structure.Definition, Is.SameAs(definition));
            AssertStructureCells(manager, structure);
        }

        [Test]
        public void TryCreateStructure_MultiCellStructure_StoresOneAggregateAndEveryCellReferencesSameId()
        {
            var manager = new MapManager();
            StructureDefinition definition = IrregularDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(2, 1, -2);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.StructureCount, Is.EqualTo(1));
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            Assert.That(structure!.ResolveOccupiedCells(), Has.Count.EqualTo(3));
            AssertStructureCells(manager, structure);
        }

        [Test]
        public void TryCreateStructure_VerticalStructure_UsesCallerSelectedYWithoutTerrainSnapping()
        {
            var manager = new MapManager();
            StructureDefinition definition = new StructureDefinition(
                new StructureDefinitionId(1),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, -2, 0),
                    new MapCellOffset(0, 2, 0)
                }));
            var id = new StructureId(1);
            var anchor = new MapCellCoord(0, 5, 0);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            Assert.That(structure!.Anchor, Is.EqualTo(anchor));
            Assert.That(
                structure.ResolveOccupiedCells(),
                Is.EqualTo(new[]
                {
                    new MapCellCoord(0, 3, 0),
                    new MapCellCoord(0, 7, 0)
                }));
            Assert.That(manager.TryGetCell(new MapCellCoord(0, 0, 0), out _), Is.False);
        }

        [Test]
        public void TryCreateStructure_DuplicateStructureId_ReturnsInvalidWithoutMutatingOriginalOrCells()
        {
            var manager = new MapManager();
            StructureDefinition originalDefinition = SingleCellDefinition();
            StructureDefinition duplicateDefinition = SingleCellDefinition(2);
            var id = new StructureId(1);
            var originalAnchor = new MapCellCoord(0, 0, 0);
            var duplicateAnchor = new MapCellCoord(1, 0, 0);
            StoreTerrainForStructure(manager, originalDefinition, originalAnchor, FootprintRotation.Deg0);
            StoreTerrainForStructure(manager, duplicateDefinition, duplicateAnchor, FootprintRotation.Deg0);
            manager.TryCreateStructure(id, originalDefinition, originalAnchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryCreateStructure(id, duplicateDefinition, duplicateAnchor, FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(StructureOperationFailureReason.StructureIdAlreadyExists));
            Assert.That(result.FootprintResult, Is.Null);
            Assert.That(manager.StructureCount, Is.EqualTo(1));
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            Assert.That(structure!.Definition, Is.SameAs(originalDefinition));
            Assert.That(structure.Anchor, Is.EqualTo(originalAnchor));
            Assert.That(manager.TryGetCell(duplicateAnchor, out _), Is.False);
        }

        [Test]
        public void TryCreateStructure_FootprintConflict_ReturnsInvalidWithoutStoringOrPartialReservation()
        {
            var manager = new MapManager();
            StructureDefinition definition = TwoCellDefinition();
            var anchor = new MapCellCoord(0, 0, 0);
            IReadOnlyList<MapCellCoord> coords = definition.Footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveFurniture(manager, coords[1], new FurnitureId(7));

            StructureOperationResult result = manager.TryCreateStructure(
                new StructureId(1),
                definition,
                anchor,
                FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(StructureOperationFailureReason.FootprintReservationFailed));
            Assert.That(result.FootprintResult, Is.Not.Null);
            Assert.That(result.FootprintResult!.Value.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(manager.StructureCount, Is.EqualTo(0));
            Assert.That(manager.TryGetStructure(new StructureId(1), out _), Is.False);
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            Assert.That(manager.TryGetCell(coords[1], out var conflictCell), Is.True);
            Assert.That(conflictCell!.FurnitureId, Is.EqualTo(new FurnitureId(7)));
        }

        [Test]
        public void TryCreateStructure_OutsideGeneratedWorld_ReturnsInvalidWithoutStoringOrPartialReservation()
        {
            var manager = new MapManager();
            StructureDefinition definition = new StructureDefinition(
                new StructureDefinitionId(1),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(99, 0, 0)
                }));
            var anchor = new MapCellCoord(0, 0, 0);
            manager.GenerateMap(seed: 0, mapSize: 5);
            int chunkCount = manager.ChunkCount;

            StructureOperationResult result = manager.TryCreateStructure(
                new StructureId(1),
                definition,
                anchor,
                FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(StructureOperationFailureReason.FootprintReservationFailed));
            Assert.That(result.FootprintResult, Is.Not.Null);
            Assert.That(result.FootprintResult!.Value.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OutsideGeneratedWorld));
            Assert.That(manager.StructureCount, Is.EqualTo(0));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
            Assert.That(manager.TryGetCell(anchor, out _), Is.False);
        }

        #endregion

        #region Lookup

        [Test]
        public void TryGetStructure_MissingId_ReturnsFalse()
        {
            var manager = new MapManager();

            bool found = manager.TryGetStructure(new StructureId(999), out var structure);

            Assert.That(found, Is.False);
            Assert.That(structure, Is.Null);
        }

        [Test]
        public void TryGetStructureAt_MissingOrEmptyCell_ReturnsFalse()
        {
            var manager = new MapManager();
            StoreTerrainForCoords(manager, new[] { new MapCellCoord(0, 0, 0) });
            Cell emptyCell = manager.GetOrCreateCell(new MapCellCoord(0, 0, 0));

            bool missingFound = manager.TryGetStructureAt(new MapCellCoord(99, 0, 0), out var missingStructure);
            bool emptyFound = manager.TryGetStructureAt(emptyCell.Coord, out var emptyStructure);

            Assert.That(missingFound, Is.False);
            Assert.That(missingStructure, Is.Null);
            Assert.That(emptyFound, Is.False);
            Assert.That(emptyStructure, Is.Null);
        }

        [Test]
        public void TryGetStructureAt_EveryOccupiedCell_ReturnsSameAuthoritativeStructure()
        {
            var manager = new MapManager();
            StructureDefinition definition = IrregularDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(-2, 1, 3);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg270);
            manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg270);
            Assert.That(manager.TryGetStructure(id, out var storedStructure), Is.True);

            foreach (MapCellCoord coord in storedStructure!.ResolveOccupiedCells())
            {
                Assert.That(manager.TryGetStructureAt(coord, out var structureAtCell), Is.True, coord.ToString());
                Assert.That(structureAtCell, Is.SameAs(storedStructure), coord.ToString());
                Assert.That(structureAtCell!.Id, Is.EqualTo(id), coord.ToString());
                Assert.That(structureAtCell.Definition, Is.SameAs(definition), coord.ToString());
                Assert.That(structureAtCell.Anchor, Is.EqualTo(anchor), coord.ToString());
                Assert.That(structureAtCell.Rotation, Is.EqualTo(FootprintRotation.Deg270), coord.ToString());
            }
        }

        [Test]
        public void TryGetStructureAt_CellReferencesMissingStructure_ThrowsInvalidOperationException()
        {
            var manager = new MapManager();
            var coord = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(manager, new[] { coord });
            ReserveStructure(manager, coord, new StructureId(404));

            Assert.Throws<InvalidOperationException>(() =>
                manager.TryGetStructureAt(coord, out _));
        }

        [Test]
        public void TryGetStructureAt_CellWithFurnitureOnly_ReturnsFalse()
        {
            var manager = new MapManager();
            var coord = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(manager, new[] { coord });
            ReserveFurniture(manager, coord, new FurnitureId(1));

            bool found = manager.TryGetStructureAt(coord, out var structure);

            Assert.That(found, Is.False);
            Assert.That(structure, Is.Null);
        }

        #endregion

        #region Rotation

        [TestCaseSource(nameof(RotationCases))]
        public void TryCreateStructure_AsymmetricRotations_ReserveExpectedCells(
            FootprintRotation rotation,
            MapCellCoord[] expectedCoords)
        {
            var manager = new MapManager();
            StructureDefinition definition = LShapeDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(2, 0, 2);
            StoreTerrainForCoords(manager, expectedCoords);

            StructureOperationResult result = manager.TryCreateStructure(id, definition, anchor, rotation);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            Assert.That(structure!.Rotation, Is.EqualTo(rotation));
            Assert.That(structure.ResolveOccupiedCells(), Is.EqualTo(expectedCoords));
            AssertStructureCells(manager, structure);
        }

        #endregion

        #region Removal

        [Test]
        public void TryRemoveStructure_SingleCellStructure_ReleasesCellAndRemovesAggregate()
        {
            var manager = new MapManager();
            StructureDefinition definition = SingleCellDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);
            manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryRemoveStructure(id);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.StructureCount, Is.EqualTo(0));
            Assert.That(manager.TryGetStructure(id, out _), Is.False);
            Assert.That(manager.TryGetCell(anchor, out _), Is.False);
            Assert.That(manager.TryGetTerrain(new TerrainTileWorldCoord(anchor.X, anchor.Z), out _), Is.True);
        }

        [Test]
        public void TryRemoveStructure_MultiCellStructure_ReleasesEveryCellAndRemovesOneAggregate()
        {
            var manager = new MapManager();
            StructureDefinition definition = IrregularDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);
            manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            IReadOnlyList<MapCellCoord> occupied = structure!.ResolveOccupiedCells();

            StructureOperationResult result = manager.TryRemoveStructure(id);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.StructureCount, Is.EqualTo(0));
            AssertCellsMissing(manager, occupied);
        }

        [Test]
        public void TryRemoveStructure_CrossChunkStructure_ReleasesEveryChunkAndRemovesAggregate()
        {
            int size = MapChunkMetrics.ChunkSize;
            var manager = new MapManager();
            StructureDefinition definition = TwoCellDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(size - 1, 0, 0);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);
            manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            IReadOnlyList<MapCellCoord> occupied = structure!.ResolveOccupiedCells();

            StructureOperationResult result = manager.TryRemoveStructure(id);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.StructureCount, Is.EqualTo(0));
            AssertCellsMissing(manager, occupied);
            AssertTerrainStillExists(manager, occupied);
        }

        [Test]
        public void TryRemoveStructure_MissingStructure_ReturnsNoOp()
        {
            var manager = new MapManager();

            StructureOperationResult result = manager.TryRemoveStructure(new StructureId(999));

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(StructureOperationFailureReason.None));
            Assert.That(manager.StructureCount, Is.EqualTo(0));
            Assert.That(manager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveStructure_InconsistentFootprintReservation_ReturnsInvalidWithoutRemovingAggregate()
        {
            var manager = new MapManager();
            StructureDefinition definition = TwoCellDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(0, 0, 0);
            IReadOnlyList<MapCellCoord> coords = definition.Footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);
            ReleaseStructureReservationWithoutSparseCleanup(manager, coords[1], id);

            StructureOperationResult result = manager.TryRemoveStructure(id);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(StructureOperationFailureReason.InconsistentState));
            Assert.That(result.FootprintResult, Is.Not.Null);
            Assert.That(result.FootprintResult!.Value.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.InconsistentReservationState));
            Assert.That(manager.StructureCount, Is.EqualTo(1));
            Assert.That(manager.TryGetStructure(id, out _), Is.True);
            Assert.That(manager.TryGetCell(coords[0], out var retainedCell), Is.True);
            Assert.That(retainedCell!.StructureId, Is.EqualTo(id));
            Assert.That(manager.TryGetCell(coords[1], out var corruptedCell), Is.True);
            Assert.That(corruptedCell!.StructureId, Is.Null);
        }

        [Test]
        public void TryRemoveStructure_CellStillHasFloor_RemainsStoredAfterStructureRelease()
        {
            var manager = new MapManager();
            StructureDefinition definition = SingleCellDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);
            manager.TryPlace(BuildKind.Floor, new MapCoord(anchor.X, anchor.Z), new Elevation(0));
            manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryRemoveStructure(id);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.StructureCount, Is.EqualTo(0));
            Assert.That(manager.TryGetCell(anchor, out var cell), Is.True);
            Assert.That(cell!.StructureId, Is.Null);
            Assert.That(cell.HasFloor(), Is.True);
        }

        #endregion

        #region Cross Chunk

        [TestCaseSource(nameof(CrossChunkCases))]
        public void TryCreateStructure_CrossChunkStructure_StoresOneAggregateAndLookupWorksFromEveryChunk(
            MapCellCoord anchor,
            MapCellOffset[] offsets)
        {
            var manager = new MapManager();
            StructureDefinition definition = new StructureDefinition(new StructureDefinitionId(1), new MapCellFootprint(offsets));
            var id = new StructureId(1);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.StructureCount, Is.EqualTo(1));
            Assert.That(manager.TryGetStructure(id, out var storedStructure), Is.True);

            foreach (MapCellCoord coord in storedStructure!.ResolveOccupiedCells())
            {
                Assert.That(manager.TryGetStructureAt(coord, out var structureAtCell), Is.True, coord.ToString());
                Assert.That(structureAtCell, Is.SameAs(storedStructure), coord.ToString());
                Assert.That(structureAtCell!.Definition, Is.SameAs(definition), coord.ToString());
                Assert.That(structureAtCell.Anchor, Is.EqualTo(anchor), coord.ToString());
            }
        }

        [Test]
        public void TryCreateStructure_NegativeAnchorAcrossChunkBoundary_UsesExistingMapRouting()
        {
            int size = MapChunkMetrics.ChunkSize;
            var manager = new MapManager();
            StructureDefinition definition = TwoCellDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(-size - 1, 0, -1);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg0);

            StructureOperationResult result = manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg0);

            Assert.That(result.Outcome, Is.EqualTo(StructureOperationOutcome.Valid));
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);
            Assert.That(
                structure!.ResolveOccupiedCells().Select(manager.ResolveChunkCoord).ToArray(),
                Is.EqualTo(new[]
                {
                    new MapChunkCoord(-2, -1),
                    new MapChunkCoord(-1, -1)
                }));
            AssertStructureCells(manager, structure);
        }

        #endregion

        #region Determinism

        [Test]
        public void StructureQueries_RepeatedCalls_ReturnSameStateAndCoordinateOrder()
        {
            var manager = new MapManager();
            StructureDefinition definition = IrregularDefinition();
            var id = new StructureId(1);
            var anchor = new MapCellCoord(-3, 1, 4);
            StoreTerrainForStructure(manager, definition, anchor, FootprintRotation.Deg90);
            manager.TryCreateStructure(id, definition, anchor, FootprintRotation.Deg90);
            Assert.That(manager.TryGetStructure(id, out var structure), Is.True);

            IReadOnlyList<MapCellCoord> firstCoords = structure!.ResolveOccupiedCells();
            IReadOnlyList<MapCellCoord> secondCoords = structure.ResolveOccupiedCells();
            bool firstLookup = manager.TryGetStructureAt(firstCoords[0], out var firstStructure);
            bool secondLookup = manager.TryGetStructureAt(firstCoords[0], out var secondStructure);

            Assert.That(secondCoords, Is.EqualTo(firstCoords));
            Assert.That(firstLookup, Is.True);
            Assert.That(secondLookup, Is.True);
            Assert.That(secondStructure, Is.SameAs(firstStructure));
            Assert.That(firstStructure, Is.SameAs(structure));
        }

        #endregion

        #region Helpers

        private static IEnumerable<TestCaseData> CrossChunkCases()
        {
            int size = MapChunkMetrics.ChunkSize;

            yield return new TestCaseData(
                new MapCellCoord(size - 1, 0, 0),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(1, 0, 0) })
                .SetName("PositiveXBoundary");

            yield return new TestCaseData(
                new MapCellCoord(0, 0, size - 1),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(0, 0, 1) })
                .SetName("PositiveZBoundary");

            yield return new TestCaseData(
                new MapCellCoord(size - 1, 0, size - 1),
                new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(0, 0, 1),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 0, 1)
                })
                .SetName("PositiveCornerBoundary");

            yield return new TestCaseData(
                new MapCellCoord(-1, 0, 0),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(1, 0, 0) })
                .SetName("NegativeXBoundary");

            yield return new TestCaseData(
                new MapCellCoord(-1, 0, -1),
                new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(0, 0, 1),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 0, 1)
                })
                .SetName("NegativeCornerBoundary");
        }

        private static IEnumerable<TestCaseData> RotationCases()
        {
            yield return new TestCaseData(
                FootprintRotation.Deg0,
                new[]
                {
                    new MapCellCoord(2, 0, 2),
                    new MapCellCoord(3, 0, 2),
                    new MapCellCoord(3, 0, 3)
                })
                .SetName("Deg0");

            yield return new TestCaseData(
                FootprintRotation.Deg90,
                new[]
                {
                    new MapCellCoord(2, 0, 2),
                    new MapCellCoord(2, 0, 1),
                    new MapCellCoord(3, 0, 1)
                })
                .SetName("Deg90");

            yield return new TestCaseData(
                FootprintRotation.Deg180,
                new[]
                {
                    new MapCellCoord(2, 0, 2),
                    new MapCellCoord(1, 0, 2),
                    new MapCellCoord(1, 0, 1)
                })
                .SetName("Deg180");

            yield return new TestCaseData(
                FootprintRotation.Deg270,
                new[]
                {
                    new MapCellCoord(2, 0, 2),
                    new MapCellCoord(2, 0, 3),
                    new MapCellCoord(1, 0, 3)
                })
                .SetName("Deg270");
        }

        private static StructureDefinition SingleCellDefinition(int definitionId = 1)
        {
            return new StructureDefinition(
                new StructureDefinitionId(definitionId),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0)
                }));
        }

        private static StructureDefinition TwoCellDefinition()
        {
            return new StructureDefinition(
                new StructureDefinitionId(1),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0)
                }));
        }

        private static StructureDefinition LShapeDefinition()
        {
            return new StructureDefinition(
                new StructureDefinitionId(1),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 0, 1)
                }));
        }

        private static StructureDefinition IrregularDefinition()
        {
            return new StructureDefinition(
                new StructureDefinitionId(1),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 1, 1)
                }));
        }

        private static void StoreTerrainForStructure(
            MapManager manager,
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            StoreTerrainForCoords(manager, definition.Footprint.Resolve(anchor, rotation));
        }

        private static void StoreTerrainForCoords(MapManager manager, IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                var terrainCoord = new TerrainTileWorldCoord(coord.X, coord.Z);

                if (!manager.TryGetTerrain(terrainCoord, out _))
                {
                    manager.StoreGeneratedTerrain(terrainCoord, FlatTile());
                }
            }
        }

        private static void ReserveStructure(MapManager manager, MapCellCoord coord, StructureId structureId)
        {
            Cell cell = manager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveStructure(structureId);
            cell.ReserveStructure(validation, structureId);
        }

        private static void ReserveFurniture(MapManager manager, MapCellCoord coord, FurnitureId furnitureId)
        {
            Cell cell = manager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveFurniture(furnitureId);
            cell.ReserveFurniture(validation, furnitureId);
        }

        private static void ReleaseStructureReservationWithoutSparseCleanup(
            MapManager manager,
            MapCellCoord coord,
            StructureId structureId)
        {
            Assert.That(manager.TryGetCell(coord, out var cell), Is.True);
            CellOccupancyValidationResult validation = cell!.ValidateReleaseStructure(structureId);
            cell.ReleaseStructure(validation, structureId);
        }

        private static TerrainTile FlatTile()
        {
            return new TerrainTile(0f, 0f, 0f, 0f);
        }

        private static void AssertStructureCells(MapManager manager, Structure structure)
        {
            foreach (MapCellCoord coord in structure.ResolveOccupiedCells())
            {
                Assert.That(manager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
                Assert.That(cell!.StructureId, Is.EqualTo(structure.Id), coord.ToString());
                Assert.That(manager.TryGetStructureAt(coord, out var structureAtCell), Is.True, coord.ToString());
                Assert.That(structureAtCell, Is.SameAs(structure), coord.ToString());
            }
        }

        private static void AssertCellsMissing(MapManager manager, IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(manager.TryGetCell(coord, out _), Is.False, coord.ToString());
            }
        }

        private static void AssertTerrainStillExists(MapManager manager, IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(manager.TryGetTerrain(new TerrainTileWorldCoord(coord.X, coord.Z), out _), Is.True, coord.ToString());
            }
        }

        #endregion
    }
}
