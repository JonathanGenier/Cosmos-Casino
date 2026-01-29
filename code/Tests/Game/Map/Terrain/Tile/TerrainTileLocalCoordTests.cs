using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Tile
{
    [TestFixture]
    internal class TerrainTileLocalCoordTests
    {
        #region Constructor

        [Test]
        public void Constructor_Int_AssignsXAndY()
        {
            // Arrange
            var coord = new TerrainTileLocalCoord(3, 5);

            // Assert
            Assert.That(coord.X, Is.EqualTo(3u));
            Assert.That(coord.Y, Is.EqualTo(5u));
        }

        [Test]
        public void Constructor_UInt_AssignsXAndY()
        {
            // Arrange
            var coord = new TerrainTileLocalCoord(2u, 4u);

            // Assert
            Assert.That(coord.X, Is.EqualTo(2u));
            Assert.That(coord.Y, Is.EqualTo(4u));
        }

        [Test]
        public void Constructor_MaxValidValues_DoesNotThrow()
        {
            // Assert
            Assert.DoesNotThrow(() => new TerrainTileLocalCoord(TerrainConfigs.ChunkSize - 1, TerrainConfigs.ChunkSize - 1));
        }

        [Test]
        public void Constructor_Int_NegativeValues_Throws()
        {
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TerrainTileLocalCoord(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TerrainTileLocalCoord(0, -1));
        }

        [Test]
        public void Constructor_Int_OutOfBounds_Throws()
        {
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TerrainTileLocalCoord(TerrainConfigs.ChunkSize, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TerrainTileLocalCoord(0, TerrainConfigs.ChunkSize));
        }

        [Test]
        public void Constructor_UInt_OutOfBounds_Throws()
        {
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TerrainTileLocalCoord((uint)TerrainConfigs.ChunkSize, 0u));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TerrainTileLocalCoord(0u, (uint)TerrainConfigs.ChunkSize));
        }

        #endregion

        #region Index

        [Test]
        public void Index_IsRowMajor()
        {
            // Arrange
            var coord = new TerrainTileLocalCoord(3, 2);
            var expectedIndex = 3 + (2 * TerrainConfigs.ChunkSize);

            // Assert
            Assert.That(coord.Index, Is.EqualTo(expectedIndex));
        }

        #endregion
    }
}