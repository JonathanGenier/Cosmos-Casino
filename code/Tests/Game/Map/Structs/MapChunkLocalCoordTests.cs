using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapChunkLocalCoordTests
    {
        #region Range

        [Test]
        public void Constructor_WithFirstAndLastLocalIndexes_Succeeds()
        {
            var first = new MapChunkLocalCoord(0, 0);
            var last = new MapChunkLocalCoord(
                MapChunkMetrics.ChunkSize - 1,
                MapChunkMetrics.ChunkSize - 1);

            Assert.That(first.X, Is.EqualTo(0));
            Assert.That(first.Z, Is.EqualTo(0));
            Assert.That(last.X, Is.EqualTo(MapChunkMetrics.ChunkSize - 1));
            Assert.That(last.Z, Is.EqualTo(MapChunkMetrics.ChunkSize - 1));
        }

        [Test]
        public void Constructor_WithNegativeLocalIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapChunkLocalCoord(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapChunkLocalCoord(0, -1));
        }

        [Test]
        public void Constructor_WithLocalIndexAtChunkSize_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapChunkLocalCoord(MapChunkMetrics.ChunkSize, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapChunkLocalCoord(0, MapChunkMetrics.ChunkSize));
        }

        #endregion

        #region Equality

        [Test]
        public void Equals_WhenCoordinatesMatch_ReturnsTrue()
        {
            var a = new MapChunkLocalCoord(1, 2);
            var b = new MapChunkLocalCoord(1, 2);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void Equals_WhenCoordinatesDiffer_ReturnsFalse()
        {
            var a = new MapChunkLocalCoord(1, 2);
            var b = new MapChunkLocalCoord(2, 1);

            Assert.That(a.Equals(b), Is.False);
            Assert.That(a != b, Is.True);
        }

        #endregion
    }
}
