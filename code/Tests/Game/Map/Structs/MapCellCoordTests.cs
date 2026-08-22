using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapCellCoordTests
    {
        #region Semantics

        [Test]
        public void Constructor_AssignsXHorizontalYVerticalZHorizontal()
        {
            var coord = new MapCellCoord(x: -4, y: 12, z: 8);

            Assert.That(coord.X, Is.EqualTo(-4));
            Assert.That(coord.Y, Is.EqualTo(12));
            Assert.That(coord.Z, Is.EqualTo(8));
        }

        [Test]
        public void Constructor_AllowsNegativeXyzCoordinates()
        {
            var coord = new MapCellCoord(-1, -2, -3);

            Assert.That(coord.X, Is.EqualTo(-1));
            Assert.That(coord.Y, Is.EqualTo(-2));
            Assert.That(coord.Z, Is.EqualTo(-3));
        }

        #endregion

        #region Equality

        [Test]
        public void Equals_WhenCoordinatesMatch_ReturnsTrue()
        {
            var a = new MapCellCoord(1, -2, 3);
            var b = new MapCellCoord(1, -2, 3);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void Equals_WhenVerticalCoordinateDiffers_ReturnsFalse()
        {
            var a = new MapCellCoord(1, -2, 3);
            var b = new MapCellCoord(1, 99, 3);

            Assert.That(a.Equals(b), Is.False);
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void DictionaryLookup_WithEqualNegativeCoordinate_FindsValue()
        {
            var dictionary = new Dictionary<MapCellCoord, string>();
            var key = new MapCellCoord(-1, -2, -3);
            var equivalentKey = new MapCellCoord(-1, -2, -3);

            dictionary[key] = "cell";

            Assert.That(dictionary[equivalentKey], Is.EqualTo("cell"));
        }

        #endregion

        #region Formatting

        [Test]
        public void ToString_ReturnsXyzCoordinate()
        {
            var coord = new MapCellCoord(1, 2, 3);

            Assert.That(coord.ToString(), Is.EqualTo("(1, 2, 3)"));
        }

        #endregion
    }
}
