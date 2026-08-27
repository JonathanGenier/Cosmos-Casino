using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;
using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class CellTests
    {
        #region Fields

        private Cell _cell = null!;

        #endregion

        #region Setup

        [SetUp]
        public void Setup()
        {
            _cell = CellAt(new Elevation(0));
        }

        #endregion

        #region Identity

        [Test]
        public void Constructor_AssignsGlobalCoordinateIdentity()
        {
            var coord = new MapCellCoord(-3, 7, 4);
            var cell = new Cell(coord);

            Assert.That(cell.Coord, Is.EqualTo(coord));
            Assert.That(cell.IsEmpty, Is.True);
        }

        #endregion

        #region Floor Placement

        [Test]
        public void ValidatePlaceFloor_MissingFloor_ReturnsValidWithoutMutation()
        {
            var first = _cell.ValidatePlaceFloor();
            var second = _cell.ValidatePlaceFloor();

            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(second.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_cell.HasFloor(), Is.False);
            Assert.That(_cell.IsEmpty, Is.True);
        }

        [Test]
        public void PlaceFloor_ValidResult_PlacesFloorInCell()
        {
            PlaceFloor(_cell);

            Assert.That(_cell.HasFloor(), Is.True);
            Assert.That(_cell.IsEmpty, Is.False);
        }

        [Test]
        public void HasFloorAt_NonMatchingElevation_ReturnsFalse()
        {
            var elevation = new Elevation(2f);
            var cell = CellAt(elevation);
            PlaceFloor(cell);

            Assert.That(cell.HasFloorAt(elevation), Is.True);
            Assert.That(cell.HasFloorAt(new Elevation(3f)), Is.False);
        }

        [Test]
        public void ValidatePlaceFloor_Duplicate_ReturnsNoOp()
        {
            PlaceFloor(_cell);

            var result = _cell.ValidatePlaceFloor();

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void PlaceFloor_NonValidResult_ThrowsWithoutChangingExistingFloor()
        {
            PlaceFloor(_cell);
            var noOp = _cell.ValidatePlaceFloor();

            Assert.Throws<InvalidOperationException>(() =>
                _cell.PlaceFloor(noOp, new Floor()));
            Assert.That(_cell.HasFloor(), Is.True);
        }

        #endregion

        #region Floor Removal

        [Test]
        public void ValidateRemoveFloor_MissingFloor_ReturnsNoOpWithoutMutation()
        {
            var first = _cell.ValidateRemoveFloor();
            var second = _cell.ValidateRemoveFloor();

            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(second.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(_cell.HasFloor(), Is.False);
            Assert.That(_cell.IsEmpty, Is.True);
        }

        [Test]
        public void RemoveFloor_ValidResult_RemovesFloorAndEmptiesCell()
        {
            PlaceFloor(_cell);

            RemoveFloor(_cell);

            Assert.That(_cell.HasFloor(), Is.False);
            Assert.That(_cell.HasWall(), Is.False);
            Assert.That(_cell.IsEmpty, Is.True);
            Assert.That(_cell.ValidatePlaceFloor().Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_cell.ValidatePlaceWall().FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
        }

        [Test]
        public void ValidateRemoveFloor_WallInCell_ReturnsBlockedWithoutMutation()
        {
            PlaceFloorAndWall(_cell);

            var result = _cell.ValidateRemoveFloor();

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
            Assert.That(_cell.HasFloor(), Is.True);
            Assert.That(_cell.HasWall(), Is.True);
        }

        [Test]
        public void RemoveFloor_NonValidResult_ThrowsWithoutMutation()
        {
            PlaceFloorAndWall(_cell);
            var blocked = _cell.ValidateRemoveFloor();

            Assert.Throws<InvalidOperationException>(() => _cell.RemoveFloor(blocked));
            Assert.That(_cell.HasFloor(), Is.True);
        }

        #endregion

        #region Wall Placement

        [Test]
        public void ValidatePlaceWall_MissingFloor_ReturnsInvalid()
        {
            var result = _cell.ValidatePlaceWall();

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
            Assert.That(_cell.HasWall(), Is.False);
        }

        [Test]
        public void PlaceWall_FloorInCell_PlacesWall()
        {
            PlaceFloor(_cell);
            var validation = _cell.ValidatePlaceWall();

            _cell.PlaceWall(validation, new Wall());

            Assert.That(validation.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_cell.HasWall(), Is.True);
        }

        [Test]
        public void HasWallAt_NonMatchingElevation_ReturnsFalse()
        {
            var elevation = new Elevation(-2f);
            var cell = CellAt(elevation);
            PlaceFloorAndWall(cell);

            Assert.That(cell.HasWallAt(elevation), Is.True);
            Assert.That(cell.HasWallAt(new Elevation(-1f)), Is.False);
        }

        [Test]
        public void ValidatePlaceWall_Duplicate_ReturnsNoOp()
        {
            PlaceFloorAndWall(_cell);

            var result = _cell.ValidatePlaceWall();

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void PlaceWall_NonValidResult_ThrowsWithoutMutation()
        {
            var invalid = _cell.ValidatePlaceWall();

            Assert.Throws<InvalidOperationException>(() =>
                _cell.PlaceWall(invalid, new Wall()));
            Assert.That(_cell.HasWall(), Is.False);
        }

        #endregion

        #region Wall Removal

        [Test]
        public void ValidateRemoveWall_MissingWall_ReturnsNoOpWithoutMutation()
        {
            PlaceFloor(_cell);

            var first = _cell.ValidateRemoveWall();
            var second = _cell.ValidateRemoveWall();

            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(second.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(_cell.HasFloor(), Is.True);
            Assert.That(_cell.HasWall(), Is.False);
        }

        [Test]
        public void RemoveWall_ValidResult_RemovesWallAndLeavesFloor()
        {
            PlaceFloorAndWall(_cell);

            RemoveWall(_cell);

            Assert.That(_cell.HasWall(), Is.False);
            Assert.That(_cell.HasFloor(), Is.True);
            Assert.That(_cell.IsEmpty, Is.False);
        }

        [Test]
        public void RemoveWall_NonValidResult_ThrowsWithoutMutation()
        {
            PlaceFloor(_cell);
            var noOp = _cell.ValidateRemoveWall();

            Assert.Throws<InvalidOperationException>(() => _cell.RemoveWall(noOp));
            Assert.That(_cell.HasFloor(), Is.True);
            Assert.That(_cell.HasWall(), Is.False);
        }

        #endregion

        #region Helpers

        private static Cell CellAt(Elevation elevation)
        {
            return new Cell(new MapCellCoord(0, elevation.MapCellY, 0));
        }

        private static void PlaceFloor(Cell cell)
        {
            var validation = cell.ValidatePlaceFloor();
            cell.PlaceFloor(validation, new Floor());
        }

        private static void PlaceFloorAndWall(Cell cell)
        {
            PlaceFloor(cell);
            var validation = cell.ValidatePlaceWall();
            cell.PlaceWall(validation, new Wall());
        }

        private static void RemoveFloor(Cell cell)
        {
            var validation = cell.ValidateRemoveFloor();
            cell.RemoveFloor(validation);
        }

        private static void RemoveWall(Cell cell)
        {
            var validation = cell.ValidateRemoveWall();
            cell.RemoveWall(validation);
        }

        #endregion
    }
}
