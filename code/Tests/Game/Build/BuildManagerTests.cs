using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Cell;
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

            var intent = BuildIntent.BuildFloor(cells);

            // Act
            var result = _buildManager.ApplyBuildOperations(intent);

            // Assert
            Assert.That(result.Results, Has.Count.EqualTo(2));

            foreach (var cell in cells)
            {
                Assert.That(_mapManager.HasFloor(cell), Is.True);
            }
        }

        #endregion
    }
}
