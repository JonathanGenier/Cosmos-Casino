using CosmosCasino.Core.Build;
using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Build
{
    [TestFixture]
    internal class BuildIntentTests
    {
        #region BuildFloor

        [Test]
        public void BuildFloor_ShouldThrowArgumentNullException_WhenCellsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                BuildIntent.BuildFloor(null!, FloorType.Metal));
        }

        [Test]
        public void BuildFloor_ShouldThrowArgumentException_WhenCellsIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                BuildIntent.BuildFloor(new List<MapCellCoord>(), FloorType.Metal));
        }

        [Test]
        public void BuildFloor_ShouldCreateIntent_WithCorrectProperties()
        {
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(1, 2, 3)
            };

            var intent = BuildIntent.BuildFloor(cells, FloorType.Metal);

            Assert.That(intent.Kind, Is.EqualTo(BuildKind.Floor));
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Place));
            Assert.That(intent.FloorType, Is.EqualTo(FloorType.Metal));
            Assert.That(intent.StructureType, Is.Null);
            Assert.That(intent.FurnitureType, Is.Null);
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
        }

        [Test]
        public void BuildFloor_ShouldCopyCells_Defensively()
        {
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(0, 0, 0)
            };

            var intent = BuildIntent.BuildFloor(cells, FloorType.Metal);

            cells.Clear();

            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells, Is.Not.SameAs(cells));
        }

        #endregion

        #region RemoveFloor

        [Test]
        public void RemoveFloor_ShouldThrowArgumentNullException_WhenCellsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                BuildIntent.RemoveFloor(null!));
        }

        [Test]
        public void RemoveFloor_ShouldThrowArgumentException_WhenCellsIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                BuildIntent.RemoveFloor(new List<MapCellCoord>()));
        }

        [Test]
        public void RemoveFloor_ShouldCreateIntent_WithCorrectProperties()
        {
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(1, 2, 3)
            };

            var intent = BuildIntent.RemoveFloor(cells);

            Assert.That(intent.Kind, Is.EqualTo(BuildKind.Floor));
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Remove));
            Assert.That(intent.FloorType, Is.Null);
            Assert.That(intent.StructureType, Is.Null);
            Assert.That(intent.FurnitureType, Is.Null);
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
        }

        [Test]
        public void RemoveFloor_ShouldCopyCells_Defensively()
        {
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(0, 0, 0)
            };

            var intent = BuildIntent.RemoveFloor(cells);

            cells.Clear();

            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells, Is.Not.SameAs(cells));
        }

        #endregion
    }
}
