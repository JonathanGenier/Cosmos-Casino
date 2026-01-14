using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal class MapCellTests
    {
        #region FIELDS

        private MapCell? _mapCell;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _mapCell = new();
        }

        #endregion

        #region MapCell

        [Test]
        public void MapCell_ShouldBeEmptyOnCreation()
        {
            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.True);
        }

        #endregion

        #region HasFloor

        [Test]
        public void HasFloor_ShouldReturnTrue_WhenFloorExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor();

            // Assert
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenFloorDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell!.HasFloor, Is.False);
        }

        #endregion

        #region HasWall

        [Test]
        public void HasWall_ShouldReturnTrue_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Assert
            Assert.That(_mapCell!.HasWall, Is.True);
        }

        [Test]
        public void HasWall_ShouldReturnFalse_WhenWallDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell!.HasWall, Is.False);
        }

        #endregion

        #region IsEmpty

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor();

            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenFloorAndWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_ShouldReturnTrue_WhenFloorAndWallDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.True);
        }

        #endregion

        #region TryPlaceFloor

        [Test]
        public void TryPlaceFloor_ShouldPlaceFloor_WhenFloorDoeNotExist()
        {
            // Act
            var result = _mapCell!.TryPlaceFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Placed));
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        [Test]
        public void TryPlaceFloor_ShouldFail_WhenFloorExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor();

            // Act
            var result = _mapCell!.TryPlaceFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        #endregion

        #region TryRemoveFloor

        [Test]
        public void TryRemoveFloor_ShouldRemoveFloor_WhenWallDoesNotExist()
        {
            // Arrange
            _mapCell!.TryPlaceFloor();

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Removed));
            Assert.That(_mapCell.HasFloor, Is.False);
        }

        [Test]
        public void TryRemoveFloor_ShouldFail_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell.HasFloor, Is.True);
        }

        [Test]
        public void TryRemoveFloor_ShouldSkip_WhenFloorDoesNotExist()
        {
            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoFloor));
            Assert.That(_mapCell.HasFloor, Is.False);
        }

        #endregion

        #region TryPlaceWall

        [Test]
        public void TryPlaceWall_ShouldPlaceWall_WhenFloorExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor();

            // Act
            var result = _mapCell!.TryPlaceWall();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Placed));
            Assert.That(_mapCell.HasWall, Is.True);
        }

        [Test]
        public void TryPlaceWall_ShouldFail_WhenFloorDoesNotExist()
        {
            // Act
            var result = _mapCell!.TryPlaceWall();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoFloor));
            Assert.That(_mapCell.HasWall, Is.False);
        }

        [Test]
        public void TryPlaceWall_ShouldSkip_WhenWallAlreadyExist()
        {
            // Arrange
            PlaceWallWithFloor();

            // Act
            var result = _mapCell!.TryPlaceWall();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell.HasWall, Is.True);
        }

        #endregion

        #region TryRemoveWall

        [Test]
        public void TryRemoveWall_ShouldRemoveWall_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Act
            var result = _mapCell!.TryRemoveWall();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Removed));
            Assert.That(_mapCell.HasWall, Is.False);
        }

        [Test]
        public void TryRemoveWall_ShouldSkip_WhenNoWallExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor();

            // Act
            var result = _mapCell!.TryRemoveWall();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoWall));
            Assert.That(_mapCell.HasWall, Is.False);
        }

        #endregion

        #region HELPERS

        private void PlaceWallWithFloor()
        {
            _mapCell!.TryPlaceFloor();
            _mapCell!.TryPlaceWall();
        }

        #endregion
    }
}