using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Cell
{
    [TestFixture]
    internal class MapCellTests
    {
        #region FIELDS

        private MapCell _mapCell = null!;

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
            Assert.That(_mapCell.IsEmpty, Is.True);
        }

        #endregion

        #region HasFloor

        [Test]
        public void HasFloor_ShouldReturnTrue_WhenFloorExists()
        {
            // Arrange
            var validationResult = _mapCell.ValidatePlaceFloor();
            _mapCell.PlaceFloor(validationResult);

            // Assert
            Assert.That(_mapCell.HasFloor, Is.True);
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenFloorDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell.HasFloor, Is.False);
        }

        #endregion

        #region HasWall

        [Test]
        public void HasWall_ShouldReturnTrue_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Assert
            Assert.That(_mapCell.HasWall, Is.True);
        }

        [Test]
        public void HasWall_ShouldReturnFalse_WhenWallDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell.HasWall, Is.False);
        }

        #endregion

        #region IsEmpty

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenFloorOnlyExists()
        {
            // Arrange
            PlaceFloor();

            // Assert
            Assert.That(_mapCell.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenFloorAndWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Assert
            Assert.That(_mapCell.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_ShouldReturnTrue_WhenFloorAndWallDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell.IsEmpty, Is.True);
        }

        #endregion

        #region ValidatePlaceFloor

        [Test]
        public void ValidatePlaceFloor_ShouldReturnValid_WhenFloorDoesNotExist()
        {
            // Act
            var validResult = _mapCell.ValidatePlaceFloor();

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(validResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidatePlaceFloor_ShouldReturnNoOp_WhenFloorExist()
        {
            // Arrange
            PlaceFloor();

            // Act
            var noOpResult = _mapCell.ValidatePlaceFloor();

            // Assert
            Assert.That(noOpResult.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(noOpResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidatePlaceFloor_Is_Idempotent_WhenStateDoesNotChange()
        {
            var r1 = _mapCell.ValidatePlaceFloor();
            var r2 = _mapCell.ValidatePlaceFloor();

            Assert.That(r1.Outcome, Is.EqualTo(r2.Outcome));
            Assert.That(r1.FailureReason, Is.EqualTo(r2.FailureReason));
        }

        #endregion

        #region PlaceFloor

        [Test]
        public void PlaceFloor_ShouldThrow_WhenValidationIsNoOp()
        {
            // Arrange
            PlaceFloor();
            var noOpResult = _mapCell.ValidatePlaceFloor();

            // Assert
            Assert.That(noOpResult.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(() => _mapCell.PlaceFloor(noOpResult), Throws.InvalidOperationException);
            Assert.That(_mapCell.HasFloor, Is.True);
        }

        [Test]
        public void PlaceFloor_DoesNotMutate_WhenValidationIsNoOp()
        {
            // Arrange
            PlaceFloor();

            // Act
            var noOpResult = _mapCell.ValidatePlaceFloor();

            // Assert
            Assert.That(noOpResult.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(() => _mapCell.PlaceFloor(noOpResult), Throws.InvalidOperationException);
            Assert.That(_mapCell.HasFloor, Is.True);
        }

        [Test]
        public void PlaceFloor_ShouldMutate_WhenValid()
        {
            // Arrange
            var validResult = _mapCell.ValidatePlaceFloor();

            // Act
            _mapCell.PlaceFloor(validResult);

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        #endregion

        #region ValidateRemoveFloor

        [Test]
        public void ValidateRemoveFloor_ShouldReturnValid_WhenFloorExistsAndNoWall()
        {
            // Arrange
            PlaceFloor();

            // Act
            var validResult = _mapCell.ValidateRemoveFloor();

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(validResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidateRemoveFloor_ShouldReturnBlocked_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Act
            var result = _mapCell.ValidateRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
        }

        [Test]
        public void ValidateRemoveFloor_ShouldReturnNoOp_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell.ValidateRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidateRemoveFloor_Is_Idempotent_WhenStateDoesNotChange()
        {
            // Arrange
            var r1 = _mapCell.ValidateRemoveFloor();
            var r2 = _mapCell.ValidateRemoveFloor();

            // Assert
            Assert.That(r1.Outcome, Is.EqualTo(r2.Outcome));
            Assert.That(r1.FailureReason, Is.EqualTo(r2.FailureReason));
        }

        #endregion

        #region RemoveFloor

        [Test]
        public void RemoveFloor_ShouldThrow_WhenIsNotValid()
        {
            // Arrange
            PlaceWallWithFloor();

            // Act
            var invalidResult = _mapCell.ValidateRemoveFloor();

            // Assert
            Assert.That(invalidResult.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(() => _mapCell.RemoveFloor(invalidResult), Throws.InvalidOperationException);
            Assert.That(_mapCell.HasFloor, Is.True);
        }

        [Test]
        public void RemoveFloor_ShouldMutate_WhenValid()
        {
            // Arrange
            PlaceFloor();

            // Act
            var validResult = _mapCell.ValidateRemoveFloor();
            _mapCell.RemoveFloor(validResult);

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapCell.HasFloor, Is.False);
        }

        [Test]
        public void RemoveFloor_DoesNotChangeValidationOutcome_WhenBlocked()
        {
            // Arrange
            PlaceWallWithFloor();

            // Assert
            var before = _mapCell.ValidateRemoveFloor();
            Assert.That(() => _mapCell.RemoveFloor(before), Throws.InvalidOperationException);

            var after = _mapCell.ValidateRemoveFloor();
            Assert.That(after.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(after.FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
        }

        #endregion

        #region ValidatePlaceWall

        [Test]
        public void ValidatePlaceWall_ShouldReturnValid_WhenFloorExistsAndNoWall()
        {
            // Arrange
            PlaceFloor();

            // Act
            var validResult = _mapCell.ValidatePlaceWall();

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(validResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidatePlaceWall_ShouldReturnNoOp_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Act
            var noOpResult = _mapCell.ValidatePlaceWall();

            // Assert
            Assert.That(noOpResult.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(noOpResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidatePlaceWall_ShouldReturnInvalid_WhenNoFloorExists()
        {
            // Act
            var invalidResult = _mapCell.ValidatePlaceWall();

            // Assert
            Assert.That(invalidResult.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(invalidResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
        }

        [Test]
        public void ValidatePlaceWall_Is_Idempotent_WhenStateDoesNotChange()
        {
            // Arrange
            PlaceFloor();
            var r1 = _mapCell.ValidatePlaceWall();
            var r2 = _mapCell.ValidatePlaceWall();

            // Assert
            Assert.That(r1.Outcome, Is.EqualTo(r2.Outcome));
            Assert.That(r1.FailureReason, Is.EqualTo(r2.FailureReason));
        }

        #endregion

        #region PlaceWall

        [Test]
        public void PlaceWall_ShouldThrow_WhenIsNotValid()
        {
            // Arrange
            var invalidResult = _mapCell.ValidatePlaceWall();

            // Assert
            Assert.That(invalidResult.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(() => _mapCell.PlaceWall(invalidResult), Throws.InvalidOperationException);
            Assert.That(_mapCell.HasWall, Is.False);
        }

        [Test]
        public void PlaceWall_ShouldThrow_WhenNoOp()
        {
            // Arrange
            PlaceWallWithFloor();
            var noOpResult = _mapCell.ValidatePlaceWall();

            // Act / Assert
            Assert.That(noOpResult.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(() => _mapCell.PlaceWall(noOpResult), Throws.InvalidOperationException);
            Assert.That(_mapCell.HasWall, Is.True);
        }

        [Test]
        public void PlaceWall_ShouldMutate_WhenValid()
        {
            // Arrange
            PlaceFloor();
            var validResult = _mapCell.ValidatePlaceWall();

            // Act
            _mapCell.PlaceWall(validResult);

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapCell.HasWall, Is.True);
        }

        #endregion

        #region ValidateRemoveWall

        [Test]
        public void ValidateRemoveWall_ShouldReturnValid_WhenWallExists()
        {
            // Arrange
            PlaceWallWithFloor();

            // Act
            var validResult = _mapCell.ValidateRemoveWall();

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(validResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidateRemoveWall_ShouldReturnNoOp_WhenNoWallExists()
        {
            // Arrange
            PlaceFloor();

            // Act
            var noOpResult = _mapCell.ValidateRemoveWall();

            // Assert
            Assert.That(noOpResult.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(noOpResult.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void ValidateRemoveWall_Is_Idempotent_WhenStateDoesNotChange()
        {
            // Arrange
            var r1 = _mapCell.ValidateRemoveWall();
            var r2 = _mapCell.ValidateRemoveWall();

            // Assert
            Assert.That(r1.Outcome, Is.EqualTo(r2.Outcome));
            Assert.That(r1.FailureReason, Is.EqualTo(r2.FailureReason));
        }

        #endregion

        #region RemoveWall

        [Test]
        public void RemoveWall_ShouldThrow_WhenValidationIsNoOp()
        {
            // Arrange
            PlaceFloor();
            var noOpResult = _mapCell.ValidateRemoveWall();

            // Assert
            Assert.That(noOpResult.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(() => _mapCell.RemoveWall(noOpResult), Throws.InvalidOperationException);
            Assert.That(_mapCell.HasWall, Is.False);
        }

        [Test]
        public void RemoveWall_ShouldMutate_WhenValid()
        {
            // Arrange
            PlaceWallWithFloor();
            var validResult = _mapCell.ValidateRemoveWall();

            // Act
            _mapCell.RemoveWall(validResult);

            // Assert
            Assert.That(validResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapCell.HasWall, Is.False);
        }

        #endregion

        #region HELPERS

        private void PlaceFloor()
        {
            var floorValidationResult = _mapCell.ValidatePlaceFloor();
            _mapCell.PlaceFloor(floorValidationResult);
        }

        private void PlaceWallWithFloor()
        {
            var floorValidationResult = _mapCell.ValidatePlaceFloor();
            _mapCell.PlaceFloor(floorValidationResult);

            var wallValidationResult = _mapCell.ValidatePlaceWall();
            _mapCell.PlaceWall(wallValidationResult);
        }

        #endregion
    }
}