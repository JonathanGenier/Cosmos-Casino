using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Systems;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Systems
{
    [TestFixture]
    internal sealed class CellSystemTests
    {
        #region Initialization

        [Test]
        public void NewCellSystem_StartsWithNoCells()
        {
            // Arrange
            var system = new CellSystem();

            // Act
            var count = system.CellCount;

            // Assert
            Assert.That(count, Is.EqualTo(0));
        }

        #endregion

        #region Explicit Elevation Compatibility

        [Test]
        public void ExplicitElevationApis_TargetProvidedElevation()
        {
            var system = new CellSystem();
            var coord = Coord();
            var elevation = new Elevation(3f);
            system.CreateCell(coord);

            var result = system.TryPlace(BuildKind.Floor, coord, elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(system.TryGetCell(coord, out var cell), Is.True);
            Assert.That(cell!.HasFloorAt(elevation), Is.True);
            Assert.That(
                cell.HasFloorAt(new Elevation(elevation.Value + Elevation.StepSize)),
                Is.False);
            Assert.That(system.Has(BuildKind.Floor, coord, elevation), Is.True);
        }

        [Test]
        public void CanPlaceFloor_DoesNotCreateBuildableState()
        {
            var system = new CellSystem();
            var coord = Coord();
            var elevation = new Elevation(2f);
            system.CreateCell(coord);

            var result = system.CanPlace(BuildKind.Floor, coord, elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(system.TryGetCell(coord, out var cell), Is.True);
            Assert.That(cell!.HasFloorAt(elevation), Is.False);
            Assert.That(cell.ValidatePlaceFloor(elevation).Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
        }

        #endregion

        #region CreateCell

        [Test]
        public void CreateCell_CreatesCell()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();

            // Act
            system.CreateCell(coord);

            // Assert
            Assert.That(system.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CreateCell_DuplicateCoord_DoesNotCreateDuplicate()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();

            // Act
            system.CreateCell(coord);
            system.CreateCell(coord);

            // Assert
            Assert.That(system.CellCount, Is.EqualTo(1));
        }

        #endregion

        #region EnumerateAllCoords

        [Test]
        public void EnumerateAllCoords_ReturnsAllExistingCoords()
        {
            // Arrange
            var system = new CellSystem();

            system.CreateCell(new MapCoord(0, 0));
            system.CreateCell(new MapCoord(1, 0));

            // Act
            var coords = system.EnumerateAllCoords();

            // Assert
            Assert.That(coords, Has.Exactly(2).Items);
        }

        #endregion

        #region TryGetCell

        [Test]
        public void TryGetCell_WhenCellExists_ReturnsTrue()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();

            system.CreateCell(coord);

            // Act
            var result = system.TryGetCell(coord, out var cell);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(cell, Is.Not.Null);
        }

        [Test]
        public void TryGetCell_WhenCellDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var system = new CellSystem();

            // Act
            var result = system.TryGetCell(Coord(), out var cell);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(cell, Is.Null);
        }

        #endregion

        #region Has API

        [Test]
        public void Has_WhenCellMissing_ReturnsFalse()
        {
            // Arrange
            var system = new CellSystem();
            var elevation = new Elevation(1f);

            // Act
            var hasFloor = system.Has(BuildKind.Floor, Coord(), elevation);
            var hasWall = system.Has(BuildKind.Wall, Coord(), elevation);

            // Assert
            Assert.That(hasFloor, Is.False);
            Assert.That(hasWall, Is.False);
        }

        [Test]
        public void Has_FloorAfterPlacement_ReturnsTrue()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();
            var elevation = new Elevation(1f);

            system.CreateCell(coord);

            // Act
            system.TryPlace(BuildKind.Floor, coord, elevation);
            var result = system.Has(BuildKind.Floor, coord, elevation);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Has_WallAfterPlacement_ReturnsTrue()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();
            var elevation = new Elevation(1f);
            system.CreateCell(coord);
            system.TryPlace(BuildKind.Floor, coord, elevation);

            // Act
            var placeResult = system.TryPlace(BuildKind.Wall, coord, elevation);
            var result = system.Has(BuildKind.Wall, coord, elevation);

            // Assert
            Assert.That(placeResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result, Is.True);
        }

        #endregion

        #region CanPlace / CanRemove - No Cell

        [Test]
        public void CanPlace_WhenNoCell_ReturnsNoCellFailure()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();

            // Act
            var result = system.CanPlace(BuildKind.Floor, coord, new Elevation(1f));

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
        }

        [Test]
        public void CanRemove_WhenNoCell_ReturnsNoCellFailure()
        {
            // Arrange
            var system = new CellSystem();

            // Act
            var result = system.CanRemove(BuildKind.Wall, Coord(), new Elevation(1f));

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
        }

        #endregion

        #region TryPlace / TryRemove - No Cell

        [Test]
        public void TryPlace_WhenNoCell_ReturnsNoCellFailure()
        {
            // Arrange
            var system = new CellSystem();

            // Act
            var result = system.TryPlace(BuildKind.Floor, Coord(), new Elevation(1f));

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
        }

        [Test]
        public void TryRemove_WhenNoCell_ReturnsNoCellFailure()
        {
            // Arrange
            var system = new CellSystem();

            // Act
            var result = system.TryRemove(BuildKind.Wall, Coord(), new Elevation(1f));

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
        }

        #endregion

        #region Unsupported BuildKind

        [Test]
        public void Has_UnsupportedBuildKind_Throws()
        {
            // Arrange
            var system = new CellSystem();

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() =>
                system.Has((BuildKind)999, Coord(), new Elevation(1f)));
        }

        [Test]
        public void CanPlace_UnsupportedBuildKind_Throws()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();
            system.CreateCell(coord);

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() =>
                system.CanPlace((BuildKind)999, coord, new Elevation(1f)));
        }

        [Test]
        public void TryPlace_UnsupportedBuildKind_Throws()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();
            system.CreateCell(coord);

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() =>
                system.TryPlace((BuildKind)999, coord, new Elevation(1f)));
        }

        #endregion

        #region Helpers

        private static MapCoord Coord(int x = 0, int y = 0)
        {
            return new MapCoord(x, y);
        }

        #endregion
    }
}
