using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Chunk
{
    [TestFixture]
    internal class TerrainChunkTests
    {
        #region Constructor

        [Test]
        public void Constructor_AssignsChunkCoordAndTiles()
        {
            // Arrange
            var tiles = CreateTiles();
            var coord = new TerrainChunkGridCoord(3, 5);

            // Act
            var chunk = new TerrainChunk(coord, tiles);

            // Assert
            Assert.That(chunk.GridCoord, Is.EqualTo(coord));
            Assert.That(chunk.Tiles, Is.SameAs(tiles));
        }

        [Test]
        public void Constructor_InvalidTileCount_Throws()
        {
            // Arrange
            var tiles = new List<TerrainTile>(1);
            var localCoord = new TerrainTileLocalCoord(0, 0);
            var gridCoord = new TerrainChunkGridCoord(0, 0);
            var worldCoord = new TerrainTileWorldCoord(gridCoord, localCoord);
            tiles.Add(new TerrainTile(localCoord, worldCoord));

            // Assert
            Assert.Throws<ArgumentException>(() => new TerrainChunk(new TerrainChunkGridCoord(0, 0), tiles));
        }

        [Test]
        public void Constructor_NullTiles_Throws()
        {
            // Arrange
            var gridCoord = new TerrainChunkGridCoord(0, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(() => new TerrainChunk(gridCoord, null!));
        }

        [Test]
        public void Constructor_TileLocalCoordMismatch_Throws()
        {
            // Arrange
            var tiles = CreateTiles().ToList();
            var localCoord = new TerrainTileLocalCoord(5, 5);
            var gridCoord = new TerrainChunkGridCoord(0, 0);
            var worldCoord = new TerrainTileWorldCoord(gridCoord, localCoord);
            tiles[0] = new TerrainTile(localCoord, worldCoord);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new TerrainChunk(new TerrainChunkGridCoord(0, 0), tiles));
        }

        #endregion

        #region TryGetTile

        [Test]
        public void TryGetTile_ValidLocalCoord_ReturnsTrueAndCorrectTile()
        {
            // Arrange
            var tiles = CreateTiles();
            var gridCoord = new TerrainChunkGridCoord(0, 0);
            var chunk = new TerrainChunk(gridCoord, tiles);
            var localCoord = new TerrainTileLocalCoord(2, 3);
            var expectedIndex = localCoord.Index;
            var expectedTile = tiles[expectedIndex];

            // Act
            var result = chunk.TryGetTile(localCoord, out var tile);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(tile, Is.SameAs(expectedTile));
        }

        [Test]
        public void TryGetTile_AllValidLocalCoords_ReturnTrue()
        {
            // Arrange
            var tiles = CreateTiles();
            var chunk = new TerrainChunk(new TerrainChunkGridCoord(0, 0), tiles);

            // Act & Assert
            for (int y = 0; y < TerrainConfigs.ChunkSize; y++)
            {
                for (int x = 0; x < TerrainConfigs.ChunkSize; x++)
                {
                    var localCoord = new TerrainTileLocalCoord(x, y);
                    var result = chunk.TryGetTile(localCoord, out var tile);

                    Assert.That(result, Is.True);
                    Assert.That(tile, Is.Not.Null);
                    Assert.That(tile.LocalCoord, Is.EqualTo(localCoord));
                }
            }
        }

        [Test]
        public void TryGetTile_DoesNotMutateTiles()
        {
            // Arrange
            var tiles = CreateTiles();
            var chunk = new TerrainChunk(new TerrainChunkGridCoord(0, 0), tiles);

            var originalTiles = tiles.ToArray();

            // Act
            var coord = new TerrainTileLocalCoord(1, 1);
            chunk.TryGetTile(coord, out _);

            // Assert
            Assert.That(chunk.Tiles, Is.EqualTo(originalTiles));
        }

        [Test]
        public void TryGetTile_NeverThrows_ForValidLocalCoord()
        {
            // Arrange
            var tiles = CreateTiles();
            var chunk = new TerrainChunk(new TerrainChunkGridCoord(0, 0), tiles);
            var coord = new TerrainTileLocalCoord(0, 0);

            // Act & Assert
            Assert.DoesNotThrow(() => chunk.TryGetTile(coord, out _));
        }

        #endregion

        #region Tiles

        [Test]
        public void Tiles_AreInRowMajorOrder()
        {
            // Arrange
            var tiles = CreateTiles();
            var chunk = new TerrainChunk(new TerrainChunkGridCoord(0, 0), tiles);

            // Act & Assert
            for (int y = 0; y < TerrainConfigs.ChunkSize; y++)
            {
                for (int x = 0; x < TerrainConfigs.ChunkSize; x++)
                {
                    var expectedIndex = x + (y * TerrainConfigs.ChunkSize);
                    Assert.That(chunk.Tiles[expectedIndex].LocalCoord, Is.EqualTo(new TerrainTileLocalCoord(x, y)));
                }
            }
        }

        [Test]
        public void Tiles_IsSameInstanceProvidedToConstructor()
        {
            // Arrange
            var tiles = CreateTiles();
            var chunk = new TerrainChunk(new TerrainChunkGridCoord(0, 0), tiles);

            // Assert
            Assert.That(chunk.Tiles, Is.SameAs(tiles));
        }

        #endregion

        #region Helpers

        private static IReadOnlyList<TerrainTile> CreateTiles()
        {
            var tiles = new List<TerrainTile>(TerrainConfigs.ChunkSize * TerrainConfigs.ChunkSize);
            var gridCoord = new TerrainChunkGridCoord(0, 0);

            for (int y = 0; y < TerrainConfigs.ChunkSize; y++)
            {
                for (int x = 0; x < TerrainConfigs.ChunkSize; x++)
                {
                    var localCoord = new TerrainTileLocalCoord(x, y);
                    var worldCoord = new TerrainTileWorldCoord(gridCoord, localCoord);

                    tiles.Add(new TerrainTile(localCoord, worldCoord));
                }
            }

            return tiles;
        }

        #endregion
    }
}
