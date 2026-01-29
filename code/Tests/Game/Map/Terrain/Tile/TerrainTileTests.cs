using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using CosmosCasino.Tests.Game.Map.Terrain.Generation;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Tile
{
    [TestFixture]
    internal class TerrainTileTests
    {
        #region New Tile

        [Test]
        public void NewTile_HasNoSlopeNeighbors()
        {
            // Arrange
            var tile = CreateTile();

            // Assert
            Assert.That(tile.SlopeNeighbors, Is.EqualTo(SlopeNeighborMask.None));
            Assert.That(tile.HasAnySlopeNeighbor, Is.False);
        }

        #endregion

        #region GenerateHeights

        [Test]
        public void GenerateHeights_FlatTile_SetsAllHeightsAndIsNotSlope()
        {
            // Arrange
            var tile = CreateTile();
            var heightGenerator = new TestTerrainHeightGenerator((x, y) => 1f);

            // Act
            tile.GenerateHeights(heightGenerator);

            // Assert
            Assert.That(tile.TopLeftHeight, Is.EqualTo(1f));
            Assert.That(tile.TopRightHeight, Is.EqualTo(1f));
            Assert.That(tile.BottomLeftHeight, Is.EqualTo(1f));
            Assert.That(tile.BottomRightHeight, Is.EqualTo(1f));
            Assert.That(tile.IsSlope, Is.False);
        }

        [Test]
        public void GenerateHeights_SlopedTile_DetectsSlope()
        {
            // Arrange
            var tile = CreateTile();
            var heightGenerator = new TestTerrainHeightGenerator(
                (x, y) => (x == 0 && y == 1) ? 2f : 1f);

            // Act
            tile.GenerateHeights(heightGenerator);

            // Assert
            Assert.That(tile.IsSlope, Is.True);
        }

        [Test]
        public void GenerateHeights_CalledTwice_Throws()
        {
            // Arrange
            var tile = CreateTile();
            var heightGenerator = new TestTerrainHeightGenerator(
                (x, y) => 1f);

            // Act
            tile.GenerateHeights(heightGenerator);

            // Assert
            Assert.Throws<InvalidOperationException>(() => tile.GenerateHeights(heightGenerator));
        }

        [Test]
        public void GenerateHeights_UsesWorldCoordinates()
        {
            // Arrange
            var tile = CreateTile();
            var heightGenerator = new TestTerrainHeightGenerator(
                (x, y) => (x * 1000f) + y);

            // Act
            tile.GenerateHeights(heightGenerator);

            // Assert
            Assert.That(tile.TopLeftHeight, Is.EqualTo(0));
            Assert.That(tile.TopRightHeight, Is.EqualTo(1000f));
            Assert.That(tile.BottomLeftHeight, Is.EqualTo(1f));
            Assert.That(tile.BottomRightHeight, Is.EqualTo(1001f));
        }

        [Test]
        public void GenerateHeights_DiagonalDifference_IsSlope()
        {
            // Arrange
            var tile = CreateTile();
            var heightGenerator = new TestTerrainHeightGenerator((x, y) => (x == 1 && y == 1) ? 2f : 1f);

            // Act
            tile.GenerateHeights(heightGenerator);

            // Assert
            Assert.That(tile.IsSlope, Is.True);
        }

        [Test]
        public void GenerateHeights_AllCornersDifferent_IsSlope()
        {
            // Arrange
            var tile = CreateTile();
            var heightGenerator = new TestTerrainHeightGenerator((x, y) =>
            {
                if (x == 0 && y == 0)
                {
                    return 1f;
                }

                if (x == 1 && y == 0)
                {
                    return 2f;
                }

                if (x == 0 && y == 1)
                {
                    return 3f;
                }

                return 4f;
            });

            // Act
            tile.GenerateHeights(heightGenerator);

            // Assert
            Assert.That(tile.IsSlope, Is.True);
        }

        [Test]
        public void GenerateHeights_SmallDifference_IsSlope()
        {
            // Arrange
            var tile = CreateTile();
            var heightGenerator = new TestTerrainHeightGenerator(
                (x, y) => (x == 0 && y == 1) ? 1.001f : 1f);

            // Act
            tile.GenerateHeights(heightGenerator);

            // Assert
            Assert.That(tile.IsSlope, Is.True);
        }

        #endregion

        #region Neighbors Slopes

        [Test]
        public void AddSlopeNeighbor_SetsMask()
        {
            // Arrange
            var tile = CreateTile();

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);

            // Assert
            Assert.That(tile.SlopeNeighbors, Is.EqualTo(SlopeNeighborMask.North));
            Assert.That(tile.HasAnySlopeNeighbor, Is.True);
        }

        [Test]
        public void AddSlopeNeighbor_MultipleDirections_CombineFlags()
        {
            // Arrange
            var tile = CreateTile();

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);
            tile.AddSlopeNeighbor(SlopeNeighborMask.East);

            // Assert
            Assert.That(tile.SlopeNeighbors, Is.EqualTo(SlopeNeighborMask.North | SlopeNeighborMask.East));
        }

        [Test]
        public void AddSlopeNeighbor_OrderDoesNotMatter()
        {
            // Arrange
            var tile = CreateTile();

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.West);
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);

            // Assert
            Assert.That(tile.SlopeNeighbors, Is.EqualTo(SlopeNeighborMask.North | SlopeNeighborMask.West));
        }

        #endregion

        #region Clear Slope Neighbors

        [Test]
        public void ClearSlopeNeighbors_ResetsMask()
        {
            // Arrange
            var tile = CreateTile();
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);

            // Act
            tile.ClearSlopeNeighbors();

            // Assert
            Assert.That(tile.SlopeNeighbors, Is.EqualTo(SlopeNeighborMask.None));
        }

        #endregion

        #region Helper Methods

        private static TerrainTile CreateTile()
        {
            var localCoord = new TerrainTileLocalCoord(0, 0);
            var gridCoord = new TerrainChunkGridCoord(0, 0);
            var worldCoord = new TerrainTileWorldCoord(gridCoord, localCoord);

            return new TerrainTile(localCoord, worldCoord);
        }

        #endregion
    }
}