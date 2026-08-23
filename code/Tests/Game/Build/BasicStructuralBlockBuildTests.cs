using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal sealed class BasicStructuralBlockBuildTests
    {
        #region Fields

        private MapManager _mapManager = null!;
        private BuildManager _buildManager = null!;

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
        public void Evaluate_BlockPlacement_IsValidAndPureWithoutConsumingStructureId()
        {
            var anchor = new MapCellCoord(0, 25, 0);
            StoreTerrainForCoords(new[] { anchor });
            BuildIntent intent = BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                anchor,
                FootprintRotation.Deg270);

            BuildResult first = _buildManager.Evaluate(intent);
            BuildResult second = _buildManager.Evaluate(intent);

            AssertEquivalentBuildResults(first, second);
            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(first.Structures.Single().StructureId, Is.EqualTo(new StructureId(1)));
            Assert.That(first.Structures.Single().DefinitionId, Is.EqualTo(StructureDefinitions.BlockDefinitionId));
            Assert.That(first.Structures.Single().Anchor, Is.EqualTo(anchor));
            Assert.That(first.Structures.Single().Rotation, Is.EqualTo(FootprintRotation.Deg270));
            Assert.That(first.Structures.Single().AffectedCells, Is.EqualTo(new[] { anchor }));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
            Assert.That(_mapManager.TryGetTerrain(new TerrainTileWorldCoord(anchor.X, anchor.Z), out _), Is.True);
        }

        [Test]
        public void Execute_BlockPlacement_CreatesOneCanonicalStructureAtAnchor()
        {
            var anchor = new MapCellCoord(-2, 0, -3);
            StoreTerrainForCoords(new[] { anchor });
            BuildIntent intent = BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                anchor,
                FootprintRotation.Deg180);
            BuildResult evaluation = _buildManager.Evaluate(intent);

            BuildResult execution = _buildManager.Execute(intent);

            AssertEquivalentBuildResults(evaluation, execution);
            Assert.That(execution.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
            AssertStoredBlock(execution.Structures.Single(), anchor, FootprintRotation.Deg180);
            AssertCellReserved(anchor, execution.Structures.Single().StructureId);
        }

        [Test]
        public void Execute_BlockPlacement_AllowsArbitraryYAtSameHorizontalCoord()
        {
            MapCellCoord[] anchors =
            {
                new(5, -10, 5),
                new(5, 0, 5),
                new(5, 1, 5),
                new(5, 25, 5)
            };
            StoreTerrainForCoords(anchors);

            BuildStructureResult[] results = anchors
                .Select(anchor => ExecuteBlock(anchor))
                .ToArray();

            Assert.That(_mapManager.StructureCount, Is.EqualTo(anchors.Length));
            Assert.That(_mapManager.CellCount, Is.EqualTo(anchors.Length));
            Assert.That(results.Select(result => result.StructureId).Distinct().Count(), Is.EqualTo(anchors.Length));
            Assert.That(results.Select(result => result.DefinitionId), Is.All.EqualTo(StructureDefinitions.BlockDefinitionId));

            foreach (MapCellCoord anchor in anchors)
            {
                AssertCellReserved(anchor, results.Single(result => result.Anchor == anchor).StructureId);
            }
        }

        [Test]
        public void Execute_AdjacentHorizontalBlocks_AreIndependentStructures()
        {
            MapCellCoord[] anchors =
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(0, 0, 1)
            };
            StoreTerrainForCoords(anchors);

            BuildStructureResult[] results = anchors
                .Select(anchor => ExecuteBlock(anchor))
                .ToArray();

            Assert.That(_mapManager.StructureCount, Is.EqualTo(3));
            Assert.That(_mapManager.CellCount, Is.EqualTo(3));
            Assert.That(results.Select(result => result.StructureId).Distinct().Count(), Is.EqualTo(3));
            Assert.That(results.Select(result => result.DefinitionId), Is.All.EqualTo(StructureDefinitions.BlockDefinitionId));
        }

        [Test]
        public void Execute_BatchPlacement_CreatesHorizontalPlatformFromSeparateBlocks()
        {
            MapCellCoord[] anchors =
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(2, 0, 0)
            };
            StoreTerrainForCoords(anchors);
            BuildIntent intent = BuildIntent.PlaceStructures(anchors.Select(Placement).ToArray());

            BuildResult result = _buildManager.Execute(intent);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(anchors.Length));
            Assert.That(result.Structures.Select(structure => structure.Anchor), Is.EqualTo(anchors));
            Assert.That(result.Structures.Select(structure => structure.AffectedCells.Single()), Is.EqualTo(anchors));
            Assert.That(result.Structures.Select(structure => structure.DefinitionId), Is.All.EqualTo(StructureDefinitions.BlockDefinitionId));
            Assert.That(result.Structures.Select(structure => structure.StructureId).Distinct().Count(), Is.EqualTo(anchors.Length));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(anchors.Length));
            Assert.That(_mapManager.CellCount, Is.EqualTo(anchors.Length));
        }

        [Test]
        public void Execute_BatchPlacement_CreatesVerticalStackFromSeparateBlocks()
        {
            MapCellCoord[] anchors =
            {
                new(0, 0, 0),
                new(0, 1, 0),
                new(0, 2, 0)
            };
            StoreTerrainForCoords(anchors);
            BuildIntent intent = BuildIntent.PlaceStructures(anchors.Select(Placement).ToArray());

            BuildResult result = _buildManager.Execute(intent);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(anchors.Length));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(anchors.Length));
            Assert.That(_mapManager.CellCount, Is.EqualTo(anchors.Length));
            Assert.That(result.Structures.Select(structure => structure.StructureId).Distinct().Count(), Is.EqualTo(anchors.Length));
            Assert.That(result.Structures.Select(structure => structure.DefinitionId), Is.All.EqualTo(StructureDefinitions.BlockDefinitionId));

            foreach (BuildStructureResult structure in result.Structures)
            {
                Assert.That(structure.AffectedCells, Is.EqualTo(new[] { structure.Anchor }));
                AssertCellReserved(structure.Anchor, structure.StructureId);
            }
        }

        #endregion

        #region Conflicts

        [Test]
        public void Execute_BlockPlacement_WhenStructureExists_IsInvalidWithoutCreatingAnotherStructure()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(new[] { anchor });
            BuildStructureResult existing = ExecuteBlock(anchor);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(anchor));
            Assert.That(result.FailedDefinitionId, Is.EqualTo(StructureDefinitions.BlockDefinitionId));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
            AssertCellReserved(anchor, existing.StructureId);
        }

        [Test]
        public void Execute_BlockPlacement_WhenFurnitureExists_IsInvalidWithFurnitureConflict()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(new[] { anchor });
            ReserveFurniture(anchor, new FurnitureId(10));

            MapCellFootprintTransactionResult validation = _mapManager.ValidateReserveStructureFootprint(
                anchor,
                StructureDefinitions.Block.Footprint,
                FootprintRotation.Deg0,
                new StructureId(1));
            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(validation.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(validation.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(validation.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.FurniturePresent));
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
            Assert.That(_mapManager.TryGetCell(anchor, out var cell), Is.True);
            Assert.That(cell!.FurnitureId, Is.EqualTo(new FurnitureId(10)));
        }

        [Test]
        public void Execute_BlockPlacement_WhenItemExists_IsInvalidWithItemConflict()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(new[] { anchor });
            ReserveItem(anchor, new ItemId(20));

            MapCellFootprintTransactionResult validation = _mapManager.ValidateReserveStructureFootprint(
                anchor,
                StructureDefinitions.Block.Footprint,
                FootprintRotation.Deg0,
                new StructureId(1));
            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(validation.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(validation.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(validation.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.ItemsPresent));
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
            Assert.That(_mapManager.TryGetCell(anchor, out var cell), Is.True);
            Assert.That(cell!.HasItem(new ItemId(20)), Is.True);
        }

        [Test]
        public void CellContainingBlock_RejectsFurnitureAndItemReservations()
        {
            var anchor = new MapCellCoord(0, 0, 0);
            StoreTerrainForCoords(new[] { anchor });
            ExecuteBlock(anchor);
            Assert.That(_mapManager.TryGetCell(anchor, out var cell), Is.True);

            CellOccupancyValidationResult furniture = cell!.ValidateReserveFurniture(new FurnitureId(1));
            CellOccupancyValidationResult item = cell.ValidateReserveItem(new ItemId(1));

            Assert.That(furniture.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(furniture.FailureReason, Is.EqualTo(CellOccupancyFailureReason.StructurePresent));
            Assert.That(item.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(item.FailureReason, Is.EqualTo(CellOccupancyFailureReason.StructurePresent));
        }

        [Test]
        public void Execute_BatchPlacement_WithLaterBlockConflict_IsAtomic()
        {
            var first = new MapCellCoord(0, 0, 0);
            var second = new MapCellCoord(1, 0, 0);
            var conflict = new MapCellCoord(2, 0, 0);
            StoreTerrainForCoords(new[] { first, second, conflict });
            BuildStructureResult existing = ExecuteBlock(conflict);
            BuildIntent intent = BuildIntent.PlaceStructures(new[]
            {
                Placement(first),
                Placement(second),
                Placement(conflict)
            });

            BuildResult result = _buildManager.Execute(intent);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(conflict));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
            Assert.That(_mapManager.TryGetCell(first, out _), Is.False);
            Assert.That(_mapManager.TryGetCell(second, out _), Is.False);
            AssertCellReserved(conflict, existing.StructureId);
        }

        #endregion

        #region Removal

        [Test]
        public void Execute_RemoveStructureAtCellContainingBlock_RemovesReservationAndKeepsTerrain()
        {
            var anchor = new MapCellCoord(3, 0, 3);
            StoreTerrainForCoords(new[] { anchor });
            BuildStructureResult placement = ExecuteBlock(anchor);

            BuildResult result = _buildManager.Execute(BuildIntent.RemoveStructureAt(anchor));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures.Single().StructureId, Is.EqualTo(placement.StructureId));
            Assert.That(result.Structures.Single().DefinitionId, Is.EqualTo(StructureDefinitions.BlockDefinitionId));
            Assert.That(result.Structures.Single().AffectedCells, Is.EqualTo(new[] { anchor }));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.TryGetStructure(placement.StructureId, out _), Is.False);
            Assert.That(_mapManager.TryGetCell(anchor, out _), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
            Assert.That(_mapManager.TryGetTerrain(new TerrainTileWorldCoord(anchor.X, anchor.Z), out _), Is.True);

            BuildResult secondRemoval = _buildManager.Execute(BuildIntent.RemoveStructureAt(anchor));

            Assert.That(secondRemoval.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(secondRemoval.Structures, Is.Empty);
            Assert.That(_mapManager.TryGetTerrain(new TerrainTileWorldCoord(anchor.X, anchor.Z), out _), Is.True);
        }

        #endregion

        #region Helpers

        private static StructurePlacementRequest Placement(MapCellCoord anchor)
        {
            return new StructurePlacementRequest(
                StructureDefinitions.Block,
                anchor,
                FootprintRotation.Deg0);
        }

        private static TerrainTile FlatTile()
        {
            return new TerrainTile(0f, 0f, 0f, 0f);
        }

        private static void AssertEquivalentBuildResults(BuildResult expected, BuildResult actual)
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

        private BuildStructureResult ExecuteBlock(
            MapCellCoord anchor,
            FootprintRotation rotation = FootprintRotation.Deg0)
        {
            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Block,
                anchor,
                rotation));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            BuildStructureResult structure = result.Structures.Single();
            AssertStoredBlock(structure, anchor, rotation);
            return structure;
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

        private void ReserveItem(MapCellCoord coord, ItemId itemId)
        {
            Cell cell = _mapManager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveItem(itemId);
            Assert.That(validation.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            cell.ReserveItem(validation, itemId);
        }

        private void AssertStoredBlock(
            BuildStructureResult result,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            Assert.That(result.Kind, Is.EqualTo(BuildStructureResultKind.Created));
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.DefinitionId, Is.EqualTo(StructureDefinitions.BlockDefinitionId));
            Assert.That(result.Anchor, Is.EqualTo(anchor));
            Assert.That(result.Rotation, Is.EqualTo(rotation));
            Assert.That(result.AffectedCells, Is.EqualTo(new[] { anchor }));
            Assert.That(_mapManager.TryGetStructure(result.StructureId, out var structure), Is.True);
            Assert.That(structure!.Definition, Is.SameAs(StructureDefinitions.Block));
            Assert.That(structure.Anchor, Is.EqualTo(anchor));
            Assert.That(structure.Rotation, Is.EqualTo(rotation));
            Assert.That(structure.ResolveOccupiedCells(), Is.EqualTo(new[] { anchor }));
        }

        private void AssertCellReserved(MapCellCoord coord, StructureId structureId)
        {
            Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
            Assert.That(cell!.StructureId, Is.EqualTo(structureId), coord.ToString());
        }

        #endregion
    }
}
