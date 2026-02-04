using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Systems;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Systems
{
    [TestFixture]
    internal class CellSystemTests
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

        #region ITerrainTileSink

        [Test]
        public void ReceiveTerrainTile_CreatesCell()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();

            // Act
            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());

            // Assert
            Assert.That(system.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void ReceiveTerrainTile_DuplicateCoord_DoesNotCreateDuplicate()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();

            // Act
            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());
            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());

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

            ((ITerrainTileSink)system).ReceiveTerrainTile(Coord(0, 0), FlatTile());
            ((ITerrainTileSink)system).ReceiveTerrainTile(Coord(1, 0), FlatTile());

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

            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());

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

            // Act
            var hasFloor = system.Has(BuildKind.Floor, Coord());
            var hasWall = system.Has(BuildKind.Wall, Coord());

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

            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());

            // Act
            system.TryPlace(BuildKind.Floor, coord);
            var result = system.Has(BuildKind.Floor, coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Has_WallAfterPlacement_ReturnsTrue()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();
            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());
            system.TryPlace(BuildKind.Floor, coord);

            // Act
            var placeResult = system.TryPlace(BuildKind.Wall, coord);
            var result = system.Has(BuildKind.Wall, coord);

            // Assert
            Assert.That(placeResult.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result, Is.True);
        }

        #endregion

        #region CanPlace / CanRemove – No Cell

        [Test]
        public void CanPlace_WhenNoCell_ReturnsNoCellFailure()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();

            // Act
            var result = system.CanPlace(BuildKind.Floor, coord);

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
            var result = system.CanRemove(BuildKind.Wall, Coord());

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
        }

        #endregion

        #region TryPlace / TryRemove – No Cell

        [Test]
        public void TryPlace_WhenNoCell_ReturnsNoCellFailure()
        {
            // Arrange
            var system = new CellSystem();

            // Act
            var result = system.TryPlace(BuildKind.Floor, Coord());

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
            var result = system.TryRemove(BuildKind.Wall, Coord());

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
                system.Has((BuildKind)999, Coord()));
        }

        [Test]
        public void CanPlace_UnsupportedBuildKind_Throws()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();
            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => system.CanPlace((BuildKind)999, coord));
        }

        [Test]
        public void TryPlace_UnsupportedBuildKind_Throws()
        {
            // Arrange
            var system = new CellSystem();
            var coord = Coord();
            ((ITerrainTileSink)system).ReceiveTerrainTile(coord, FlatTile());

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => system.TryPlace((BuildKind)999, coord));
        }

        #endregion

        #region Helpers

        private static TerrainTile FlatTile()
        {
            return new TerrainTile(1f, 1f, 1f, 1f);
        }


        private static MapCoord Coord(int x = 0, int y = 0)
        {
            return new MapCoord(x, y);
        }

        #endregion
    }
}