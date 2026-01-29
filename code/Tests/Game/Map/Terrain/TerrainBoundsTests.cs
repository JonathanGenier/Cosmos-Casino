using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain
{
    [TestFixture]
    internal class TerrainBoundsTests
    {
        #region CreateNew

        [Test]
        public void CreateNew_CreatesSymmetricBounds()
        {
            // Act
            var bounds = TerrainBounds.CreateNew(1);

            // Assert
            Assert.That(bounds.MinX, Is.EqualTo(-bounds.MaxX));
            Assert.That(bounds.MinY, Is.EqualTo(-bounds.MaxY));
        }

        [Test]
        public void CreateNew_WidthAndHeightMatchChunkCount()
        {
            // Arrange
            var chunkCountPerAxis = 3;

            // Act
            var bounds = TerrainBounds.CreateNew(chunkCountPerAxis);

            // Assert
            Assert.That(bounds.Width, Is.EqualTo(chunkCountPerAxis));
            Assert.That(bounds.Height, Is.EqualTo(chunkCountPerAxis));
        }

        #endregion

        #region Contains

        [Test]
        public void Contains_OriginChunk_ReturnsTrue()
        {
            // Arrange
            var bounds = TerrainBounds.CreateNew(1);

            // Assert
            Assert.That(bounds.Contains(new TerrainChunkGridCoord(0, 0)), Is.True);
        }

        [Test]
        public void Contains_OutsideBounds_ReturnsFalse()
        {
            // Arrange
            var bounds = TerrainBounds.CreateNew(1);

            // Assert
            Assert.That(bounds.Contains(new TerrainChunkGridCoord(bounds.MaxX + 1, 0)), Is.False);
            Assert.That(bounds.Contains(new TerrainChunkGridCoord(0, bounds.MinY - 1)), Is.False);
        }

        [Test]
        public void Contains_BoundaryCoordinates_ReturnTrue()
        {
            // Arrange
            var bounds = TerrainBounds.CreateNew(1);

            // Assert
            Assert.That(bounds.Contains(new TerrainChunkGridCoord(bounds.MinX, bounds.MinY)), Is.True);
            Assert.That(bounds.Contains(new TerrainChunkGridCoord(bounds.MaxX, bounds.MaxY)), Is.True);
        }

        #endregion
    }
}