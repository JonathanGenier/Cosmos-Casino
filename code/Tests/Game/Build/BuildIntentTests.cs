using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal class BuildIntentTests
    {
        #region BuildFloor

        [Test]
        public void BuildFloor_ShouldThrowArgumentNullException_WhenCellsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                BuildIntent.BuildFloor(null!));
        }

        [Test]
        public void BuildFloor_ShouldThrowArgumentException_WhenCellsIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                BuildIntent.BuildFloor(new List<MapCellCoord>()));
        }

        [Test]
        public void BuildFloor_ShouldCreateIntent_WithCorrectProperties()
        {
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(1, 2, 3)
            };

            var intent = BuildIntent.BuildFloor(cells);

            Assert.That(intent.Kind, Is.EqualTo(BuildKind.Floor));
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Place));
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
        }

        [Test]
        public void BuildFloor_ShouldCopyCells_Defensively()
        {
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(0, 0, 0)
            };

            var intent = BuildIntent.BuildFloor(cells);

            cells.Clear();

            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells, Is.Not.SameAs(cells));
        }

        #endregion
    }
}
