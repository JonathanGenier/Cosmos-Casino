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

        #region Cell Storage

        [Test]
        public void GetOrCreateCell_EmptyLocalCoordinate_CreatesSparseCellWithGlobalCoordinate()
        {
            var chunkCoord = new MapChunkCoord(-1, 2);
            var chunk = new MapChunk(chunkCoord);
            var local = new MapChunkLocalCoord(3, 4);
            const int y = -7;
            var expectedCoord = MapMath.ChunkLocalToCell(chunkCoord, local, y);

            var cell = chunk.GetOrCreateCell(local, y);

            Assert.That(cell.Coord, Is.EqualTo(expectedCoord));
            Assert.That(chunk.CellCount, Is.EqualTo(1));
            Assert.That(chunk.TryGetCell(local, y, out var storedCell), Is.True);
            Assert.That(storedCell, Is.SameAs(cell));
        }

        [Test]
        public void GetOrCreateCell_DuplicateLocalAndY_ReturnsExistingSparseCell()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));
            var local = new MapChunkLocalCoord(2, 3);

            var first = chunk.GetOrCreateCell(local, y: 5);
            var second = chunk.GetOrCreateCell(local, y: 5);

            Assert.That(second, Is.SameAs(first));
            Assert.That(chunk.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void GetOrCreateCell_SameLocalWithDifferentGlobalY_CreatesIndependentSparseCells()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));
            var local = new MapChunkLocalCoord(2, 3);

            var lower = chunk.GetOrCreateCell(local, y: -100);
            var origin = chunk.GetOrCreateCell(local, y: 0);
            var upper = chunk.GetOrCreateCell(local, y: 100);

            Assert.That(lower, Is.Not.SameAs(origin));
            Assert.That(origin, Is.Not.SameAs(upper));
            Assert.That(lower.Coord.Y, Is.EqualTo(-100));
            Assert.That(origin.Coord.Y, Is.EqualTo(0));
            Assert.That(upper.Coord.Y, Is.EqualTo(100));
            Assert.That(chunk.CellCount, Is.EqualTo(3));
        }

        [Test]
        public void TryGetCell_EmptySparseCoordinate_ReturnsFalseWithoutCreatingCell()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));

            bool found = chunk.TryGetCell(new MapChunkLocalCoord(2, 3), y: 5, out _);

            Assert.That(found, Is.False);
            Assert.That(chunk.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveCell_RemovesOnlyMatchingLocalAndY()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));
            var local = new MapChunkLocalCoord(2, 3);
            chunk.GetOrCreateCell(local, y: 1);
            var remaining = chunk.GetOrCreateCell(local, y: 2);

            bool removed = chunk.TryRemoveCell(local, y: 1);

            Assert.That(removed, Is.True);
            Assert.That(chunk.TryGetCell(local, y: 1, out _), Is.False);
            Assert.That(chunk.TryGetCell(local, y: 2, out var storedCell), Is.True);
            Assert.That(storedCell, Is.SameAs(remaining));
            Assert.That(chunk.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void TryRemoveCell_DoesNotRemoveDenseTerrainAtSameLocalCoordinate()
        {
            var chunk = new MapChunk(new MapChunkCoord(0, 0));
            var local = new MapChunkLocalCoord(2, 3);
            var terrainTile = FlatTile(1f);
            chunk.StoreGeneratedTerrain(local, terrainTile);
            chunk.GetOrCreateCell(local, y: 0);

            bool removed = chunk.TryRemoveCell(local, y: 0);

            Assert.That(removed, Is.True);
            Assert.That(chunk.CellCount, Is.EqualTo(0));
            Assert.That(chunk.TryGetTerrain(local, out var storedTerrain), Is.True);
            Assert.That(storedTerrain, Is.SameAs(terrainTile));
            Assert.That(chunk.TerrainTileCount, Is.EqualTo(1));
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
