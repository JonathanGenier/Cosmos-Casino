using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal sealed class BuildIntentTests
    {
        #region BuildFloor

        [Test]
        public void BuildFloor_NullCells_ThrowsArgumentNullException()
        {
            // Arrange / Act / Assert
            Assert.Throws<ArgumentNullException>(() =>
                BuildIntent.PlaceFloor(null!));
        }

        [Test]
        public void BuildFloor_EmptyCells_ThrowsArgumentException()
        {
            // Arrange
            var cells = new List<MapCellCoord>();

            // Act / Assert
            Assert.Throws<ArgumentException>(() =>
                BuildIntent.PlaceFloor(cells));
        }

        [Test]
        public void BuildFloor_ValidCells_CreatesCorrectIntent()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(1, 2, 3)
            };

            // Act
            var intent = BuildIntent.PlaceFloor(cells);

            // Assert
            Assert.That(intent.Kind, Is.EqualTo(BuildKind.Floor));
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Place));
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells[0], Is.EqualTo(cells[0]));
        }

        [Test]
        public void BuildFloor_CopiesCells_Defensively()
        {
            // Arrange
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(0, 0, 0)
            };

            // Act
            var intent = BuildIntent.PlaceFloor(cells);
            cells.Clear();

            // Assert
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells, Is.Not.SameAs(cells));
        }

        #endregion

        #region RemoveFloor

        [Test]
        public void RemoveFloor_NullCells_ThrowsArgumentNullException()
        {
            // Arrange / Act / Assert
            Assert.Throws<ArgumentNullException>(() => BuildIntent.RemoveFloor(null!));
        }

        [Test]
        public void RemoveFloor_EmptyCells_ThrowsArgumentException()
        {
            // Arrange
            var cells = new List<MapCellCoord>();

            // Act / Assert
            Assert.Throws<ArgumentException>(() => BuildIntent.RemoveFloor(cells));
        }

        [Test]
        public void RemoveFloor_ValidCells_CreatesCorrectIntent()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(1, 1, 1)
            };

            // Act
            var intent = BuildIntent.RemoveFloor(cells);

            // Assert
            Assert.That(intent.Kind, Is.EqualTo(BuildKind.Floor));
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Remove));
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells[0], Is.EqualTo(cells[0]));
        }

        [Test]
        public void RemoveFloor_CopiesCells_Defensively()
        {
            // Arrange
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(3, 3, 3)
            };

            // Act
            var intent = BuildIntent.RemoveFloor(cells);
            cells.Clear();

            // Assert
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells, Is.Not.SameAs(cells));
        }

        #endregion

        #region BuildWall

        [Test]
        public void BuildWall_NullCells_ThrowsArgumentNullException()
        {
            // Arrange / Act / Assert
            Assert.Throws<ArgumentNullException>(() =>
                BuildIntent.PlaceWall(null!));
        }

        [Test]
        public void BuildWall_EmptyCells_ThrowsArgumentException()
        {
            // Arrange
            var cells = new List<MapCellCoord>();

            // Act / Assert
            Assert.Throws<ArgumentException>(() =>
                BuildIntent.PlaceWall(cells));
        }

        [Test]
        public void BuildWall_ValidCells_CreatesCorrectIntent()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(5, 0, 1)
            };

            // Act
            var intent = BuildIntent.PlaceWall(cells);

            // Assert
            Assert.That(intent.Kind, Is.EqualTo(BuildKind.Wall));
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Place));
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells[0], Is.EqualTo(cells[0]));
        }

        [Test]
        public void BuildWall_CopiesCells_Defensively()
        {
            // Arrange
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(2, 0, 0)
            };

            // Act
            var intent = BuildIntent.PlaceWall(cells);
            cells.Clear();

            // Assert
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells, Is.Not.SameAs(cells));
        }

        #endregion

        #region RemoveWall

        [Test]
        public void RemoveWall_NullCells_ThrowsArgumentNullException()
        {
            // Arrange / Act / Assert
            Assert.Throws<ArgumentNullException>(() => BuildIntent.RemoveWall(null!));
        }

        [Test]
        public void RemoveWall_EmptyCells_ThrowsArgumentException()
        {
            // Arrange
            var cells = new List<MapCellCoord>();

            // Act / Assert
            Assert.Throws<ArgumentException>(() => BuildIntent.RemoveWall(cells));
        }

        [Test]
        public void RemoveWall_ValidCells_CreatesCorrectIntent()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(5, 0, 2)
            };

            // Act
            var intent = BuildIntent.RemoveWall(cells);

            // Assert
            Assert.That(intent.Kind, Is.EqualTo(BuildKind.Wall));
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Remove));
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells[0], Is.EqualTo(cells[0]));
        }

        [Test]
        public void RemoveWall_CopiesCells_Defensively()
        {
            // Arrange
            var cells = new List<MapCellCoord>
            {
                new MapCellCoord(9, 0, 9)
            };

            // Act
            var intent = BuildIntent.RemoveWall(cells);
            cells.Clear();

            // Assert
            Assert.That(intent.Cells, Has.Count.EqualTo(1));
            Assert.That(intent.Cells, Is.Not.SameAs(cells));
        }

        #endregion

        #region General

        [Test]
        public void Intent_StoresMultipleCells_InOriginalOrder()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0),
                new MapCellCoord(1, 0, 0),
                new MapCellCoord(2, 0, 0)
            };

            // Act
            var intent = BuildIntent.PlaceFloor(cells);

            // Assert
            Assert.That(intent.Cells.Count, Is.EqualTo(3));
            Assert.That(intent.Cells.SequenceEqual(cells), Is.True);
        }

        [Test]
        public void ToString_ReturnsReadableSummary()
        {
            // Arrange
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0),
                new MapCellCoord(1, 0, 0)
            };

            var intent = BuildIntent.PlaceWall(cells);

            // Act
            var text = intent.ToString();

            // Assert
            Assert.That(text, Is.EqualTo("Place Wall for 2 cells"));
        }

        #endregion
    }
}
