using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using CosmosCasino.Core.Game.Map.Terrain.Math;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Math
{
    [TestFixture]
    internal sealed class TerrainMathTests
    {
        #region FloorDiv

        [Test]
        public void FloorDiv_PositiveValues_MatchesNormalDivision()
        {
            // Arrange
            int divisor = TerrainConfigs.ChunkSize;

            // Act & Assert
            Assert.That(TerrainMath.FloorDiv(0, divisor), Is.EqualTo(0));
            Assert.That(TerrainMath.FloorDiv(divisor, divisor), Is.EqualTo(1));
            Assert.That(TerrainMath.FloorDiv(divisor * 2, divisor), Is.EqualTo(2));
        }

        [Test]
        public void FloorDiv_PositiveNonMultiple_FloorsDown()
        {
            // Arrange
            int divisor = TerrainConfigs.ChunkSize;

            // Act & Assert
            Assert.That(TerrainMath.FloorDiv(divisor + 1, divisor), Is.EqualTo(1));
            Assert.That(TerrainMath.FloorDiv((divisor * 2) - 1, divisor), Is.EqualTo(1));
        }

        [Test]
        public void FloorDiv_NegativeExactMultiple_ReturnsCorrectChunk()
        {
            // Arrange
            int divisor = TerrainConfigs.ChunkSize;

            // Act & Assert
            Assert.That(TerrainMath.FloorDiv(-divisor, divisor), Is.EqualTo(-1));
            Assert.That(TerrainMath.FloorDiv(-divisor * 2, divisor), Is.EqualTo(-2));
        }

        [Test]
        public void FloorDiv_NegativeNonMultiple_FloorsTowardNegativeInfinity()
        {
            // Arrange
            int divisor = TerrainConfigs.ChunkSize;

            // Act & Assert
            Assert.That(TerrainMath.FloorDiv(-1, divisor), Is.EqualTo(-1));
            Assert.That(TerrainMath.FloorDiv(-divisor + 1, divisor), Is.EqualTo(-1));
            Assert.That(TerrainMath.FloorDiv(-divisor - 1, divisor), Is.EqualTo(-2));
        }

        #endregion

        #region TileWorldCoordToChunkCoord

        [Test]
        public void TileWorldCoordToChunkCoord_Origin_IsChunkZero()
        {
            // Arrange
            var world = new TerrainTileWorldCoord(0, 0);

            // Act
            var chunk = TerrainMath.TileWorldCoordToChunkGridCoord(world);

            // Assert
            Assert.That(chunk, Is.EqualTo(new TerrainChunkGridCoord(0, 0)));
        }

        [Test]
        public void TileWorldCoordToChunkCoord_PositiveBoundary_CrossesChunk()
        {
            // Arrange
            int size = TerrainConfigs.ChunkSize;
            var world = new TerrainTileWorldCoord(size, size);

            // Act
            var chunk = TerrainMath.TileWorldCoordToChunkGridCoord(world);

            // Assert
            Assert.That(chunk, Is.EqualTo(new TerrainChunkGridCoord(1, 1)));
        }

        [Test]
        public void TileWorldCoordToChunkCoord_NegativeCoordinates_MapCorrectly()
        {
            // Arrange
            var world = new TerrainTileWorldCoord(-1, -1);

            // Act
            var chunk = TerrainMath.TileWorldCoordToChunkGridCoord(world);

            // Assert
            Assert.That(chunk.X, Is.EqualTo(-1));
            Assert.That(chunk.Y, Is.EqualTo(-1));
        }

        [Test]
        public void TileWorldCoordToChunkCoord_NegativeBoundary_IsCorrect()
        {
            // Arrange
            int size = TerrainConfigs.ChunkSize;
            var world = new TerrainTileWorldCoord(-size, -size);

            // Act
            var chunk = TerrainMath.TileWorldCoordToChunkGridCoord(world);

            // Assert
            Assert.That(chunk, Is.EqualTo(new TerrainChunkGridCoord(-1, -1)));
        }

        #endregion

        #region TileWorldCoordToLocalCoord

        [Test]
        public void TileWorldCoordToLocalCoord_Origin_IsLocalZero()
        {
            // Arrange
            var world = new TerrainTileWorldCoord(0, 0);
            var chunk = new TerrainChunkGridCoord(0, 0);

            // Act
            var local = TerrainMath.TileWorldCoordToLocalCoord(world, chunk);

            // Assert
            Assert.That(local.X, Is.EqualTo(0));
            Assert.That(local.Y, Is.EqualTo(0));
        }

        [Test]
        public void TileWorldCoordToLocalCoord_ChunkBoundary_IsLocalZero()
        {
            // Arrange
            int size = TerrainConfigs.ChunkSize;
            var world = new TerrainTileWorldCoord(size, size);
            var chunk = new TerrainChunkGridCoord(1, 1);

            // Act
            var local = TerrainMath.TileWorldCoordToLocalCoord(world, chunk);

            // Assert
            Assert.That(local.X, Is.EqualTo(0));
            Assert.That(local.Y, Is.EqualTo(0));
        }

        [Test]
        public void TileWorldCoordToLocalCoord_LastTileInChunk_IsMaxLocal()
        {
            // Arrange
            int size = TerrainConfigs.ChunkSize;
            var world = new TerrainTileWorldCoord(size - 1, size - 1);
            var chunk = new TerrainChunkGridCoord(0, 0);

            // Act
            var local = TerrainMath.TileWorldCoordToLocalCoord(world, chunk);

            // Assert
            Assert.That(local.X, Is.EqualTo(size - 1));
            Assert.That(local.Y, Is.EqualTo(size - 1));
        }

        [Test]
        public void TileWorldCoordToLocalCoord_NegativeWorldCoord_IsValidLocal()
        {
            // Arrange
            int size = TerrainConfigs.ChunkSize;
            var world = new TerrainTileWorldCoord(-1, -1);
            var chunk = new TerrainChunkGridCoord(-1, -1);

            // Act
            var local = TerrainMath.TileWorldCoordToLocalCoord(world, chunk);

            // Assert
            Assert.That(local.X, Is.EqualTo(size - 1));
            Assert.That(local.Y, Is.EqualTo(size - 1));
        }

        #endregion

        #region ChunkGridCoordToWorldOrigin

        [Test]
        public void ChunkGridCoordToWorldOrigin_ZeroZero_IsCenteredCorrectly()
        {
            // Arrange
            var gridCoord = new TerrainChunkGridCoord(0, 0);
            float expectedCenter = -(TerrainConfigs.ChunkSize / 2) - 0.5f;

            // Act
            var origin = TerrainMath.ChunkGridCoordToWorldOrigin(gridCoord);

            // Assert
            Assert.That(origin.X, Is.EqualTo(expectedCenter));
            Assert.That(origin.Y, Is.EqualTo(expectedCenter));
        }

        [Test]
        public void ChunkGridCoordToWorldOrigin_PositiveX_ShiftsByChunkSize()
        {
            // Arrange
            var gridCoord = new TerrainChunkGridCoord(1, 0);
            float expectedX = (1 * TerrainConfigs.ChunkSize) - (TerrainConfigs.ChunkSize / 2) - 0.5f;

            // Act
            var origin = TerrainMath.ChunkGridCoordToWorldOrigin(gridCoord);

            // Assert
            Assert.That(origin.X, Is.EqualTo(expectedX));
        }

        [Test]
        public void ChunkGridCoordToWorldOrigin_PositiveY_ShiftsByChunkSize()
        {
            // Arrange
            var gridCoord = new TerrainChunkGridCoord(0, 1);
            float expectedY = (1 * TerrainConfigs.ChunkSize) - (TerrainConfigs.ChunkSize / 2) - 0.5f;

            // Act
            var origin = TerrainMath.ChunkGridCoordToWorldOrigin(gridCoord);

            // Assert
            Assert.That(origin.Y, Is.EqualTo(expectedY));
        }

        [Test]
        public void ChunkGridCoordToWorldOrigin_NegativeCoordinates_MirrorCorrectly()
        {
            // Arrange
            var gridCoord = new TerrainChunkGridCoord(-1, -1);
            float expected = (-1 * TerrainConfigs.ChunkSize) - (TerrainConfigs.ChunkSize / 2) - 0.5f;

            // Act
            var origin = TerrainMath.ChunkGridCoordToWorldOrigin(gridCoord);

            // Assert
            Assert.That(origin.X, Is.EqualTo(expected));
            Assert.That(origin.Y, Is.EqualTo(expected));
        }

        [Test]
        public void ChunkGridCoordToWorldOrigin_AdjacentChunks_AreExactlyChunkSizeApart()
        {
            // Arrange
            var a = TerrainMath.ChunkGridCoordToWorldOrigin(new TerrainChunkGridCoord(0, 0));
            var b = TerrainMath.ChunkGridCoordToWorldOrigin(new TerrainChunkGridCoord(1, 0));

            // Act
            float distance = b.X - a.X;

            // Assert
            Assert.That(distance, Is.EqualTo(TerrainConfigs.ChunkSize));
        }

        [Test]
        public void ChunkGridCoordToWorldOrigin_SameInput_AlwaysProducesSameOutput()
        {
            // Arrange
            var coord = new TerrainChunkGridCoord(3, -2);

            // Act
            var a = TerrainMath.ChunkGridCoordToWorldOrigin(coord);
            var b = TerrainMath.ChunkGridCoordToWorldOrigin(coord);

            // Assert
            Assert.That(a, Is.EqualTo(b));
        }

        #endregion

        #region Invariants

        [Test]
        public void WorldToChunkAndLocal_ReconstructsOriginalWorld()
        {
            // Arrange
            int size = TerrainConfigs.ChunkSize;

            for (int wx = -size * 2; wx <= size * 2; wx++)
            {
                for (int wy = -size * 2; wy <= size * 2; wy++)
                {
                    var world = new TerrainTileWorldCoord(wx, wy);

                    // Act
                    var chunk = TerrainMath.TileWorldCoordToChunkGridCoord(world);
                    var local = TerrainMath.TileWorldCoordToLocalCoord(world, chunk);

                    int reconstructedX = (chunk.X * size) + (int)local.X;
                    int reconstructedY = (chunk.Y * size) + (int)local.Y;

                    // Assert
                    Assert.That(reconstructedX, Is.EqualTo(wx));
                    Assert.That(reconstructedY, Is.EqualTo(wy));
                }
            }
        }

        #endregion
    }
}
