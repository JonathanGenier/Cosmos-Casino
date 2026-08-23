using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Systems;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Systems
{
    [TestFixture]
    internal sealed class CellSystemTests
    {
        #region Has API

        [Test]
        public void Has_WhenCellMissing_ReturnsFalse()
        {
            var system = new CellSystem();

            var hasFloor = system.Has(BuildKind.Floor, null);
            var hasWall = system.Has(BuildKind.Wall, null);

            Assert.That(hasFloor, Is.False);
            Assert.That(hasWall, Is.False);
        }

        [Test]
        public void Has_FloorAfterPlacement_ReturnsTrue()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloor(cell);

            bool result = system.Has(BuildKind.Floor, cell);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Has_WallAfterPlacement_ReturnsTrue()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloorAndWall(cell);

            bool result = system.Has(BuildKind.Wall, cell);

            Assert.That(result, Is.True);
        }

        #endregion

        #region CanPlace / CanRemove

        [Test]
        public void CanPlaceFloor_MissingCell_ReturnsValidWithoutCreatingState()
        {
            var system = new CellSystem();

            var result = system.CanPlace(BuildKind.Floor, Coord(), null);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
        }

        [Test]
        public void CanPlaceFloor_ExistingFloor_ReturnsNoOp()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloor(cell);

            var result = system.CanPlace(BuildKind.Floor, Coord(), cell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        [Test]
        public void CanPlaceWall_MissingCell_ReturnsNoFloorFailure()
        {
            var system = new CellSystem();

            var result = system.CanPlace(BuildKind.Wall, Coord(), null);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
        }

        [Test]
        public void CanPlaceWall_CellWithFloor_ReturnsValid()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloor(cell);

            var result = system.CanPlace(BuildKind.Wall, Coord(), cell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
        }

        [Test]
        public void CanRemoveFloor_MissingCell_ReturnsNoOp()
        {
            var system = new CellSystem();

            var result = system.CanRemove(BuildKind.Floor, Coord(), null);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        [Test]
        public void CanRemoveFloor_CellWithWall_ReturnsBlocked()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloorAndWall(cell);

            var result = system.CanRemove(BuildKind.Floor, Coord(), cell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
        }

        [Test]
        public void CanRemoveWall_MissingCell_ReturnsNoOp()
        {
            var system = new CellSystem();

            var result = system.CanRemove(BuildKind.Wall, Coord(), null);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        #endregion

        #region TryPlace / TryRemove

        [Test]
        public void TryPlaceFloor_MissingCell_CreatesOnlyAfterValidationSucceeds()
        {
            var system = new CellSystem();
            int createCalls = 0;
            Cell? createdCell = null;

            var result = system.TryPlace(
                BuildKind.Floor,
                Coord(),
                null,
                () =>
                {
                    createCalls++;
                    createdCell = CellAt();
                    return createdCell;
                });

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(createCalls, Is.EqualTo(1));
            Assert.That(createdCell, Is.Not.Null);
            Assert.That(createdCell!.HasFloor(), Is.True);
        }

        [Test]
        public void TryPlaceFloor_ExistingCell_DoesNotInvokeCreateDelegate()
        {
            var system = new CellSystem();
            var cell = CellAt();

            var result = system.TryPlace(
                BuildKind.Floor,
                Coord(),
                cell,
                ThrowingCreateCell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(cell.HasFloor(), Is.True);
        }

        [Test]
        public void TryPlaceFloor_ExistingFloor_ReturnsNoOpWithoutCreatingState()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloor(cell);

            var result = system.TryPlace(
                BuildKind.Floor,
                Coord(),
                cell,
                ThrowingCreateCell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(cell.HasFloor(), Is.True);
        }

        [Test]
        public void TryPlaceWall_MissingCell_ReturnsNoFloorAndDoesNotCreateState()
        {
            var system = new CellSystem();

            var result = system.TryPlace(
                BuildKind.Wall,
                Coord(),
                null,
                ThrowingCreateCell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
        }

        [Test]
        public void TryPlaceWall_CellWithFloor_PlacesWall()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloor(cell);

            var result = system.TryPlace(
                BuildKind.Wall,
                Coord(),
                cell,
                ThrowingCreateCell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(cell.HasWall(), Is.True);
        }

        [Test]
        public void TryRemoveFloor_MissingCell_ReturnsNoOp()
        {
            var system = new CellSystem();

            var result = system.TryRemove(BuildKind.Floor, Coord(), null);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        [Test]
        public void TryRemoveFloor_ExistingFloor_RemovesFloor()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloor(cell);

            var result = system.TryRemove(BuildKind.Floor, Coord(), cell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(cell.HasFloor(), Is.False);
            Assert.That(cell.IsEmpty, Is.True);
        }

        [Test]
        public void TryRemoveWall_MissingCell_ReturnsNoOp()
        {
            var system = new CellSystem();

            var result = system.TryRemove(BuildKind.Wall, Coord(), null);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
        }

        [Test]
        public void TryRemoveWall_ExistingWall_RemovesWallAndLeavesFloor()
        {
            var system = new CellSystem();
            var cell = CellAt();
            PlaceFloorAndWall(cell);

            var result = system.TryRemove(BuildKind.Wall, Coord(), cell);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(cell.HasWall(), Is.False);
            Assert.That(cell.HasFloor(), Is.True);
        }

        #endregion

        #region Unsupported BuildKind

        [Test]
        public void Has_UnsupportedBuildKind_Throws()
        {
            var system = new CellSystem();

            Assert.Throws<InvalidOperationException>(() =>
                system.Has((BuildKind)999, null));
        }

        [Test]
        public void CanPlace_UnsupportedBuildKind_Throws()
        {
            var system = new CellSystem();

            Assert.Throws<InvalidOperationException>(() =>
                system.CanPlace((BuildKind)999, Coord(), null));
        }

        [Test]
        public void CanRemove_UnsupportedBuildKind_Throws()
        {
            var system = new CellSystem();

            Assert.Throws<InvalidOperationException>(() =>
                system.CanRemove((BuildKind)999, Coord(), null));
        }

        [Test]
        public void TryPlace_UnsupportedBuildKind_ThrowsWithoutCreatingState()
        {
            var system = new CellSystem();

            Assert.Throws<InvalidOperationException>(() =>
                system.TryPlace((BuildKind)999, Coord(), null, ThrowingCreateCell));
        }

        [Test]
        public void TryRemove_UnsupportedBuildKind_Throws()
        {
            var system = new CellSystem();

            Assert.Throws<InvalidOperationException>(() =>
                system.TryRemove((BuildKind)999, Coord(), null));
        }

        #endregion

        #region Helpers

        private static MapCoord Coord(int x = 0, int y = 0)
        {
            return new MapCoord(x, y);
        }

        private static Cell CellAt(int y = 0)
        {
            return new Cell(new MapCellCoord(0, y, 0));
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

        private static Cell ThrowingCreateCell()
        {
            throw new InvalidOperationException("Cell creation was not expected.");
        }

        #endregion
    }
}
