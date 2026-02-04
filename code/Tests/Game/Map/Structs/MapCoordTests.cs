using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal class MapCoordTests
    {
        #region EQUALS

        [Test]
        public void Equals_ShouldReturnTrue_WhenCoordinatesMatch()
        {
            var a = new MapCoord(1, 2);
            var b = new MapCoord(1, 2);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenCoordinatesDiffer()
        {
            var a = new MapCoord(1, 2);
            var b = new MapCoord(3, 4);

            Assert.That(a.Equals(b), Is.False);
            Assert.That(a != b, Is.True);
        }

        #endregion

        #region EQUALS OBJECT

        [Test]
        public void EqualsObject_ShouldReturnTrue_WhenObjectIsSameCellCoord()
        {
            object a = new MapCoord(1, 2);
            object b = new MapCoord(1, 2);

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsObject_ShouldReturnFalse_WhenObjectIsDifferentType()
        {
            var coord = new MapCoord(1, 2);

            Assert.That(coord.Equals("not a coord"), Is.False);
        }

        #endregion

        #region HASHCODE

        [Test]
        public void GetHashCode_ShouldBeEqual_ForEqualCoordinates()
        {
            var a = new MapCoord(5, 6);
            var b = new MapCoord(5, 6);

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        #endregion

        #region DICTIONARY KEY

        [Test]
        public void CellCoord_ShouldWorkAsDictionaryKey()
        {
            var dict = new Dictionary<MapCoord, string>();
            var key1 = new MapCoord(1, 2);
            var key2 = new MapCoord(1, 2);

            dict[key1] = "value";

            Assert.That(dict.ContainsKey(key2), Is.True);
            Assert.That(dict[key2], Is.EqualTo("value"));
        }


        #endregion

        #region TOSTRING

        [Test]
        public void ToString_ShouldReturnFormattedCoordinates()
        {
            var coord = new MapCoord(1, 9);

            Assert.That(coord.ToString(), Is.EqualTo("(1, 9)"));
        }

        #endregion
    }
}