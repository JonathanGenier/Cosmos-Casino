using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal class MapManagerTests
    {
        #region FIELDS

        private MapManager _mapManager = null!;
        private MapCellCoord _coord;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _mapManager = new MapManager();
            _coord = new MapCellCoord(1, 1, 1);
        }

        #endregion

        #region HasFloor

        [Test]
        public void HasFloor_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var result = _mapManager.HasFloor(_coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager.HasFloor(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region TryPlaceFloor

        [Test]
        public void TryPlaceFloor_CreatesCell_WhenCellDoesNotExist()
        {
            // Arrange
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager.TryPlaceFloor(_coord);

            // Assert
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount + 1));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        [Test]
        public void TryPlaceFloor_PreservesExistingCell_WhenCellAlreadyExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager.TryPlaceFloor(_coord);

            // Assert
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        #endregion

        #region TryRemoveFloor

        [Test]
        public void TryRemoveFloor_ShouldSkip_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.TryRemoveFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveFloor_ShouldRemoveCell_WhenFloorIsRemoved()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            var result = _mapManager.TryRemoveFloor(_coord);

            // Assert
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount - 1));
            Assert.That(_mapManager.HasFloor(_coord), Is.False);
        }

        [Test]
        public void TryRemoveFloor_ShouldNotDeleteCell_WhenRemovalIsBlocked()
        {
            // Arrange
            PlaceWallWithFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager.TryRemoveFloor(_coord);

            // Assert
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        #endregion

        #region HasWall

        [Test]
        public void HasWall_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            PlaceWallWithFloor(_coord);

            // Act
            var result = _mapManager.HasWall(_coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasWall_ShouldReturnFalse_WhenCellExistsWithoutWall()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var result = _mapManager.HasWall(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasWall_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager.HasWall(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region TryPlaceWall

        [Test]
        public void TryPlaceWall_ShouldSkip_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.TryPlaceWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryPlaceWall_ShouldPlaceWall_WhenCellExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager.TryPlaceWall(_coord);

            // Assert
            Assert.That(_mapManager.HasWall(_coord), Is.True);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region TryRemoveWall

        [Test]
        public void TryRemoveWall_ShouldSkip_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.TryRemoveWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveWall_ShouldRemoveWall_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager.TryRemoveWall(_coord);

            // Assert
            Assert.That(_mapManager.HasWall(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryRemoveWall_ShouldNotMutate_WhenNoWallExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager.TryRemoveWall(_coord);

            // Assert
            Assert.That(_mapManager.HasWall(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region HELPERS

        private void PlaceWallWithFloor(MapCellCoord coord)
        {
            _mapManager.TryPlaceFloor(coord);
            _mapManager.TryPlaceWall(coord);
        }

        #endregion
    }
}