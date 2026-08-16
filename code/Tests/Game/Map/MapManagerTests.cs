using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapManagerTests
    {
        #region Generation Bounds

        [Test]
        public void GenerateMap_OddMapSize_CreatesSymmetricBoundsAroundZero()
        {
            var manager = new MapManager();

            manager.GenerateMap(seed: 0, mapSize: 5);

            Assert.That(manager.CellCount, Is.EqualTo(25));
            Assert.That(manager.TryGetCell(new MapCoord(-2, -2), out _), Is.True);
            Assert.That(manager.TryGetCell(new MapCoord(0, 0), out _), Is.True);
            Assert.That(manager.TryGetCell(new MapCoord(2, 2), out _), Is.True);
            Assert.That(manager.TryGetCell(new MapCoord(-3, 0), out _), Is.False);
            Assert.That(manager.TryGetCell(new MapCoord(3, 0), out _), Is.False);
            Assert.That(manager.TryGetCell(new MapCoord(0, -3), out _), Is.False);
            Assert.That(manager.TryGetCell(new MapCoord(0, 3), out _), Is.False);
        }

        [Test]
        public void GenerateMap_EvenPositiveMapSize_ThrowsArgumentException()
        {
            var manager = new MapManager();

            Assert.Throws<ArgumentException>(() =>
                manager.GenerateMap(seed: 0, mapSize: 4));
        }

        [Test]
        public void ConfiguredMapSize_HasUniqueCenterTile()
        {
            Assert.That(TerrainConfigs.TileCountPerAxis, Is.Positive);
            Assert.That(TerrainConfigs.TileCountPerAxis % 2, Is.EqualTo(1));
        }

        #endregion

        #region Build Elevation Compatibility

        [Test]
        public void CoordinateOnlyBuildOperations_TargetCellTerrainBaseElevation()
        {
            var manager = new MapManager();
            var coord = new MapCoord(-1, 1);
            manager.GenerateMap(seed: 0, mapSize: 5);
            Assert.That(manager.TryGetCell(coord, out var cell), Is.True);
            var baseElevation = cell!.TerrainTile.BaseElevation;

            var result = manager.TryPlace(BuildKind.Floor, coord);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(cell.HasFloorAt(baseElevation), Is.True);
        }

        #endregion

        #region Terrain Alignment

        [TestCase(0, 0)]
        [TestCase(-1, 1)]
        public void TryGetTerrainBaseElevation_ExistingCoordinate_ReturnsTerrainBaseElevation(int x, int y)
        {
            var manager = new MapManager();
            var coord = new MapCoord(x, y);
            manager.GenerateMap(seed: 0, mapSize: 5);
            Assert.That(manager.TryGetCell(coord, out var cell), Is.True);

            bool found = manager.TryGetTerrainBaseElevation(coord, out var elevation);

            Assert.That(found, Is.True);
            Assert.That(elevation, Is.EqualTo(cell!.TerrainTile.BaseElevation));
        }

        [Test]
        public void TryGetTerrainBaseElevation_MissingCoordinate_ReturnsFalse()
        {
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 5);

            bool found = manager.TryGetTerrainBaseElevation(new MapCoord(3, 0), out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryGetTerrain_TerrainWorldCoordAndMapCoordIdentifySameTile()
        {
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 5);

            Assert.That(
                manager.TryGetTerrain(new TerrainTileWorldCoord(0, 0), out var centerTerrain),
                Is.True);
            Assert.That(
                manager.TryGetTerrain(new MapCoord(0, 0), out var centerMapTerrain),
                Is.True);
            Assert.That(centerTerrain, Is.SameAs(centerMapTerrain));

            Assert.That(
                manager.TryGetTerrain(new TerrainTileWorldCoord(1, 0), out var eastTerrain),
                Is.True);
            Assert.That(
                manager.TryGetTerrain(new MapCoord(1, 0), out var eastMapTerrain),
                Is.True);
            Assert.That(eastTerrain, Is.SameAs(eastMapTerrain));

            Assert.That(
                manager.TryGetTerrain(new TerrainTileWorldCoord(-1, 0), out var westTerrain),
                Is.True);
            Assert.That(
                manager.TryGetTerrain(new MapCoord(-1, 0), out var westMapTerrain),
                Is.True);
            Assert.That(westTerrain, Is.SameAs(westMapTerrain));
        }

        #endregion
    }
}
