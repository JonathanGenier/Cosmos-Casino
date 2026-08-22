using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapManagerChunkTests
    {
        #region Chunk Resolution

        [Test]
        public void GetOrCreateChunk_SameHorizontalCoordinateWithDifferentVerticalY_ReturnsSameChunk()
        {
            var manager = new MapManager();

            MapChunk below = manager.GetOrCreateChunk(new MapCellCoord(5, -100, 7));
            MapChunk baseLevel = manager.GetOrCreateChunk(new MapCellCoord(5, 0, 7));
            MapChunk above = manager.GetOrCreateChunk(new MapCellCoord(5, 100, 7));

            Assert.That(baseLevel, Is.SameAs(below));
            Assert.That(above, Is.SameAs(baseLevel));
            Assert.That(manager.ChunkCount, Is.EqualTo(1));
        }

        [Test]
        public void GetOrCreateChunk_WhenCalledWithChunkCoord_ReusesExistingChunk()
        {
            var manager = new MapManager();
            var coord = new MapChunkCoord(-1, 2);

            MapChunk first = manager.GetOrCreateChunk(coord);
            MapChunk second = manager.GetOrCreateChunk(coord);

            Assert.That(second, Is.SameAs(first));
            Assert.That(manager.ChunkCount, Is.EqualTo(1));
        }

        [Test]
        public void TryGetChunk_BeforeChunkIsResolved_ReturnsFalse()
        {
            var manager = new MapManager();

            bool found = manager.TryGetChunk(new MapChunkCoord(-99, 42), out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryGetChunk_AfterChunkIsResolved_ReturnsChunk()
        {
            var manager = new MapManager();
            var coord = new MapChunkCoord(-99, 42);
            MapChunk chunk = manager.GetOrCreateChunk(coord);

            bool found = manager.TryGetChunk(coord, out var resolved);

            Assert.That(found, Is.True);
            Assert.That(resolved, Is.SameAs(chunk));
        }

        #endregion
    }
}
