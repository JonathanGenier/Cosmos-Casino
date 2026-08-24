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
    internal sealed class PillarBuildTests
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
        public void Execute_PillarPlacement_CreatesAuthoritativeStructure(FootprintRotation rotation)
        {
            var anchor = new MapCellCoord(-4, 3, 9);
            StoreTerrainForCoords(new[] { anchor });
            BuildIntent intent = BuildIntent.PlaceStructure(
                StructureDefinitions.Pillar,
                anchor,
                rotation);

            BuildResult result = _buildManager.Execute(intent);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(1));
            AssertStoredPillar(result.Structures.Single(), anchor, rotation);
            AssertCellReserved(anchor, result.Structures.Single().StructureId);
        }

        [Test]
        public void Execute_PillarPlacement_WhenCellContainsBlock_IsInvalidWithoutPartialMutation()
        {
            var anchor = new MapCellCoord(1, 0, 1);
            StoreTerrainForCoords(new[] { anchor });
            BuildStructureResult block = ExecuteStructure(StructureDefinitions.Block, anchor, FootprintRotation.Deg0);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Pillar,
                anchor,
                FootprintRotation.Deg0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(anchor));
            Assert.That(result.FailedDefinitionId, Is.EqualTo(StructureDefinitions.PillarDefinitionId));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            AssertCellReserved(anchor, block.StructureId);
        }

        [Test]
        public void Execute_PillarPlacement_WhenCellContainsPillar_IsInvalidWithoutPartialMutation()
        {
            var anchor = new MapCellCoord(2, 0, -2);
            StoreTerrainForCoords(new[] { anchor });
            BuildStructureResult existing = ExecuteStructure(StructureDefinitions.Pillar, anchor, FootprintRotation.Deg0);

            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(
                StructureDefinitions.Pillar,
                anchor,
                FootprintRotation.Deg180));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildFailureReason.OccupancyConflict));
            Assert.That(result.FailedCell, Is.EqualTo(anchor));
            Assert.That(result.FailedDefinitionId, Is.EqualTo(StructureDefinitions.PillarDefinitionId));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(1));
            AssertCellReserved(anchor, existing.StructureId);
        }

        #endregion

        #region Removal

        [Test]
        public void Execute_RemoveStructureAtCellContainingPillar_RemovesReservationAndKeepsTerrain()
        {
            var anchor = new MapCellCoord(-8, 4, -9);
            StoreTerrainForCoords(new[] { anchor });
            BuildStructureResult placement = ExecuteStructure(StructureDefinitions.Pillar, anchor, FootprintRotation.Deg270);

            BuildResult result = _buildManager.Execute(BuildIntent.RemoveStructureAt(anchor));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Structures, Has.Count.EqualTo(1));
            Assert.That(result.Structures.Single().Kind, Is.EqualTo(BuildStructureResultKind.Removed));
            Assert.That(result.Structures.Single().StructureId, Is.EqualTo(placement.StructureId));
            Assert.That(result.Structures.Single().DefinitionId, Is.EqualTo(StructureDefinitions.PillarDefinitionId));
            Assert.That(result.Structures.Single().Anchor, Is.EqualTo(anchor));
            Assert.That(result.Structures.Single().Rotation, Is.EqualTo(FootprintRotation.Deg270));
            Assert.That(result.Structures.Single().AffectedCells, Is.EqualTo(new[] { anchor }));
            Assert.That(_mapManager.StructureCount, Is.EqualTo(0));
            Assert.That(_mapManager.TryGetStructure(placement.StructureId, out _), Is.False);
            Assert.That(_mapManager.TryGetCell(anchor, out _), Is.False);
            Assert.That(_mapManager.TryGetTerrain(new TerrainTileWorldCoord(anchor.X, anchor.Z), out _), Is.True);
        }

        #endregion

        #region Snapshots

        [Test]
        public void GetStructureSnapshots_IncludesPillarDefinitionAnchorAndRotation()
        {
            var anchor = new MapCellCoord(12, 6, -13);
            StoreTerrainForCoords(new[] { anchor });
            BuildStructureResult placement = ExecuteStructure(StructureDefinitions.Pillar, anchor, FootprintRotation.Deg90);

            StructureSnapshot snapshot = _mapManager.GetStructureSnapshots().Single();

            Assert.That(snapshot.Id, Is.EqualTo(placement.StructureId));
            Assert.That(snapshot.Definition, Is.SameAs(StructureDefinitions.Pillar));
            Assert.That(snapshot.Anchor, Is.EqualTo(anchor));
            Assert.That(snapshot.Rotation, Is.EqualTo(FootprintRotation.Deg90));
        }

        #endregion

        #region Helpers

        private BuildStructureResult ExecuteStructure(
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            BuildResult result = _buildManager.Execute(BuildIntent.PlaceStructure(definition, anchor, rotation));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            return result.Structures.Single();
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

        private void AssertStoredPillar(
            BuildStructureResult result,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            Assert.That(result.Kind, Is.EqualTo(BuildStructureResultKind.Created));
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.DefinitionId, Is.EqualTo(StructureDefinitions.PillarDefinitionId));
            Assert.That(result.Anchor, Is.EqualTo(anchor));
            Assert.That(result.Rotation, Is.EqualTo(rotation));
            Assert.That(result.AffectedCells, Is.EqualTo(new[] { anchor }));
            Assert.That(_mapManager.TryGetStructure(result.StructureId, out var structure), Is.True);
            Assert.That(structure!.Definition, Is.SameAs(StructureDefinitions.Pillar));
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
