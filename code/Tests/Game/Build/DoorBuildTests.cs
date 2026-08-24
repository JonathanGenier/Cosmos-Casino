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
    internal sealed class DoorBuildTests
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

        [TestCase(FootprintRotation.Deg0)]
        [TestCase(FootprintRotation.Deg90)]
        [TestCase(FootprintRotation.Deg180)]
        [TestCase(FootprintRotation.Deg270)]
        public void Execute_DoorPlacement_CreatesOneAuthoritativeStructureForCompleteFootprint(
            FootprintRotation rotation)
        {
            var anchor = new MapCellCoord(-4, 2, 9);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveDoor(anchor, rotation);
            StoreTerrainForCoords(expectedCells);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Door,
                anchor,
                rotation));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(1));
            AssertStoredDoor(result.Structures.Single(), anchor, rotation, expectedCells);
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            Assert.That(_mapManager.CellCount, Is.EqualTo(expectedCells.Count));
            AssertEveryDoorCellResolvesToStructure(result.Structures.Single());
        }

        [Test]
        public void TryGetStructureSnapshotAt_EveryDoorCell_ReturnsSameAuthoritativeSnapshot()
        {
            var anchor = new MapCellCoord(3, 1, -5);
            const FootprintRotation rotation = FootprintRotation.Deg270;
            StoreTerrainForCoords(ResolveDoor(anchor, rotation));
            BuildStructureResult placement = ExecuteDoor(anchor, rotation);

            foreach (MapCellCoord cell in placement.AffectedCells)
            {
                Assert.That(_mapManager.TryGetStructureSnapshotAt(cell, out StructureSnapshot snapshot), Is.True, cell.ToString());
                Assert.That(snapshot.Id, Is.EqualTo(placement.StructureId), cell.ToString());
                Assert.That(snapshot.Definition, Is.SameAs(StructureDefinitions.Door), cell.ToString());
                Assert.That(snapshot.Anchor, Is.EqualTo(anchor), cell.ToString());
                Assert.That(snapshot.Rotation, Is.EqualTo(rotation), cell.ToString());
            }
        }

        [Test]
        public void GetStructureSnapshots_IncludesDoorDefinitionAnchorAndRotation()
        {
            var anchor = new MapCellCoord(12, 4, -13);
            const FootprintRotation rotation = FootprintRotation.Deg90;
            StoreTerrainForCoords(ResolveDoor(anchor, rotation));
            BuildStructureResult placement = ExecuteDoor(anchor, rotation);

            StructureSnapshot snapshot = _mapManager.GetStructureSnapshots().Single();

            Assert.That(snapshot.Id, Is.EqualTo(placement.StructureId));
            Assert.That(snapshot.Definition, Is.SameAs(StructureDefinitions.Door));
            Assert.That(snapshot.Anchor, Is.EqualTo(anchor));
            Assert.That(snapshot.Rotation, Is.EqualTo(rotation));
        }

        #endregion

        #region Conflicts

        [TestCase(FootprintRotation.Deg0)]
        [TestCase(FootprintRotation.Deg90)]
        [TestCase(FootprintRotation.Deg180)]
        [TestCase(FootprintRotation.Deg270)]
        public void Execute_DoorPlacement_WhenAnyFootprintCellConflicts_IsInvalidWithoutPartialReservations(
            FootprintRotation rotation)
        {
            var anchor = new MapCellCoord(5, 0, 5);
            IReadOnlyList<MapCellCoord> doorCells = ResolveDoor(anchor, rotation);
            MapCellCoord conflictCell = doorCells.Last();
            StoreTerrainForCoords(doorCells);
            BuildStructureResult blocker = ExecuteStructure(
                StructureDefinitions.Block,
                conflictCell,
                FootprintRotation.Deg0);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Door,
                anchor,
                rotation));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(conflictCell));
            Assert.That(result.FailedDefinitionId, Is.EqualTo(StructureDefinitions.DoorDefinitionId));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
            AssertCellReserved(conflictCell, blocker.StructureId);
            AssertDoorCellsUnreservedExcept(doorCells, conflictCell);
        }

        [Test]
        public void Execute_CrossChunkDoorPlacement_WithConflictingCell_RollsBackAllDoorCells()
        {
            var anchor = new MapCellCoord(MapChunkMetrics.ChunkSize - 1, 0, 0);
            IReadOnlyList<MapCellCoord> doorCells = ResolveDoor(anchor, FootprintRotation.Deg0);
            MapCellCoord conflictCell = doorCells.Single(cell => cell.X == MapChunkMetrics.ChunkSize && cell.Y == 0);
            StoreTerrainForCoords(doorCells);
            BuildStructureResult blocker = ExecuteStructure(
                StructureDefinitions.Block,
                conflictCell,
                FootprintRotation.Deg0);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Door,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(conflictCell));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
            AssertCellReserved(conflictCell, blocker.StructureId);
            AssertDoorCellsUnreservedExcept(doorCells, conflictCell);
        }

        #endregion

        #region Removal

        [Test]
        public void Execute_RemoveStructureAtNonAnchorDoorCell_RemovesEntireDoor()
        {
            var anchor = new MapCellCoord(-8, 2, -9);
            const FootprintRotation rotation = FootprintRotation.Deg270;
            StoreTerrainForCoords(ResolveDoor(anchor, rotation));
            BuildStructureResult placement = ExecuteDoor(anchor, rotation);
            MapCellCoord nonAnchorCell = placement.AffectedCells.First(cell => cell != anchor);

            BuildResult result = _buildManager.Execute(BuildIntent.RemoveStructureAt(nonAnchorCell));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(1));
            Assert.That(result.Structures.Single().Kind, Is.EqualTo(BuildStructureResultKind.Removed));
            Assert.That(result.Structures.Single().StructureId, Is.EqualTo(placement.StructureId));
            Assert.That(result.Structures.Single().DefinitionId, Is.EqualTo(StructureDefinitions.DoorDefinitionId));
            Assert.That(result.Structures.Single().Anchor, Is.EqualTo(anchor));
            Assert.That(result.Structures.Single().Rotation, Is.EqualTo(rotation));
            Assert.That(result.Structures.Single().AffectedCells, Is.EqualTo(placement.AffectedCells));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.TryGetStructure(placement.StructureId, out _), Is.False);
            AssertDoorCellsReleased(placement.AffectedCells);
            AssertTerrainStillExists(placement.AffectedCells);
        }

        #endregion

        #region Chunk Boundaries

        [Test]
        public void Execute_DoorPlacementAcrossPositiveXChunkBoundary_SucceedsAtomically()
        {
            var anchor = new MapCellCoord(MapChunkMetrics.ChunkSize - 1, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveDoor(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(expectedCells);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Door,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures.Single().AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(expectedCells.Select(MapMath.CellToChunk).Distinct().Count(), Is.GreaterThan(1));
            AssertEveryDoorCellResolvesToStructure(result.Structures.Single());
        }

        [Test]
        public void Execute_RotatedDoorPlacementAcrossPositiveZChunkBoundary_SucceedsAtomically()
        {
            var anchor = new MapCellCoord(0, 0, MapChunkMetrics.ChunkSize - 1);
            const FootprintRotation rotation = FootprintRotation.Deg270;
            IReadOnlyList<MapCellCoord> expectedCells = ResolveDoor(anchor, rotation);
            StoreTerrainForCoords(expectedCells);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Door,
                anchor,
                rotation));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures.Single().AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(expectedCells.Select(MapMath.CellToChunk).Distinct().Count(), Is.GreaterThan(1));
            AssertEveryDoorCellResolvesToStructure(result.Structures.Single());
        }

        [Test]
        public void Execute_DoorPlacementAcrossNegativeXChunkBoundary_SucceedsAtomically()
        {
            var anchor = new MapCellCoord(-1, 0, 0);
            IReadOnlyList<MapCellCoord> expectedCells = ResolveDoor(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(expectedCells);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Door,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures.Single().AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(expectedCells.Select(MapMath.CellToChunk).Distinct().Count(), Is.GreaterThan(1));
            AssertEveryDoorCellResolvesToStructure(result.Structures.Single());
        }

        #endregion

        #region Helpers

        private BuildStructureResult ExecuteDoor(
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            return ExecuteStructure(StructureDefinitions.Door, anchor, rotation);
        }

        private BuildStructureResult ExecuteStructure(
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(definition, anchor, rotation));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            return result.Structures.Single();
        }

        private IReadOnlyList<MapCellCoord> ResolveDoor(
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            return StructureDefinitions.Door.Footprint.Resolve(anchor, rotation);
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

        private TerrainTile FlatTile()
        {
            return new TerrainTile(0f, 0f, 0f, 0f);
        }

        private void AssertStoredDoor(
            BuildStructureResult result,
            MapCellCoord anchor,
            FootprintRotation rotation,
            IReadOnlyList<MapCellCoord> expectedCells)
        {
            Assert.That(result.Kind, Is.EqualTo(BuildStructureResultKind.Created));
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.DefinitionId, Is.EqualTo(StructureDefinitions.DoorDefinitionId));
            Assert.That(result.Anchor, Is.EqualTo(anchor));
            Assert.That(result.Rotation, Is.EqualTo(rotation));
            Assert.That(result.AffectedCells, Is.EqualTo(expectedCells));
            Assert.That(_mapManager.TryGetStructure(result.StructureId, out var structure), Is.True);
            Assert.That(structure!.Definition, Is.SameAs(StructureDefinitions.Door));
            Assert.That(structure.Anchor, Is.EqualTo(anchor));
            Assert.That(structure.Rotation, Is.EqualTo(rotation));
            Assert.That(structure.ResolveOccupiedCells(), Is.EqualTo(expectedCells));
        }

        private void AssertEveryDoorCellResolvesToStructure(BuildStructureResult placement)
        {
            foreach (MapCellCoord cell in placement.AffectedCells)
            {
                Assert.That(_mapManager.TryGetStructureIdAt(cell, out StructureId structureId), Is.True, cell.ToString());
                Assert.That(structureId, Is.EqualTo(placement.StructureId), cell.ToString());
                AssertCellReserved(cell, placement.StructureId);
            }
        }

        private void AssertCellReserved(MapCellCoord coord, StructureId structureId)
        {
            Assert.That(_mapManager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
            Assert.That(cell!.StructureId, Is.EqualTo(structureId), coord.ToString());
        }

        private void AssertDoorCellsUnreservedExcept(
            IReadOnlyList<MapCellCoord> doorCells,
            MapCellCoord excludedCell)
        {
            foreach (MapCellCoord cell in doorCells)
            {
                if (cell == excludedCell)
                {
                    continue;
                }

                Assert.That(_mapManager.TryGetCell(cell, out _), Is.False, cell.ToString());
            }
        }

        private void AssertDoorCellsReleased(IEnumerable<MapCellCoord> doorCells)
        {
            foreach (MapCellCoord cell in doorCells)
            {
                Assert.That(_mapManager.TryGetStructureIdAt(cell, out _), Is.False, cell.ToString());
                Assert.That(_mapManager.TryGetCell(cell, out _), Is.False, cell.ToString());
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
