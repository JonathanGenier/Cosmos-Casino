using CosmosCasino.Core.Game.Map;
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
    }
}
