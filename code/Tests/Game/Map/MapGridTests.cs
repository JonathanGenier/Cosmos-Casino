using CosmosCasino.Core.Game.Floor;
using CosmosCasino.Core.Game.Map.Cell;
using CosmosCasino.Core.Game.Map.Grid;
using NUnit.Framework;
using System.Numerics;

namespace CosmosCasino.Tests.Game.Map
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
            var cellCoord = new MapCellCoord(1, 1, 1);
            _mapGrid!.GetOrCreateCell(cellCoord);

            // Act
            var result = _mapGrid!.GetCell(cellCoord);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetCell_ShouldReturnNull_WhenCellDoesNotExist()
        {
            // Arrange
            var cellCoord = new MapCellCoord(1, 1, 1);

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
            var cellCoord = new MapCellCoord(1, 1, 1);
            var cell1 = _mapGrid!.GetOrCreateCell(cellCoord);

            // Act
            var cell2 = _mapGrid!.GetOrCreateCell(cellCoord);

            // Assert
            Assert.That(cell2, Is.SameAs(cell1));
        }

        [Test]
        public void GetOrCreateCell_ShouldCreateNewCell_WhenCellDoesNotExist()
        {
            // Arrange
            var cellCoord = new MapCellCoord(1, 1, 1);

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
            var coord = new MapCellCoord(1, 1, 1);

            // Act
            var result = _mapGrid!.TryRemoveCell(coord);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryRemoveCell_ShouldReturnFalse_WhenCellIsNotEmpty()
        {
            // Arrange
            var coord = new MapCellCoord(1, 1, 1);
            var cell = _mapGrid!.GetOrCreateCell(coord);
            cell.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapGrid!.TryRemoveCell(coord);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryRemoveCell_ShouldRemoveCellAndReturnTrue_WhenCellIsEmpty()
        {
            // Arrange
            var coord = new MapCellCoord(1, 1, 1);
            _mapGrid!.GetOrCreateCell(coord);

            // Act
            var result = _mapGrid!.TryRemoveCell(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapGrid!.GetCell(coord), Is.Null);
        }

        #endregion

        #region WorldToCell

        [Test]
        public void WorldToCell_AtOrigin_ReturnsZeroCell()
        {
            MapCellCoord cell = MapGrid.WorldToCell(0f, 0f, 0f);

            Assert.That(cell.X, Is.EqualTo(0));
            Assert.That(cell.Y, Is.EqualTo(0));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_WithinSameCell_ReturnsSameCell()
        {
            MapCellCoord cell = MapGrid.WorldToCell(0.1f, 0f, 0.9f);

            Assert.That(cell.X, Is.EqualTo(0));
            Assert.That(cell.Y, Is.EqualTo(0));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_OnPositiveBoundary_SnapsToNextCell()
        {
            MapCellCoord cell = MapGrid.WorldToCell(1.0f, 0f, 1.0f);

            Assert.That(cell.X, Is.EqualTo(1));
            Assert.That(cell.Y, Is.EqualTo(1));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_NegativeCoordinates_SnapCorrectly()
        {
            MapCellCoord cell = MapGrid.WorldToCell(-0.1f, 0f, -0.1f);

            Assert.That(cell.X, Is.EqualTo(-1));
            Assert.That(cell.Y, Is.EqualTo(-1));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_IgnoresWorldY()
        {
            MapCellCoord cellLow = MapGrid.WorldToCell(0.5f, 0f, 0.5f);
            MapCellCoord cellHigh = MapGrid.WorldToCell(0.5f, 999f, 0.5f);

            Assert.That(cellLow, Is.EqualTo(cellHigh));
        }

        #endregion

        #region Round-trip consistency

        [Test]
        public void WorldToCell_Then_CellToWorld_ReturnsSameCellCenter()
        {
            float worldX = 4.25f;
            float worldZ = 7.9f;

            MapCellCoord cell = MapGrid.WorldToCell(worldX, 0f, worldZ);
            Vector3 worldCenter = MapGrid.CellToWorld(cell);

            Assert.That(worldCenter.X, Is.EqualTo(cell.X + 0.5f));
            Assert.That(worldCenter.Z, Is.EqualTo(cell.Y + 0.5f));
            Assert.That(worldCenter.Y, Is.EqualTo(0f));
        }

        #endregion
    }
}
