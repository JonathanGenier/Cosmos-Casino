using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Furniture
{
    [TestFixture]
    internal sealed class FurnitureManagerTests
    {
        #region Fields

        private MapManager _mapManager = null!;
        private BuildManager _buildManager = null!;
        private FurnitureManager _furnitureManager = null!;

        #endregion

        #region Setup

        [SetUp]
        public void Setup()
        {
            _mapManager = new MapManager();
            _buildManager = new BuildManager(_mapManager);
            _furnitureManager = new FurnitureManager(_mapManager);
        }

        #endregion

        #region Placement

        [TestCase(FootprintRotation.Deg0)]
        [TestCase(FootprintRotation.Deg90)]
        [TestCase(FootprintRotation.Deg180)]
        [TestCase(FootprintRotation.Deg270)]
        public void Place_CasinoTable_CreatesOneAuthoritativeFurnitureForCompleteFootprint(
            FootprintRotation rotation)
        {
            var anchor = new MapCellCoord(-4, 0, 9);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, rotation);
            StoreTerrainForCoords(expectedCells);

            FurnitureOperationResult result = _furnitureManager.Place(CasinoTablePlacement(anchor, rotation));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Valid));
            Assert.That(result.Changes, Has.Count.EqualTo(1));
            AssertStoredCasinoTable(result.Changes.Single(), anchor, rotation, expectedCells);
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(1));
            Assert.That(_mapManager.CellCount, Is.EqualTo(expectedCells.Count));
            AssertEveryCasinoTableCellResolvesToFurniture(result.Changes.Single());
        }

        [Test]
        public void FailedPlacement_DoesNotConsumeFurnitureIdentity()
        {
            var invalidAnchor = new MapCellCoord(900, 0, 900);

            FurnitureOperationResult invalid = _furnitureManager.Place(CasinoTablePlacement(invalidAnchor, FootprintRotation.Deg0));

            Assert.That(invalid.Outcome, Is.EqualTo(FurnitureOperationOutcome.Invalid));
            Assert.That(invalid.FailureReason, Is.EqualTo(FurnitureFailureReason.OutsideGeneratedWorld));
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(0));

            var validAnchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(ResolveCasinoTable(validAnchor, FootprintRotation.Deg0));

            FurnitureChangeResult placement = PlaceCasinoTable(validAnchor, FootprintRotation.Deg0);

            Assert.That(placement.FurnitureId, Is.EqualTo(new FurnitureId(1)));
        }

        #endregion

        #region Lookup

        [Test]
        public void TryGetFurnitureSnapshotAt_EveryCasinoTableCell_ReturnsSameAuthoritativeSnapshot()
        {
            var anchor = new MapCellCoord(3, 1, -5);
            const FootprintRotation rotation = FootprintRotation.Deg270;
            StoreTerrainForCoords(ResolveCasinoTable(anchor, rotation));
            FurnitureChangeResult placement = PlaceCasinoTable(anchor, rotation);

            foreach (MapCellCoord cell in placement.AffectedCells)
            {
                Assert.That(_mapManager.TryGetFurnitureIdAt(cell, out FurnitureId furnitureId), Is.True, cell.ToString());
                Assert.That(furnitureId, Is.EqualTo(placement.FurnitureId), cell.ToString());
                Assert.That(_mapManager.TryGetFurnitureSnapshotAt(cell, out FurnitureSnapshot snapshot), Is.True, cell.ToString());
                Assert.That(snapshot.Id, Is.EqualTo(placement.FurnitureId), cell.ToString());
                Assert.That(snapshot.Definition, Is.SameAs(FurnitureDefinitions.CasinoTable), cell.ToString());
                Assert.That(snapshot.Anchor, Is.EqualTo(anchor), cell.ToString());
                Assert.That(snapshot.Rotation, Is.EqualTo(rotation), cell.ToString());
            }
        }

        [Test]
        public void GetFurnitureSnapshots_ReturnsDeterministicIdentityOrder()
        {
            var firstAnchor = new MapCellCoord(10, 0, 10);
            var secondAnchor = new MapCellCoord(20, 0, 20);
            StoreTerrainForCoords(ResolveCasinoTable(firstAnchor, FootprintRotation.Deg0));
            StoreTerrainForCoords(ResolveCasinoTable(secondAnchor, FootprintRotation.Deg90));
            FurnitureChangeResult first = PlaceCasinoTable(firstAnchor, FootprintRotation.Deg0);
            FurnitureChangeResult second = PlaceCasinoTable(secondAnchor, FootprintRotation.Deg90);

            IReadOnlyList<FurnitureSnapshot> snapshots = _mapManager.GetFurnitureSnapshots();

            Assert.That(snapshots.Select(snapshot => snapshot.Id).ToArray(), Is.EqualTo(new[] { first.FurnitureId, second.FurnitureId }));
            Assert.That(snapshots.Select(snapshot => snapshot.Anchor).ToArray(), Is.EqualTo(new[] { firstAnchor, secondAnchor }));
            Assert.That(snapshots.Select(snapshot => snapshot.Rotation).ToArray(), Is.EqualTo(new[] { FootprintRotation.Deg0, FootprintRotation.Deg90 }));
        }

        #endregion

        #region Conflicts

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(5)]
        public void Place_CasinoTable_WhenStructureExistsOnAnyFootprintCell_IsInvalidWithoutPartialReservations(
            int conflictIndex)
        {
            var anchor = new MapCellCoord(5, 0, 5);
            IReadOnlyList<MapCellCoord> casinoTableCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            MapCellCoord conflictCell = casinoTableCells[conflictIndex];
            StoreTerrainForCoords(casinoTableCells);
            BuildStructureResult blocker = PlaceBlock(conflictCell);

            FurnitureOperationResult result = _furnitureManager.Place(CasinoTablePlacement(anchor, FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(FurnitureFailureReason.StructurePresent));
            Assert.That(result.FailedCell, Is.EqualTo(conflictCell));
            Assert.That(result.FailedDefinitionId, Is.EqualTo(FurnitureDefinitions.CasinoTableDefinitionId));
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(0));
            AssertCellReserved(conflictCell, blocker.StructureId);
            AssertCasinoTableCellsHaveNoFurniture(casinoTableCells);
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(5)]
        public void Place_CasinoTable_WhenFurnitureExistsOnAnyFootprintCell_IsInvalidWithoutPartialReservations(
            int conflictIndex)
        {
            var anchor = new MapCellCoord(8, 0, 8);
            IReadOnlyList<MapCellCoord> casinoTableCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            MapCellCoord conflictCell = casinoTableCells[conflictIndex];
            StoreTerrainForCoords(casinoTableCells);
            FurnitureChangeResult blocker = PlaceFurniture(SingleCellFurnitureDefinition(), conflictCell, FootprintRotation.Deg0);

            FurnitureOperationResult result = _furnitureManager.Place(CasinoTablePlacement(anchor, FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(FurnitureFailureReason.FurniturePresent));
            Assert.That(result.FailedCell, Is.EqualTo(conflictCell));
            Assert.That(result.FailedDefinitionId, Is.EqualTo(FurnitureDefinitions.CasinoTableDefinitionId));
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(1));
            AssertFurnitureReserved(conflictCell, blocker.FurnitureId);
            AssertCasinoTableCellsHaveNoFurnitureExcept(casinoTableCells, conflictCell, blocker.FurnitureId);
        }

        #endregion

        #region Item Coexistence

        [Test]
        public void Place_CasinoTable_WhenItemAlreadyExistsOnFootprintCell_PreservesItemAndFurniture()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            MapCellCoord itemCell = expectedCells[2];
            var itemId = new ItemId(10);
            StoreTerrainForCoords(expectedCells);
            ReserveItem(itemCell, itemId);

            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg0);

            AssertFurnitureReserved(itemCell, placement.FurnitureId);
            AssertItemReserved(itemCell, itemId);
        }

        [Test]
        public void ReserveItem_WhenCasinoTableAlreadyOccupiesCell_PreservesFurnitureAndItem()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            MapCellCoord itemCell = expectedCells[3];
            var itemId = new ItemId(11);
            StoreTerrainForCoords(expectedCells);
            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg0);

            ReserveItem(itemCell, itemId);

            AssertFurnitureReserved(itemCell, placement.FurnitureId);
            AssertItemReserved(itemCell, itemId);
        }

        [Test]
        public void ReserveItem_MultipleItemsOnCasinoTableCell_PreservesFurnitureAndAllItems()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            MapCellCoord itemCell = expectedCells[4];
            var firstItemId = new ItemId(12);
            var secondItemId = new ItemId(13);
            StoreTerrainForCoords(expectedCells);
            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg0);

            ReserveItem(itemCell, firstItemId);
            ReserveItem(itemCell, secondItemId);

            AssertFurnitureReserved(itemCell, placement.FurnitureId);
            AssertItemReserved(itemCell, firstItemId);
            AssertItemReserved(itemCell, secondItemId);
            Assert.That(GetCell(itemCell).ItemIds, Has.Count.EqualTo(2));
        }

        #endregion

        #region Removal

        [Test]
        public void Remove_CasinoTableFromAnchor_ReleasesCompleteFootprintAndRemovesAggregate()
        {
            var anchor = new MapCellCoord(-3, 0, 6);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg90);
            StoreTerrainForCoords(expectedCells);
            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg90);

            FurnitureOperationResult result = _furnitureManager.Remove(new FurnitureRemovalRequest(anchor));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Valid));
            Assert.That(result.Changes, Has.Count.EqualTo(1));
            Assert.That(result.Changes.Single().Kind, Is.EqualTo(FurnitureChangeResultKind.Removed));
            Assert.That(result.Changes.Single().FurnitureId, Is.EqualTo(placement.FurnitureId));
            Assert.That(result.Changes.Single().AffectedCells, Is.EqualTo(placement.AffectedCells));
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(0));
            AssertCasinoTableCellsReleased(placement.AffectedCells);
            AssertTerrainStillExists(placement.AffectedCells);
        }

        [Test]
        public void Remove_CasinoTableFromNonAnchorCell_RemovesEntireFurniture()
        {
            var anchor = new MapCellCoord(-8, 0, -9);
            const FootprintRotation rotation = FootprintRotation.Deg270;
            StoreTerrainForCoords(ResolveCasinoTable(anchor, rotation));
            FurnitureChangeResult placement = PlaceCasinoTable(anchor, rotation);
            MapCellCoord nonAnchorCell = placement.AffectedCells.First(cell => cell != anchor);

            FurnitureOperationResult result = _furnitureManager.Remove(new FurnitureRemovalRequest(nonAnchorCell));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Valid));
            Assert.That(result.Changes.Single().FurnitureId, Is.EqualTo(placement.FurnitureId));
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(0));
            AssertCasinoTableCellsReleased(placement.AffectedCells);
        }

        [Test]
        public void Remove_CasinoTableWithItems_ReleasesFurnitureAndKeepsItemCellsAllocated()
        {
            var anchor = new MapCellCoord(4, 0, -6);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            var firstItemId = new ItemId(20);
            var secondItemId = new ItemId(21);
            StoreTerrainForCoords(expectedCells);
            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg0);
            ReserveItem(expectedCells[0], firstItemId);
            ReserveItem(expectedCells[5], secondItemId);

            FurnitureOperationResult result = _furnitureManager.Remove(new FurnitureRemovalRequest(expectedCells[3]));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Valid));
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(0));
            AssertItemReserved(expectedCells[0], firstItemId);
            AssertItemReserved(expectedCells[5], secondItemId);
            Assert.That(GetCell(expectedCells[0]).FurnitureId, Is.Null);
            Assert.That(GetCell(expectedCells[5]).FurnitureId, Is.Null);
            AssertEmptyCellsRemovedExcept(placement.AffectedCells, new[] { expectedCells[0], expectedCells[5] });
        }

        #endregion

        #region Chunk Boundaries

        [Test]
        public void Place_CasinoTableAcrossPositiveXChunkBoundary_SucceedsAsOneFurnitureAggregate()
        {
            var anchor = new MapCellCoord(MapChunkMetrics.ChunkSize - 2, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(expectedCells);

            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg0);

            Assert.That(placement.AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(expectedCells.Select(MapMath.CellToChunk).Distinct().Count(), Is.GreaterThan(1));
            AssertEveryCasinoTableCellResolvesToFurniture(placement);
        }

        [Test]
        public void Place_RotatedCasinoTableAcrossPositiveZChunkBoundary_SucceedsAsOneFurnitureAggregate()
        {
            var anchor = new MapCellCoord(0, 0, MapChunkMetrics.ChunkSize - 1);
            const FootprintRotation rotation = FootprintRotation.Deg270;
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, rotation);
            StoreTerrainForCoords(expectedCells);

            FurnitureChangeResult placement = PlaceCasinoTable(anchor, rotation);

            Assert.That(placement.AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(expectedCells.Select(MapMath.CellToChunk).Distinct().Count(), Is.GreaterThan(1));
            AssertEveryCasinoTableCellResolvesToFurniture(placement);
        }

        [Test]
        public void Place_CasinoTableAcrossNegativeXChunkBoundary_SucceedsAsOneFurnitureAggregate()
        {
            var anchor = new MapCellCoord(-1, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(expectedCells);

            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg0);

            Assert.That(placement.AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(expectedCells.Select(MapMath.CellToChunk).Distinct().Count(), Is.GreaterThan(1));
            AssertEveryCasinoTableCellResolvesToFurniture(placement);
        }

        [Test]
        public void Place_CrossChunkCasinoTableWithFarChunkConflict_RollsBackAllFurnitureCells()
        {
            var anchor = new MapCellCoord(MapChunkMetrics.ChunkSize - 2, 0, 0);
            IReadOnlyList<MapCellCoord> casinoTableCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            MapCellCoord conflictCell = casinoTableCells.Single(cell => cell.X == MapChunkMetrics.ChunkSize && cell.Z == 1);
            StoreTerrainForCoords(casinoTableCells);
            FurnitureChangeResult blocker = PlaceFurniture(SingleCellFurnitureDefinition(), conflictCell, FootprintRotation.Deg0);

            FurnitureOperationResult result = _furnitureManager.Place(CasinoTablePlacement(anchor, FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(FurnitureFailureReason.FurniturePresent));
            Assert.That(result.FailedCell, Is.EqualTo(conflictCell));
            Assert.That(_mapManager.FurnitureCount, Is.EqualTo(1));
            AssertFurnitureReserved(conflictCell, blocker.FurnitureId);
            AssertCasinoTableCellsHaveNoFurnitureExcept(casinoTableCells, conflictCell, blocker.FurnitureId);
        }

        #endregion

        #region Structure Regression

        [Test]
        public void Execute_StructurePlacementOnFurniture_IsInvalidWithoutMutatingFurniture()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveCasinoTable(anchor, FootprintRotation.Deg0);
            MapCellCoord conflictCell = expectedCells[1];
            StoreTerrainForCoords(expectedCells);
            FurnitureChangeResult placement = PlaceCasinoTable(anchor, FootprintRotation.Deg0);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                conflictCell,
                FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(conflictCell));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            AssertEveryCasinoTableCellResolvesToFurniture(placement);
        }

        [Test]
        public void StructureIdAndFurnitureId_AllocateIndependently()
        {
            var structureAnchor = new MapCellCoord(0, 0, 0);
            var furnitureAnchor = new MapCellCoord(10, 0, 10);
            StoreTerrainForCoords(new[] { structureAnchor });
            StoreTerrainForCoords(ResolveCasinoTable(furnitureAnchor, FootprintRotation.Deg0));

            BuildStructureResult structure = PlaceBlock(structureAnchor);
            FurnitureChangeResult furniture = PlaceCasinoTable(furnitureAnchor, FootprintRotation.Deg0);

            Assert.That(structure.StructureId, Is.EqualTo(new StructureId(1)));
            Assert.That(furniture.FurnitureId, Is.EqualTo(new FurnitureId(1)));
        }

        #endregion

        #region Helpers

        private FurniturePlacementRequest CasinoTablePlacement(
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            return new FurniturePlacementRequest(FurnitureDefinitions.CasinoTable, anchor, rotation);
        }

        private FurnitureDefinition SingleCellFurnitureDefinition()
        {
            return new FurnitureDefinition(
                new FurnitureDefinitionId(2999),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0)
                }));
        }

        private FurnitureChangeResult PlaceCasinoTable(
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            return PlaceFurniture(FurnitureDefinitions.CasinoTable, anchor, rotation);
        }

        private FurnitureChangeResult PlaceFurniture(
            FurnitureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            FurnitureOperationResult result = _furnitureManager.Place(new FurniturePlacementRequest(
                definition,
                anchor,
                rotation));

            Assert.That(result.Outcome, Is.EqualTo(FurnitureOperationOutcome.Valid));
            Assert.That(result.Changes, Has.Count.EqualTo(1));
            return result.Changes.Single();
        }

        private BuildStructureResult PlaceBlock(MapCellCoord anchor)
        {
            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(1));
            return result.Structures.Single();
        }

        private IReadOnlyList<MapCellCoord> ResolveCasinoTable(
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            return FurnitureDefinitions.CasinoTable.Footprint.Resolve(anchor, rotation);
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

        private void ReserveItem(MapCellCoord coord, ItemId itemId)
        {
            Cell cell = _mapManager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveItem(itemId);

            Assert.That(validation.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            cell.ReserveItem(validation, itemId);
        }

        private TerrainTile FlatTile()
        {
            return new TerrainTile(0f, 0f, 0f, 0f);
        }

        private Cell GetCell(MapCellCoord coord)
        {
            Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
            return cell!;
        }

        private void AssertStoredCasinoTable(
            FurnitureChangeResult result,
            MapCellCoord anchor,
            FootprintRotation rotation,
            IReadOnlyList<MapCellCoord> expectedCells)
        {
            Assert.That(result.Kind, Is.EqualTo(FurnitureChangeResultKind.Created));
            Assert.That(result.DefinitionId, Is.EqualTo(FurnitureDefinitions.CasinoTableDefinitionId));
            Assert.That(result.Anchor, Is.EqualTo(anchor));
            Assert.That(result.Rotation, Is.EqualTo(rotation));
            Assert.That(result.AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(_mapManager.TryGetFurniture(result.FurnitureId, out var furniture), Is.True);
            Assert.That(furniture!.Definition, Is.SameAs(FurnitureDefinitions.CasinoTable));
            Assert.That(furniture.Anchor, Is.EqualTo(anchor));
            Assert.That(furniture.Rotation, Is.EqualTo(rotation));
            Assert.That(furniture.ResolveOccupiedCells(), Is.EqualTo(expectedCells));
        }

        private void AssertEveryCasinoTableCellResolvesToFurniture(FurnitureChangeResult placement)
        {
            foreach (MapCellCoord cell in placement.AffectedCells)
            {
                AssertFurnitureReserved(cell, placement.FurnitureId);
                Assert.That(_mapManager.TryGetFurnitureIdAt(cell, out FurnitureId furnitureId), Is.True, cell.ToString());
                Assert.That(furnitureId, Is.EqualTo(placement.FurnitureId), cell.ToString());
            }
        }

        private void AssertCellReserved(MapCellCoord coord, StructureId structureId)
        {
            Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
            Assert.That(cell!.StructureId, Is.EqualTo(structureId), coord.ToString());
        }

        private void AssertFurnitureReserved(MapCellCoord coord, FurnitureId furnitureId)
        {
            Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
            Assert.That(cell!.FurnitureId, Is.EqualTo(furnitureId), coord.ToString());
        }

        private void AssertItemReserved(MapCellCoord coord, ItemId itemId)
        {
            Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
            Assert.That(cell!.HasItem(itemId), Is.True, coord.ToString());
        }

        private void AssertCasinoTableCellsHaveNoFurniture(IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(_mapManager.TryGetFurnitureIdAt(coord, out _), Is.False, coord.ToString());
            }
        }

        private void AssertCasinoTableCellsHaveNoFurnitureExcept(
            IEnumerable<MapCellCoord> coords,
            MapCellCoord excludedCell,
            FurnitureId expectedFurnitureId)
        {
            foreach (MapCellCoord coord in coords)
            {
                if (coord == excludedCell)
                {
                    AssertFurnitureReserved(coord, expectedFurnitureId);
                    continue;
                }

                Assert.That(_mapManager.TryGetFurnitureIdAt(coord, out _), Is.False, coord.ToString());
            }
        }

        private void AssertCasinoTableCellsReleased(IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(_mapManager.TryGetFurnitureIdAt(coord, out _), Is.False, coord.ToString());

                if (_mapManager.TryGetCell(coord, out var cell))
                {
                    Assert.That(cell!.FurnitureId, Is.Null, coord.ToString());
                }
            }
        }

        private void AssertEmptyCellsRemovedExcept(
            IEnumerable<MapCellCoord> coords,
            IReadOnlyCollection<MapCellCoord> retainedCells)
        {
            foreach (MapCellCoord coord in coords)
            {
                if (retainedCells.Contains(coord))
                {
                    Assert.That(_mapManager.TryGetCell(coord, out _), Is.True, coord.ToString());
                    continue;
                }

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

        #endregion
    }
}
