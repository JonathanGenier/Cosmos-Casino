using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal class GridTests
    {
        #region Initial State

        [Test]
        public void NewGrid_StartsEmpty()
        {
            // Arrange
            var grid = new Grid();

            // Act
            var count = grid.CellCount;
            var coords = grid.AllCoords;

            // Assert
            Assert.That(count, Is.EqualTo(0));
            Assert.That(coords, Is.Empty);
        }

        #endregion

        #region CreateCell

        [Test]
        public void CreateCell_NewCoord_AddsCell()
        {
            // Arrange
            var grid = new Grid();
            var coord = new MapCoord(0, 0);

            // Act
            grid.CreateCell(coord, FlatTile());

            // Assert
            Assert.That(grid.CellCount, Is.EqualTo(1));
            Assert.That(grid.GetCell(coord), Is.Not.Null);
        }

        [Test]
        public void CreateCell_DuplicateCoord_DoesNotCreateSecondCell()
        {
            // Arrange
            var grid = new Grid();
            var coord = new MapCoord(1, 2);

            // Act
            grid.CreateCell(coord, FlatTile());
            grid.CreateCell(coord, FlatTile());

            // Assert
            Assert.That(grid.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CreateCell_DuplicateCoord_DoesNotOverwriteExistingCell()
        {
            // Arrange
            var grid = new Grid();
            var coord = new MapCoord(3, 3);

            var firstTile = FlatTile();
            var secondTile = new TerrainTile(2f, 2f, 2f, 2f);

            // Act
            grid.CreateCell(coord, firstTile);
            var firstCell = grid.GetCell(coord);

            grid.CreateCell(coord, secondTile);
            var secondCell = grid.GetCell(coord);

            // Assert
            Assert.That(firstCell, Is.SameAs(secondCell));
        }

        [Test]
        public void CreateCell_MultipleDistinctCoords_AllStored()
        {
            // Arrange
            var grid = new Grid();

            // Act
            grid.CreateCell(new MapCoord(0, 0), FlatTile());
            grid.CreateCell(new MapCoord(1, 0), FlatTile());
            grid.CreateCell(new MapCoord(0, 1), FlatTile());

            // Assert
            Assert.That(grid.CellCount, Is.EqualTo(3));
        }

        #endregion

        #region GetCell

        [Test]
        public void GetCell_ExistingCoord_ReturnsCell()
        {
            // Arrange
            var grid = new Grid();
            var coord = new MapCoord(5, 5);
            grid.CreateCell(coord, FlatTile());

            // Act
            var cell = grid.GetCell(coord);

            // Assert
            Assert.That(cell, Is.Not.Null);
        }

        [Test]
        public void GetCell_MissingCoord_ReturnsNull()
        {
            // Arrange
            var grid = new Grid();

            // Act
            var cell = grid.GetCell(new MapCoord(99, 99));

            // Assert
            Assert.That(cell, Is.Null);
        }

        #endregion

        #region AllCoords

        [Test]
        public void AllCoords_ReturnsAllCreatedCoordinates()
        {
            // Arrange
            var grid = new Grid();
            var coords = new[]
            {
                new MapCoord(0, 0),
                new MapCoord(1, 1),
                new MapCoord(-1, -1),
            };

            foreach (var coord in coords)
            {
                grid.CreateCell(coord, FlatTile());
            }

            // Act
            var allCoords = grid.AllCoords.ToArray();

            // Assert
            Assert.That(allCoords.Length, Is.EqualTo(coords.Length));
            foreach (var coord in coords)
            {
                Assert.That(allCoords.Contains(coord), Is.True);
            }
        }

        [Test]
        public void AllCoords_IsLiveView_ReflectsNewCells()
        {
            // Arrange
            var grid = new Grid();
            var initialCount = grid.AllCoords.Count();

            // Act
            grid.CreateCell(new MapCoord(0, 0), FlatTile());

            // Assert
            Assert.That(grid.AllCoords.Count(), Is.EqualTo(initialCount + 1));
        }

        #endregion

        #region Stability / Integrity

        [Test]
        public void RepeatedCreateAndGet_DoesNotCorruptState()
        {
            // Arrange
            var grid = new Grid();
            var coord = new MapCoord(7, 7);

            // Act
            for (int i = 0; i < 10; i++)
            {
                grid.CreateCell(coord, FlatTile());
                var cell = grid.GetCell(coord);

                Assert.That(cell, Is.Not.Null);
            }

            // Assert
            Assert.That(grid.CellCount, Is.EqualTo(1));
        }

        #endregion

        #region Helper Methods

        private static TerrainTile FlatTile()
        {
            return new TerrainTile(1f, 1f, 1f, 1f);
        }

        #endregion
    }
}