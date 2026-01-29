using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Tile
{
    [TestFixture]
    internal class TerrainTileWorldCoordTests
    {
        #region Constructor

        [Test]
        public void Constructor_ChunkAndLocal_ComputesCorrectWorldCoord()
        {
            // Arrange
            var gridCoord = new TerrainChunkGridCoord(2, 3);
            var localCoord = new TerrainTileLocalCoord(4, 5);

            // Act
            var worldCoord = new TerrainTileWorldCoord(gridCoord, localCoord);

            // Assert
            Assert.That(worldCoord.X, Is.EqualTo((2 * TerrainConfigs.ChunkSize) + 4));
            Assert.That(worldCoord.Y, Is.EqualTo((3 * TerrainConfigs.ChunkSize) + 5));
        }

        [Test]
        public void Constructor_NegativeChunkCoord_ComputesCorrectWorldCoord()
        {
            // Arrange
            var gridCoord = new TerrainChunkGridCoord(-1, -2);
            var localCoord = new TerrainTileLocalCoord(0, 0);
            var worldCoord = new TerrainTileWorldCoord(gridCoord, localCoord);

            // Assert
            Assert.That(worldCoord.X, Is.EqualTo(-TerrainConfigs.ChunkSize));
            Assert.That(worldCoord.Y, Is.EqualTo(-2 * TerrainConfigs.ChunkSize));
        }

        #endregion
    }
}
