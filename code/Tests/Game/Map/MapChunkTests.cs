using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapChunkTests
    {
        #region Identity

        [Test]
        public void Constructor_AssignsChunkCoordinateIdentity()
        {
            var coord = new MapChunkCoord(-2, 3);
            var chunk = new MapChunk(coord);

            Assert.That(chunk.Coord, Is.EqualTo(coord));
            Assert.That(chunk.ChunkSize, Is.EqualTo(MapChunkMetrics.ChunkSize));
        }

        #endregion

        #region Terrain Storage

        [Test]
        public void StoreGeneratedTerrain_AtLocalCoordinate_CanRetrieveSameTerrainTile()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));
            var local = new MapChunkLocalCoord(2, 3);
            var terrainTile = FlatTile(1f);

            chunk.StoreGeneratedTerrain(local, terrainTile);

            Assert.That(chunk.TryGetTerrain(local, out var storedTerrain), Is.True);
            Assert.That(storedTerrain, Is.SameAs(terrainTile));
            Assert.That(chunk.TerrainTileCount, Is.EqualTo(1));
        }

        [Test]
        public void TryGetTerrain_EmptyLocalSlot_ReturnsFalse()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));

            bool found = chunk.TryGetTerrain(new MapChunkLocalCoord(2, 3), out _);

            Assert.That(found, Is.False);
            Assert.That(chunk.TerrainTileCount, Is.EqualTo(0));
        }

        [Test]
        public void StoreGeneratedTerrain_DuplicateLocalCoordinate_ThrowsInvalidOperationException()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));
            var local = new MapChunkLocalCoord(2, 3);
            chunk.StoreGeneratedTerrain(local, FlatTile(1f));

            Assert.Throws<InvalidOperationException>(() =>
                chunk.StoreGeneratedTerrain(local, FlatTile(2f)));
            Assert.That(chunk.TerrainTileCount, Is.EqualTo(1));
        }

        [Test]
        public void TryReplaceTerrain_ExistingLocalCoordinate_ReplacesTerrainTile()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));
            var local = new MapChunkLocalCoord(2, 3);
            chunk.StoreGeneratedTerrain(local, FlatTile(1f));
            var replacement = FlatTile(4f);

            bool replaced = chunk.TryReplaceTerrain(local, replacement);

            Assert.That(replaced, Is.True);
            Assert.That(chunk.TryGetTerrain(local, out var storedTerrain), Is.True);
            Assert.That(storedTerrain, Is.SameAs(replacement));
            Assert.That(chunk.TerrainTileCount, Is.EqualTo(1));
        }

        [Test]
        public void TryReplaceTerrain_EmptyLocalCoordinate_ReturnsFalse()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));

            bool replaced = chunk.TryReplaceTerrain(new MapChunkLocalCoord(2, 3), FlatTile(4f));

            Assert.That(replaced, Is.False);
            Assert.That(chunk.TerrainTileCount, Is.EqualTo(0));
        }

        #endregion

        #region Spatial Queries

        [Test]
        public void Contains_WhenSameHorizontalRegionWithDifferentVerticalY_ReturnsTrue()
        {
            int size = MapChunkMetrics.ChunkSize;
            var chunkCoord = new MapChunkCoord(-2, 3);
            var chunk = new MapChunk(chunkCoord);
            int globalX = (chunkCoord.X * size) + (size - 1);
            int globalZ = chunkCoord.Z * size;

            Assert.That(chunk.Contains(new MapCellCoord(globalX, -100, globalZ)), Is.True);
            Assert.That(chunk.Contains(new MapCellCoord(globalX, 0, globalZ)), Is.True);
            Assert.That(chunk.Contains(new MapCellCoord(globalX, 100, globalZ)), Is.True);
        }

        [Test]
        public void Contains_WhenHorizontalRegionDiffers_ReturnsFalse()
        {
            int size = MapChunkMetrics.ChunkSize;
            var chunk = new MapChunk(new MapChunkCoord(0, 0));

            Assert.That(chunk.Contains(new MapCellCoord(size, 0, 0)), Is.False);
            Assert.That(chunk.Contains(new MapCellCoord(0, 0, -1)), Is.False);
        }

        #endregion

        #region Helpers

        private static TerrainTile FlatTile(float height)
        {
            return new TerrainTile(height, height, height, height);
        }

        #endregion
    }
}
