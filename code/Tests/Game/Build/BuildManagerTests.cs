using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal sealed class BuildManagerTests
    {
        #region Fields

        private MapManager _mapManager = null!;
        private BuildManager _buildManager = null!;

        #endregion

        #region Setup

        [SetUp]
        public void Setup()
        {
            _mapManager = new MapManager();
            _buildManager = new BuildManager(_mapManager);
        }

        #endregion

        #region Evaluate Floor

        [Test]
        public void Evaluate_PlaceFloor_SingleCell_DoesNotMutateMap()
        {
            // Arrange
            var cells = CreateCells();
            var intent = BuildIntent.BuildFloor(cells);

            // Act
            var result = _buildManager.Evaluate(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(1));
            Assert.That(result.Results[0].Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(cells.All(c => _mapManager.HasFloor(c)), Is.False);
        }

        [Test]
        public void Evaluate_PlaceFloor_MultipleCells_DoesNotMutateMap()
        {
            // Arrange
            var cells = CreateCells(3);
            var intent = BuildIntent.BuildFloor(cells);

            // Act
            var result = _buildManager.Evaluate(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(cells.Count));
            Assert.That(result.Results.All(r => r.Outcome == BuildOperationOutcome.Valid), Is.True);
            Assert.That(cells.All(c => _mapManager.HasFloor(c)), Is.False);
        }

        [Test]
        public void Evaluate_PlaceFloor_AlreadyOccupied_ReturnsNoOp()
        {
            // Arrange
            var cells = CreateCells();
            PlaceFloor(cells);
            var intent = BuildIntent.BuildFloor(cells);

            // Act
            var result = _buildManager.Evaluate(intent);

            // Assert
            Assert.That(result.Results.Single().Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(cells.All(c => _mapManager.HasFloor(c)), Is.True);
        }

        #endregion

        #region Evaluate Wall

        [Test]
        public void Evaluate_PlaceWall_SingleCell_WithFloor_DoesNotMutateMap()
        {
            // Arrange
            var cells = CreateCells();
            PlaceFloor(cells);
            var intent = BuildIntent.BuildWall(cells);

            // Act
            var result = _buildManager.Evaluate(intent);

            // Assert
            Assert.That(result.Results.Single().Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapManager.HasWall(cells[0]), Is.False);
        }

        [Test]
        public void Evaluate_PlaceWall_WithoutFloor_ReturnsInvalid()
        {
            // Arrange
            var cells = CreateCells(2);
            var intent = BuildIntent.BuildWall(cells);

            // Act
            var result = _buildManager.Evaluate(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(cells.Count));
            Assert.That(result.Results.All(r => r.Outcome == BuildOperationOutcome.Invalid), Is.True);
            Assert.That(cells.All(c => _mapManager.HasWall(c)), Is.False);
        }

        #endregion

        #region Execute Floor

        [Test]
        public void Execute_PlaceFloor_SingleCell_CreatesFloor()
        {
            // Arrange
            var cells = CreateCells();
            var intent = BuildIntent.BuildFloor(cells);

            // Act
            var result = _buildManager.Execute(intent);

            // Assert
            Assert.That(result.Results.Single().Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapManager.HasFloor(cells[0]), Is.True);
        }

        [Test]
        public void Execute_PlaceFloor_MultipleCells_CreatesAllFloors()
        {
            // Arrange
            var cells = CreateCells(3);
            var intent = BuildIntent.BuildFloor(cells);

            // Act
            var result = _buildManager.Execute(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(cells.Count));
            Assert.That(result.Results.All(r => r.Outcome == BuildOperationOutcome.Valid), Is.True);
            Assert.That(cells.All(c => _mapManager.HasFloor(c)), Is.True);
        }

        [Test]
        public void Execute_PlaceFloor_DuplicateCells_AllowsPartialFailure()
        {
            // Arrange
            var cell = new MapCellCoord(0, 0, 0);
            var intent = BuildIntent.BuildFloor([cell, cell]);

            // Act
            var result = _buildManager.Execute(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(2));
            Assert.That(result.Results[0].Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(result.Results[1].Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(_mapManager.HasFloor(cell), Is.True);
        }

        #endregion

        #region Execute Wall

        [Test]
        public void Execute_PlaceWall_WithExistingFloor_CreatesWall()
        {
            // Arrange
            var cells = CreateCells();
            PlaceFloor(cells);
            var intent = BuildIntent.BuildWall(cells);

            // Act
            var result = _buildManager.Execute(intent);

            // Assert
            Assert.That(result.Results.Single().Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(_mapManager.HasWall(cells[0]), Is.True);
        }

        [Test]
        public void Execute_PlaceWall_WithoutFloor_ReturnsInvalid_AndDoesNotCreateWall()
        {
            // Arrange
            var cells = CreateCells(2);
            var intent = BuildIntent.BuildWall(cells);

            // Act
            var result = _buildManager.Execute(intent);

            // Assert
            Assert.That(result.Results.All(r => r.Outcome == BuildOperationOutcome.Invalid), Is.True);
            Assert.That(cells.All(c => !_mapManager.HasWall(c)), Is.True);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper method to place floor tiles directly via MapManager, bypassing BuildManager validation.
        /// This allows tests to set up specific map states without going through the system under test.
        /// </summary>
        /// <param name="cells">The collection of map cell coordinates where floor tiles should be placed.</param>
        /// <remarks>
        /// This method is used to arrange test scenarios by directly mutating the map state.
        /// Use this when you need to set up preconditions for testing BuildManager behavior.
        /// </remarks>
        internal void PlaceFloor(IReadOnlyList<MapCellCoord> cells)
        {
            foreach (var cell in cells)
            {
                _mapManager.TryPlaceFloor(cell);
            }
        }

        /// <summary>
        /// Helper method to create a collection of map cell coordinates for test arrangement.
        /// Creates cells in a horizontal line starting at (0, 0, 0) and incrementing along the X-axis.
        /// </summary>
        /// <param name="cellCount">The number of map cell coordinates to create. Defaults to 1. Values less than or equal to 0 will be treated as 1.</param>
        /// <returns>A read-only list of <see cref="MapCellCoord"/> instances positioned sequentially along the X-axis at Y=0, Z=0.</returns>
        internal IReadOnlyList<MapCellCoord> CreateCells(int cellCount = 1)
        {
            if (cellCount <= 0)
            {
                cellCount = 1;
            }

            var cells = new List<MapCellCoord>(cellCount);

            for (int i = 0; i < cellCount; i++)
            {
                cells.Add(new MapCellCoord(i, 0, 0));
            }

            return cells;
        }

        #endregion
    }
}