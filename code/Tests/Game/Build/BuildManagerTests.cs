using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Floor;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Cell;
using CosmosCasino.Core.Game.Structure;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal class BuildManagerTests
    {
        #region Fields

        private MapManager _mapManager = null!;
        private BuildManager _buildManager = null!;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void Setup()
        {
            _mapManager = new MapManager();
            _buildManager = new BuildManager(_mapManager);
        }

        #endregion

        #region PlaceFloor

        [Test]
        public void ApplyBuildOperations_PlaceFloor_CreatesFloorOnAllCells()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0),
                new MapCellCoord(1, 0, 0),
            };

            var intent = BuildIntent.BuildFloor(cells, FloorType.Metal);

            // Act
            var result = _buildManager.ApplyBuildOperations(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(2));

            foreach (var cell in cells)
            {
                Assert.That(_mapManager.HasFloor(cell), Is.True);
                Assert.That(_mapManager.GetFloorType(cell), Is.EqualTo(FloorType.Metal));
            }
        }

        [Test]
        public void ApplyBuildOperations_PlaceFloor_ReplacesAllBlockedCells()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0),
                new MapCellCoord(1, 0, 0),
            };

            foreach (var cell in cells)
            {
                _mapManager.TryPlaceFloor(cell, FloorType.Carbon);
            }

            var intent = BuildIntent.BuildFloor(cells, FloorType.Metal);

            // Act
            var result = _buildManager.ApplyBuildOperations(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(cells.Length));
            Assert.That(result.Results.All(r => r.Outcome == BuildOperationOutcome.Replaced), Is.True);
            Assert.That(cells.All(c => _mapManager.GetFloorType(c) == FloorType.Metal), Is.True);
        }

        [Test]
        public void ApplyBuildOperations_ReplaceFloor_MixedOutcomes()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0), // place
                new MapCellCoord(1, 0, 0), // replace
                new MapCellCoord(2, 0, 0), // unchanged (skipped)
            };

            _mapManager.TryPlaceFloor(cells[1], FloorType.Carbon);
            _mapManager.TryPlaceFloor(cells[2], FloorType.Metal);

            var intent = BuildIntent.BuildFloor(cells, FloorType.Metal);

            // Act
            var result = _buildManager.ApplyBuildOperations(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(3));
            Assert.That(_mapManager.GetFloorType(cells[0]), Is.EqualTo(FloorType.Metal)); // placed
            Assert.That(_mapManager.GetFloorType(cells[1]), Is.EqualTo(FloorType.Metal)); // replaced
            Assert.That(_mapManager.GetFloorType(cells[2]), Is.EqualTo(FloorType.Metal)); // unchanged (skipped)
        }

        #endregion

        #region RemoveFloor

        [Test]
        public void ApplyBuildOperations_RemoveFloor_RemovesAllFloors()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0),
                new MapCellCoord(1, 0, 0),
                new MapCellCoord(2, 0, 0),
            };

            foreach (var cell in cells)
            {
                _mapManager.TryPlaceFloor(cell, FloorType.Metal);
            }

            var intent = BuildIntent.RemoveFloor(cells);

            // Act
            var result = _buildManager.ApplyBuildOperations(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(cells.Length));
            Assert.That(result.Results.All(r => r.Outcome == BuildOperationOutcome.Removed), Is.True);
            Assert.That(cells.All(c => _mapManager.HasFloor(c) == false), Is.True);
            Assert.That(cells.All(c => _mapManager.CellExists(c) == false), Is.True);
        }

        [Test]
        public void ApplyBuildOperations_RemoveFloor_MixedOutcomes()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0), // Removed
                new MapCellCoord(1, 0, 0), // Blocked
                new MapCellCoord(2, 0, 0), // No Cell
            };

            _mapManager.TryPlaceFloor(cells[0], FloorType.Metal);
            _mapManager.TryPlaceFloor(cells[1], FloorType.Metal);
            _mapManager.TryPlaceStructure(cells[1], StructureType.Wall);

            var intent = BuildIntent.RemoveFloor(cells);

            // Act
            var result = _buildManager.ApplyBuildOperations(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(3));
            Assert.That(result.Results[0].Outcome, Is.EqualTo(BuildOperationOutcome.Removed));
            Assert.That(result.Results[1].Outcome, Is.EqualTo(BuildOperationOutcome.Failed));
            Assert.That(result.Results[1].FailureReason, Is.EqualTo(BuildOperationFailureReason.Blocked));
            Assert.That(result.Results[2].Outcome, Is.EqualTo(BuildOperationOutcome.Failed));
            Assert.That(result.Results[2].FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
            Assert.That(_mapManager.CellExists(cells[0]), Is.False);
            Assert.That(_mapManager.HasFloor(cells[1]), Is.True);
            Assert.That(_mapManager.CellExists(cells[2]), Is.False);
        }

        #endregion
    }
}
