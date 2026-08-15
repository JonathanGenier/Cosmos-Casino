using CosmosCasino.Core.Game;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain
{
    [TestFixture]
    internal class TerrainMathTests
    {
        #region Tile Indexes

        [TestCase(0, 5, -2)]
        [TestCase(2, 5, 0)]
        [TestCase(4, 5, 2)]
        [TestCase(0, 225, -112)]
        [TestCase(112, 225, 0)]
        [TestCase(224, 225, 112)]
        public void TileIndexToWorldCoord_ProducesCenteredSignedCoordinate(int index, int tileCount, int expected)
        {
            int coord = TerrainMath.TileIndexToWorldCoord(index, tileCount);

            Assert.That(coord, Is.EqualTo(expected));
        }

        [Test]
        public void TileIndexToWorldCoord_EvenTileCount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                TerrainMath.TileIndexToWorldCoord(index: 0, tileCount: 4));
        }

        #endregion

        #region Tile Alignment

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(-1, 0)]
        [TestCase(17, -8)]
        public void TileToWorldCenter_MatchesMapCellCenter(int x, int y)
        {
            var terrainCoord = new TerrainTileWorldCoord(x, y);
            var mapCoord = new MapCoord(x, y);

            Assert.That(
                TerrainMath.TileToWorldCenter(terrainCoord),
                Is.EqualTo(MapMath.CellToWorldCenter(mapCoord)));
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(-1, 0)]
        [TestCase(17, -8)]
        public void TileToWorldOrigin_MatchesMapCellOrigin(int x, int y)
        {
            var terrainCoord = new TerrainTileWorldCoord(x, y);
            var mapCoord = new MapCoord(x, y);

            Assert.That(
                TerrainMath.TileToWorldOrigin(terrainCoord),
                Is.EqualTo(MapMath.CellToWorldOrigin(mapCoord)));
        }

        [TestCase(0, 0)]
        [TestCase(4, 9)]
        [TestCase(-4, -9)]
        [TestCase(-12, 7)]
        public void TileAndMapCellAtSameCoordinate_HaveSamePhysicalFootprint(int x, int y)
        {
            var terrainOrigin = TerrainMath.TileToWorldOrigin(new TerrainTileWorldCoord(x, y));
            var mapOrigin = MapMath.CellToWorldOrigin(new MapCoord(x, y));

            var terrainMaximum = new WorldCoord(
                terrainOrigin.X + WorldGridMetrics.GridUnitSize,
                terrainOrigin.Y + WorldGridMetrics.GridUnitSize);
            var mapMaximum = new WorldCoord(
                mapOrigin.X + WorldGridMetrics.GridUnitSize,
                mapOrigin.Y + WorldGridMetrics.GridUnitSize);

            Assert.That(terrainOrigin, Is.EqualTo(mapOrigin));
            Assert.That(terrainMaximum, Is.EqualTo(mapMaximum));
        }

        #endregion

        #region Chunk Indexes

        [TestCase(0, 15, -7)]
        [TestCase(7, 15, 0)]
        [TestCase(14, 15, 7)]
        public void ChunkIndexToGridCoord_ProducesCenteredSignedCoordinate(int index, int chunkCount, int expected)
        {
            int coord = TerrainMath.ChunkIndexToGridCoord(index, chunkCount);

            Assert.That(coord, Is.EqualTo(expected));
        }

        [Test]
        public void ChunkIndexToGridCoord_EvenChunkCount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                TerrainMath.ChunkIndexToGridCoord(index: 0, chunkCount: 14));
        }

        #endregion

        #region Chunk Local To World

        [TestCase(0, 0, 7, 7, 0, 0)]
        [TestCase(0, 0, 0, 0, -7, -7)]
        [TestCase(0, 0, 14, 14, 7, 7)]
        [TestCase(1, 0, 0, 7, 8, 0)]
        [TestCase(-1, 0, 14, 7, -8, 0)]
        [TestCase(0, 1, 7, 0, 0, 8)]
        [TestCase(0, -1, 7, 14, 0, -8)]
        [TestCase(7, 7, 14, 14, 112, 112)]
        [TestCase(-7, -7, 0, 0, -112, -112)]
        public void ChunkLocalToWorldTileCoord_MapsChunkLocalIndexesToSignedWorldTiles(
            int chunkX,
            int chunkY,
            int localX,
            int localY,
            int expectedX,
            int expectedY)
        {
            var chunkCoord = new TerrainChunkGridCoord(chunkX, chunkY);
            var localCoord = new TerrainChunkLocalCoord(localX, localY);

            var worldCoord = TerrainMath.ChunkLocalToWorldTileCoord(
                chunkCoord,
                localCoord,
                chunkSize: 15);

            Assert.That(worldCoord, Is.EqualTo(new TerrainTileWorldCoord(expectedX, expectedY)));
        }

        [Test]
        public void ChunkLocalToWorldTileCoord_NegativeLocalCoord_ThrowsArgumentOutOfRangeException()
        {
            var chunkCoord = new TerrainChunkGridCoord(0, 0);
            var localCoord = new TerrainChunkLocalCoord(-1, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                TerrainMath.ChunkLocalToWorldTileCoord(chunkCoord, localCoord, chunkSize: 15));
        }

        [Test]
        public void ChunkLocalToWorldTileCoord_LocalCoordAtChunkSize_ThrowsArgumentOutOfRangeException()
        {
            var chunkCoord = new TerrainChunkGridCoord(0, 0);
            var localCoord = new TerrainChunkLocalCoord(15, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                TerrainMath.ChunkLocalToWorldTileCoord(chunkCoord, localCoord, chunkSize: 15));
        }

        #endregion
    }
}
