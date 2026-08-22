using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapChunkCoordTests
    {
        #region Equality

        [Test]
        public void Equals_WhenCoordinatesMatch_ReturnsTrue()
        {
            var a = new MapChunkCoord(-2, 3);
            var b = new MapChunkCoord(-2, 3);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void Equals_WhenCoordinatesDiffer_ReturnsFalse()
        {
            var a = new MapChunkCoord(-2, 3);
            var b = new MapChunkCoord(-2, 4);

            Assert.That(a.Equals(b), Is.False);
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void DictionaryLookup_WithEqualNegativeCoordinate_FindsValue()
        {
            var dictionary = new Dictionary<MapChunkCoord, string>();
            var key = new MapChunkCoord(-5, -7);
            var equivalentKey = new MapChunkCoord(-5, -7);

            dictionary[key] = "chunk";

            Assert.That(dictionary[equivalentKey], Is.EqualTo("chunk"));
        }

        #endregion

        #region Formatting

        [Test]
        public void ToString_ReturnsXzCoordinate()
        {
            var coord = new MapChunkCoord(-1, 2);

            Assert.That(coord.ToString(), Is.EqualTo("(-1, 2)"));
        }

        #endregion
    }
}
