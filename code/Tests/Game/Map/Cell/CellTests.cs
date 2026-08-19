using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class CellTests
    {
        #region FIELDS

        private Cell _cell = null!;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _cell = new Cell(new MapCoord(0, 0), new TerrainTile(0f, 0f, 0f, 0f));
        }

        #endregion

        #region Floor Placement

        [Test]
        public void ValidatePlaceFloor_MissingLayer_ReturnsValidWithoutMutation()
        {
            var elevation = new Elevation(0);

            var first = _cell.ValidatePlaceFloor(elevation);
            var second = _cell.ValidatePlaceFloor(elevation);

            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(second.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_cell.HasFloorAt(elevation), Is.False);
        }

        [Test]
        public void PlaceFloor_ValidResult_PlacesFloorAtRequestedElevation()
        {
            var elevation = new Elevation(4);

            PlaceFloor(elevation);

            Assert.That(_cell.HasFloorAt(elevation), Is.True);
            Assert.That(_cell.HasFloorAt(new Elevation(0)), Is.False);
        }

        [Test]
        public void ValidatePlaceFloor_DuplicateAtSameElevation_ReturnsNoOp()
        {
            var elevation = new Elevation(2);
            PlaceFloor(elevation);

            var result = _cell.ValidatePlaceFloor(elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void PlaceFloor_AtDifferentElevations_CreatesIndependentContents()
        {
            var lower = new Elevation(2f);
            var upper = new Elevation(2.5f);

            PlaceFloor(lower);
            PlaceFloor(upper);

            Assert.That(_cell.HasFloorAt(lower), Is.True);
            Assert.That(_cell.HasFloorAt(upper), Is.True);
            Assert.That(_cell.HasFloorAt(new Elevation(3f)), Is.False);
        }

        [Test]
        public void PlaceFloor_NonValidResult_ThrowsWithoutChangingExistingFloor()
        {
            var elevation = new Elevation(0);
            PlaceFloor(elevation);
            var noOp = _cell.ValidatePlaceFloor(elevation);

            Assert.Throws<InvalidOperationException>(() =>
                _cell.PlaceFloor(noOp, new Floor(), elevation));
            Assert.That(_cell.HasFloorAt(elevation), Is.True);
        }

        #endregion

        #region Floor Removal

        [Test]
        public void ValidateRemoveFloor_MissingLayer_ReturnsNoOpWithoutMutation()
        {
            var elevation = new Elevation(0);

            var first = _cell.ValidateRemoveFloor(elevation);
            var second = _cell.ValidateRemoveFloor(elevation);

            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(second.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(_cell.HasFloorAt(elevation), Is.False);
        }

        [Test]
        public void RemoveFloor_ValidResult_RemovesOnlyRequestedElevation()
        {
            var lower = new Elevation(0);
            var upper = new Elevation(1);
            PlaceFloor(lower);
            PlaceFloor(upper);

            RemoveFloor(lower);

            Assert.That(_cell.HasFloorAt(lower), Is.False);
            Assert.That(_cell.HasFloorAt(upper), Is.True);
        }

        [Test]
        public void RemoveFloor_LastContent_ReturnsElevationToAbsentLayerBehavior()
        {
            var elevation = new Elevation(-3);
            PlaceFloor(elevation);

            RemoveFloor(elevation);

            Assert.That(_cell.HasFloorAt(elevation), Is.False);
            Assert.That(_cell.HasWallAt(elevation), Is.False);
            Assert.That(_cell.ValidatePlaceFloor(elevation).Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_cell.ValidatePlaceWall(elevation).FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
        }

        [Test]
        public void ValidateRemoveFloor_WallAtSameElevation_ReturnsBlockedWithoutMutation()
        {
            var elevation = new Elevation(0);
            PlaceFloorAndWall(elevation);

            var result = _cell.ValidateRemoveFloor(elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
            Assert.That(_cell.HasFloorAt(elevation), Is.True);
            Assert.That(_cell.HasWallAt(elevation), Is.True);
        }

        [Test]
        public void RemoveFloor_WallAtDifferentElevation_DoesNotBlockRemoval()
        {
            var lower = new Elevation(0);
            var upper = new Elevation(1);
            PlaceFloor(lower);
            PlaceFloorAndWall(upper);

            var validation = _cell.ValidateRemoveFloor(lower);
            _cell.RemoveFloor(validation, lower);

            Assert.That(validation.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_cell.HasFloorAt(lower), Is.False);
            Assert.That(_cell.HasFloorAt(upper), Is.True);
            Assert.That(_cell.HasWallAt(upper), Is.True);
        }

        [Test]
        public void RemoveFloor_NonValidResult_ThrowsWithoutMutation()
        {
            var elevation = new Elevation(0);
            PlaceFloorAndWall(elevation);
            var blocked = _cell.ValidateRemoveFloor(elevation);

            Assert.Throws<InvalidOperationException>(() => _cell.RemoveFloor(blocked, elevation));
            Assert.That(_cell.HasFloorAt(elevation), Is.True);
        }

        #endregion

        #region Wall Placement

        [Test]
        public void ValidatePlaceWall_MissingFloorAtElevation_ReturnsInvalid()
        {
            var elevation = new Elevation(1);

            var result = _cell.ValidatePlaceWall(elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
            Assert.That(_cell.HasWallAt(elevation), Is.False);
        }

        [Test]
        public void ValidatePlaceWall_FloorAtDifferentElevation_ReturnsInvalid()
        {
            PlaceFloor(new Elevation(0));

            var result = _cell.ValidatePlaceWall(new Elevation(1));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
        }

        [Test]
        public void PlaceWall_FloorAtSameElevation_PlacesWall()
        {
            var elevation = new Elevation(3);
            PlaceFloor(elevation);
            var validation = _cell.ValidatePlaceWall(elevation);

            _cell.PlaceWall(validation, new Wall(), elevation);

            Assert.That(validation.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_cell.HasWallAt(elevation), Is.True);
        }

        [Test]
        public void ValidatePlaceWall_DuplicateAtSameElevation_ReturnsNoOp()
        {
            var elevation = new Elevation(0);
            PlaceFloorAndWall(elevation);

            var result = _cell.ValidatePlaceWall(elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.None));
        }

        [Test]
        public void PlaceWall_NonValidResult_ThrowsWithoutMutation()
        {
            var elevation = new Elevation(0);
            var invalid = _cell.ValidatePlaceWall(elevation);

            Assert.Throws<InvalidOperationException>(() =>
                _cell.PlaceWall(invalid, new Wall(), elevation));
            Assert.That(_cell.HasWallAt(elevation), Is.False);
        }

        #endregion

        #region Wall Removal

        [Test]
        public void ValidateRemoveWall_MissingWall_ReturnsNoOpWithoutMutation()
        {
            var elevation = new Elevation(0);
            PlaceFloor(elevation);

            var first = _cell.ValidateRemoveWall(elevation);
            var second = _cell.ValidateRemoveWall(elevation);

            Assert.That(first.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(second.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(_cell.HasWallAt(elevation), Is.False);
        }

        [Test]
        public void RemoveWall_ValidResult_RemovesOnlyRequestedElevation()
        {
            var lower = new Elevation(0);
            var upper = new Elevation(1);
            PlaceFloorAndWall(lower);
            PlaceFloorAndWall(upper);

            RemoveWall(lower);

            Assert.That(_cell.HasWallAt(lower), Is.False);
            Assert.That(_cell.HasFloorAt(lower), Is.True);
            Assert.That(_cell.HasWallAt(upper), Is.True);
        }

        [Test]
        public void RemoveWall_NonValidResult_ThrowsWithoutMutation()
        {
            var elevation = new Elevation(0);
            PlaceFloor(elevation);
            var noOp = _cell.ValidateRemoveWall(elevation);

            Assert.Throws<InvalidOperationException>(() => _cell.RemoveWall(noOp, elevation));
            Assert.That(_cell.HasFloorAt(elevation), Is.True);
            Assert.That(_cell.HasWallAt(elevation), Is.False);
        }

        #endregion

        #region HELPERS

        private void PlaceFloor(Elevation elevation)
        {
            var validation = _cell.ValidatePlaceFloor(elevation);
            _cell.PlaceFloor(validation, new Floor(), elevation);
        }

        private void PlaceFloorAndWall(Elevation elevation)
        {
            PlaceFloor(elevation);
            var validation = _cell.ValidatePlaceWall(elevation);
            _cell.PlaceWall(validation, new Wall(), elevation);
        }

        private void RemoveFloor(Elevation elevation)
        {
            var validation = _cell.ValidateRemoveFloor(elevation);
            _cell.RemoveFloor(validation, elevation);
        }

        private void RemoveWall(Elevation elevation)
        {
            var validation = _cell.ValidateRemoveWall(elevation);
            _cell.RemoveWall(validation, elevation);
        }

        #endregion
    }
}
