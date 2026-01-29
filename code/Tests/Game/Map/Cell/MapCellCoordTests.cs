using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Cell
{
    [TestFixture]
    internal class MapCellCoordTests
    {
        #region EQUALS

        [Test]
        public void Equals_ShouldReturnTrue_WhenCoordinatesMatch()
        {
            var a = new MapCellCoord(1, 2, 3);
            var b = new MapCellCoord(1, 2, 3);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenCoordinatesDiffer()
        {
            var a = new MapCellCoord(1, 2, 3);
            var b = new MapCellCoord(1, 2, 4);

            Assert.That(a.Equals(b), Is.False);
            Assert.That(a != b, Is.True);
        }

        #endregion

        #region EQUALS OBJECT

        [Test]
        public void EqualsObject_ShouldReturnTrue_WhenObjectIsSameCellCoord()
        {
            object a = new MapCellCoord(1, 2, 3);
            object b = new MapCellCoord(1, 2, 3);

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsObject_ShouldReturnFalse_WhenObjectIsDifferentType()
        {
            var coord = new MapCellCoord(1, 2, 3);

            Assert.That(coord.Equals("not a coord"), Is.False);
        }

        #endregion

        #region HASHCODE

        [Test]
        public void GetHashCode_ShouldBeEqual_ForEqualCoordinates()
        {
            var a = new MapCellCoord(5, 6, 7);
            var b = new MapCellCoord(5, 6, 7);

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        #endregion

        #region DICTIONARY KEY

        [Test]
        public void CellCoord_ShouldWorkAsDictionaryKey()
        {
            var dict = new Dictionary<MapCellCoord, string>();
            var key1 = new MapCellCoord(1, 2, 3);
            var key2 = new MapCellCoord(1, 2, 3);

            dict[key1] = "value";

            Assert.That(dict.ContainsKey(key2), Is.True);
            Assert.That(dict[key2], Is.EqualTo("value"));
        }


        #endregion

        #region TOSTRING

        [Test]
        public void ToString_ShouldReturnFormattedCoordinates()
        {
            var coord = new MapCellCoord(1, 2, 3);

            Assert.That(coord.ToString(), Is.EqualTo("(1, 2, 3)"));
        }

        #endregion
    }
}
