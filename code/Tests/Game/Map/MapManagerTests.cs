using CosmosCasino.Core.Game.Build.Domain;
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
        public void HasFloor_ShouldReturnTrue_WhenFloorExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var result = _mapManager.HasFloor(_coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenFloorDoesNotExists()
        {
            // Act
            var result = _mapManager.HasFloor(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region CanPlaceFloor

        [Test]
        public void CanPlaceFloor_ShouldReturnValid_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.CanPlaceFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanPlaceFloor_ReturnsNoOp_WhenFloorAlreadyExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var result = _mapManager.CanPlaceFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanPlaceFloor_MatchesTryPlaceFloorOutcome_WhenCellDoesNotExist()
        {
            // Act
            var preview = _mapManager.CanPlaceFloor(_coord);
            var commit = _mapManager.TryPlaceFloor(_coord);

            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(commit.Outcome));
        }

        #endregion

        #region TryPlaceFloor

        [Test]
        public void TryPlaceFloor_ShouldCreateCell_WhenCellDoesNotExist()
        {
            // Arrange
            int prevCellCount = _mapManager.CellCount;

            // Act
            var result = _mapManager.TryPlaceFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Cell, Is.EqualTo(_coord));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount + 1));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        [Test]
        public void TryPlaceFloor_ReturnsNoOp_WhenCellAlreadyExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            var result = _mapManager.TryPlaceFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.Cell, Is.EqualTo(_coord));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        [Test]
        public void TryPlaceFloor_ReevaluatesState_AndDoesNotTrustPreview()
        {
            // Arrange
            var preview = _mapManager.CanPlaceFloor(_coord);
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var commit = _mapManager.TryPlaceFloor(_coord);

            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(commit.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        #endregion

        #region CanRemoveFloor

        [Test]
        public void CanRemoveFloor_ShouldReturnNoOp_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.CanRemoveFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanRemoveFloor_ReturnsValid_WhenFloorExistsAndNotBlocked()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var result = _mapManager.CanRemoveFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanRemoveFloor_ReturnsInvalid_WhenRemovalIsBlockedByWall()
        {
            // Arrange
            PlaceWallWithFloor(_coord);

            // Act
            var result = _mapManager.CanRemoveFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
        }

        [Test]
        public void CanRemoveFloor_MatchesTryRemoveFloorOutcome_WhenCellDoesNotExist()
        {
            // Act
            var preview = _mapManager.CanRemoveFloor(_coord);
            var commit = _mapManager.TryRemoveFloor(_coord);

            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(commit.Outcome));
        }

        #endregion

        #region TryRemoveFloor

        [Test]
        public void TryRemoveFloor_ReturnsNoOp_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.TryRemoveFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
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
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
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
            var result = _mapManager.TryRemoveFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        [Test]
        public void TryRemoveFloor_ReevaluatesState_AndDoesNotTrustPreview()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            var preview = _mapManager.CanRemoveFloor(_coord);
            _mapManager.TryRemoveFloor(_coord);

            // Act
            var commit = _mapManager.TryRemoveFloor(_coord);

            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(commit.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        #endregion

        #region HasWall

        [Test]
        public void HasWall_ShouldReturnTrue_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor(_coord);

            // Act
            var result = _mapManager.HasWall(_coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasWall_ShouldReturnFalse_WhenFloorExistsWithoutWall()
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

        #region CanPlaceWall

        [Test]
        public void CanPlaceWall_ShouldReturnInvalid_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.CanPlaceWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
        }

        [Test]
        public void CanPlaceWall_ShouldReturnValid_WhenFloorExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var result = _mapManager.CanPlaceWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanPlaceWall_ShouldReturnNoOp_WhenWallAlreadyExists()
        {
            // Arrange
            PlaceWallWithFloor(_coord);

            // Act
            var result = _mapManager.CanPlaceWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanPlaceWall_MatchesTryPlaceWallOutcome_WhenCellDoesNotExist()
        {
            // Act
            var preview = _mapManager.CanPlaceWall(_coord);
            var commit = _mapManager.TryPlaceWall(_coord);

            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(commit.Outcome));
        }

        #endregion

        #region TryPlaceWall

        [Test]
        public void TryPlaceWall_ShouldReturnInvalid_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.TryPlaceWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryPlaceWall_ShouldReturnValid_WhenFloorExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            var result = _mapManager.TryPlaceWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
            Assert.That(_mapManager.HasWall(_coord), Is.True);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryPlaceWall_ShouldReturnNoOp_WhenWallAlreadyExists()
        {
            // Arrange
            PlaceWallWithFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            var result = _mapManager.TryPlaceWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
            Assert.That(_mapManager.HasWall(_coord), Is.True);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryPlaceWall_ReevaluatesState_AndDoesNotTrustPreview()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            var preview = _mapManager.CanPlaceWall(_coord);
            _mapManager.TryPlaceWall(_coord);

            // Act
            var commit = _mapManager.TryPlaceWall(_coord);

            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(commit.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        #endregion

        #region CanRemoveWall

        [Test]
        public void CanRemoveWall_ShouldReturnNoOp_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.CanRemoveWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void CanRemoveWall_ShouldReturnValid_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor(_coord);

            // Act
            var result = _mapManager.CanRemoveWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanRemoveWall_ShouldReturnNoOp_WhenNoWallExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);

            // Act
            var result = _mapManager.CanRemoveWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void CanRemoveWall_MatchesTryRemoveWallOutcome_WhenCellDoesNotExist()
        {
            // Act
            var preview = _mapManager.CanRemoveWall(_coord);
            var commit = _mapManager.TryRemoveWall(_coord);
            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(commit.Outcome));
        }

        #endregion

        #region TryRemoveWall

        [Test]
        public void TryRemoveWall_ShouldReturnNoOp_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager.TryRemoveWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveWall_ShouldReturnValid_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            var result = _mapManager.TryRemoveWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
            Assert.That(_mapManager.HasWall(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryRemoveWall_ShouldReturnNoOp_WhenNoWallExists()
        {
            // Arrange
            _mapManager.TryPlaceFloor(_coord);
            int prevCellCount = _mapManager.CellCount;

            // Act
            var result = _mapManager.TryRemoveWall(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
            Assert.That(_mapManager.HasWall(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryRemoveWall_ReevaluatesState_AndDoesNotTrustPreview()
        {
            // Arrange
            PlaceWallWithFloor(_coord);
            var preview = _mapManager.CanRemoveWall(_coord);
            _mapManager.TryRemoveWall(_coord);

            // Act
            var commit = _mapManager.TryRemoveWall(_coord);

            // Assert
            Assert.That(preview.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(commit.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
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