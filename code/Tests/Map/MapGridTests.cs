using CosmosCasino.Core.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Map
{
    [TestFixture]
    internal class MapGridTests
    {
        #region FIELDS

        private MapGrid? _mapGrid;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _mapGrid = new MapGrid();
        }

        #endregion

        #region GetCell

        [Test]
        public void GetCell_ShouldReturnCell_WhenCellExists()
        {
            // Arrange
            var cellCoord = new CellCoord(1, 1, 1);
            _mapGrid!.GetOrCreateCell(cellCoord);

            // Act
            var result = _mapGrid!.GetCell(cellCoord);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetCell_ShouldReturnNull_WhenCellDoesNotExists()
        {
            // Arrange
            var cellCoord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapGrid!.GetCell(cellCoord);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetOrCreateCell

        [Test]
        public void GetOrCreateCell_ShouldReturnExistingCell_WhenCellExists()
        {
            // Arrange
            var cellCoord = new CellCoord(1, 1, 1);
            var cell1 = _mapGrid!.GetOrCreateCell(cellCoord);

            // Act
            var cell2 = _mapGrid!.GetOrCreateCell(cellCoord);

            // Assert
            Assert.That(cell2, Is.SameAs(cell1));
        }

        [Test]
        public void GetOrCreateCell_ShouldCreateNewCell_WhenCellDoesNotExists()
        {
            // Arrange
            var cellCoord = new CellCoord(1, 1, 1);

            // Act
            var cell = _mapGrid!.GetOrCreateCell(cellCoord);

            // Assert
            Assert.That(cell, Is.Not.Null);
        }

        #endregion

        #region TryRemoveCell

        [Test]
        public void TryRemoveCell_ShouldReturnFalse_WhenCellDoesNotExist()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapGrid!.TryRemoveCell(coord);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryRemoveCell_ShouldReturnFalse_WhenCellIsNotEmpty()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            var cell = _mapGrid!.GetOrCreateCell(coord);
            cell.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapGrid!.TryRemoveCell(coord);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryRemoveCell_ShouldRemoveCellAndReturnTrue_WhenCellIsEmpty()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapGrid!.GetOrCreateCell(coord);

            // Act
            var result = _mapGrid!.TryRemoveCell(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapGrid!.GetCell(coord), Is.Null);
        }

        #endregion
    }
}
