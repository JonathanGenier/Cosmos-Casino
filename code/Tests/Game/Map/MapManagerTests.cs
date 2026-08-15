using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal class MapManagerTests
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

        #region Terrain Alignment

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
