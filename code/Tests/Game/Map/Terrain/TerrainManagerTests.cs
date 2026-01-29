using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using CosmosCasino.Core.Game.Map.Terrain.Math;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain
{
    [TestFixture]
    internal sealed class TerrainManagerTests
    {
        #region GenerateInitialTerrain

        [Test]
        public void GenerateInitialTerrain_CalledTwice_Throws()
        {
            // Arrange
            var manager = new TerrainManager(1);

            // Act
            manager.GenerateInitialTerrain(seed: 123);

            // Assert
            Assert.Throws<InvalidOperationException>(() =>
                manager.GenerateInitialTerrain(seed: 456));
        }

        [Test]
        public void GenerateInitialTerrain_GeneratesAllChunksInBounds()
        {
            // Arrange
            var manager = new TerrainManager(1);

            // Act
            manager.GenerateInitialTerrain(seed: 0);

            // Assert
            for (int y = manager.Bounds.MinY; y <= manager.Bounds.MaxY; y++)
            {
                for (int x = manager.Bounds.MinX; x <= manager.Bounds.MaxX; x++)
                {
                    Assert.That(
                        manager.TryGetChunk(new TerrainChunkGridCoord(x, y), out _),
                        Is.True);
                }
            }
        }

        [Test]
        public void GenerateInitialTerrain_SameSeed_IsDeterministic()
        {
            // Arrange
            var m1 = new TerrainManager(1);
            var m2 = new TerrainManager(1);

            // Act
            m1.GenerateInitialTerrain(seed: 42);
            m2.GenerateInitialTerrain(seed: 42);
            m1.TryGetTileFromWorldCoord(new TerrainTileWorldCoord(0, 0), out var t1);
            m2.TryGetTileFromWorldCoord(new TerrainTileWorldCoord(0, 0), out var t2);

            // Assert
            Assert.That(t1.TopLeftHeight, Is.EqualTo(t2.TopLeftHeight));
        }

        [Test]
        public void GenerateInitialTerrain_DifferentSeed_ProducesDifferentTerrain()
        {
            // Arrange
            var m1 = new TerrainManager(1);
            var m2 = new TerrainManager(1);

            // Act
            m1.GenerateInitialTerrain(seed: 1);
            m2.GenerateInitialTerrain(seed: 2);

            m1.TryGetTileFromWorldCoord(new TerrainTileWorldCoord(0, 0), out var t1);
            m2.TryGetTileFromWorldCoord(new TerrainTileWorldCoord(0, 0), out var t2);

            // Assert
            Assert.That(t1.TopLeftHeight, Is.Not.EqualTo(t2.TopLeftHeight));
        }

        [Test]
        public void GenerateInitialTerrain_AllTilesBelongToCorrectChunk()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            // Act & Assert
            for (int y = manager.Bounds.MinY; y <= manager.Bounds.MaxY; y++)
            {
                for (int x = manager.Bounds.MinX; x <= manager.Bounds.MaxX; x++)
                {
                    manager.TryGetChunk(new TerrainChunkGridCoord(x, y), out var chunk);

                    foreach (var tile in chunk.Tiles)
                    {
                        int expectedChunkX = TerrainMath.FloorDiv(
                    tile.WorldCoord.X,
                    TerrainConfigs.ChunkSize);

                        int expectedChunkY = TerrainMath.FloorDiv(
                    tile.WorldCoord.Y,
                    TerrainConfigs.ChunkSize);

                        Assert.That(expectedChunkX, Is.EqualTo(chunk.GridCoord.X));
                        Assert.That(expectedChunkY, Is.EqualTo(chunk.GridCoord.Y));
                    }
                }
            }
        }

        #endregion

        #region TryGetChunk

        [Test]
        public void TryGetChunk_BeforeGeneration_ReturnsFalse()
        {
            // Arrange
            var manager = new TerrainManager(1);

            // Act
            var result = manager.TryGetChunk(new TerrainChunkGridCoord(0, 0), out _);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryGetChunk_OutsideBounds_ReturnsFalse()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);
            var outsideCoord = new TerrainChunkGridCoord(manager.Bounds.MaxX + 1, manager.Bounds.MaxY + 1);

            // Act
            var result = manager.TryGetChunk(outsideCoord, out _);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryGetChunkAtWorld_BeforeGeneration_ReturnsFalse()
        {
            // Arrange
            var manager = new TerrainManager(1);

            // Act
            var result = manager.TryGetChunkFromWorldCoord(new TerrainTileWorldCoord(0, 0), out _);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryGetChunkAtWorld_ValidWorldCoord_ReturnsCorrectChunk()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            var worldCoord = new TerrainTileWorldCoord(0, 0);

            // Act
            var result = manager.TryGetChunkFromWorldCoord(worldCoord, out var chunk);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(chunk.GridCoord, Is.EqualTo(new TerrainChunkGridCoord(0, 0)));
        }

        [Test]
        public void TryGetChunkAtWorld_NegativeWorldCoord_MapsCorrectly()
        {
            // Arrange
            var manager = new TerrainManager(3);
            manager.GenerateInitialTerrain(seed: 0);

            var worldCoord = new TerrainTileWorldCoord(-1, -1);

            // Act
            var result = manager.TryGetChunkFromWorldCoord(worldCoord, out var chunk);

            // Assert
            Assert.That(manager.Bounds.Width, Is.GreaterThanOrEqualTo(3));
            Assert.That(result, Is.True);
            Assert.That(chunk.GridCoord.X, Is.LessThanOrEqualTo(0));
            Assert.That(chunk.GridCoord.Y, Is.LessThanOrEqualTo(0));
        }

        #endregion

        #region TryGetTileAtWorld

        [Test]
        public void TryGetTileAtWorld_BeforeGeneration_ReturnsFalse()
        {
            // Arrange
            var manager = new TerrainManager(1);

            // Act
            var result = manager.TryGetTileFromWorldCoord(
                new TerrainTileWorldCoord(0, 0), out _);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryGetTileAtWorld_ValidWorldCoord_ReturnsTrue()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            var worldCoord = new TerrainTileWorldCoord(0, 0);

            // Act
            var result = manager.TryGetTileFromWorldCoord(worldCoord, out var tile);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(tile, Is.Not.Null);
        }

        [Test]
        public void TryGetTileAtWorld_ChunkBoundary_Works()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            int edge = TerrainConfigs.ChunkSize - 1;
            var boundaryCoord = new TerrainTileWorldCoord(edge, edge);

            // Act
            var result = manager.TryGetTileFromWorldCoord(boundaryCoord, out _);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TryGetTileAtWorld_OutsideTerrain_ReturnsFalse()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            var outsideWorldCoord = new TerrainTileWorldCoord(
                (manager.Bounds.MaxX + 1) * TerrainConfigs.ChunkSize,
                0);

            // Act
            var result = manager.TryGetTileFromWorldCoord(outsideWorldCoord, out _);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryGetTileAtWorld_CrossChunkBoundary_ReturnsCorrectTile()
        {
            // Arrange
            var manager = new TerrainManager(3);
            manager.GenerateInitialTerrain(seed: 0);

            int worldX = TerrainConfigs.ChunkSize;
            var worldCoord = new TerrainTileWorldCoord(worldX, 0);

            // Act
            var result = manager.TryGetTileFromWorldCoord(worldCoord, out var tile);

            // Assert
            Assert.That(manager.Bounds.Width, Is.GreaterThanOrEqualTo(3));
            Assert.That(result, Is.True);
            Assert.That(tile.LocalCoord.X, Is.EqualTo(0));
            Assert.That(tile.LocalCoord.Y, Is.EqualTo(0));
        }

        [Test]
        public void TryGetTileAtWorld_NegativeWorldCoord_ResolvesCorrectTile()
        {
            // Arrange
            var manager = new TerrainManager(3);
            manager.GenerateInitialTerrain(seed: 0);

            var worldCoord = new TerrainTileWorldCoord(-1, 0);

            // Act
            var result = manager.TryGetTileFromWorldCoord(worldCoord, out var tile);

            // Assert
            Assert.That(manager.Bounds.Width, Is.GreaterThanOrEqualTo(3));
            Assert.That(result, Is.True);
            Assert.That(tile.LocalCoord.X, Is.EqualTo(TerrainConfigs.ChunkSize - 1));
        }

        #endregion

        #region ResolveSlopeNeighbors

        [Test]
        public void ResolveSlopeNeighbors_EdgeTiles_DoNotThrow()
        {
            // Arrange
            var manager = new TerrainManager(1);

            // Act + Assert
            Assert.DoesNotThrow(() => manager.GenerateInitialTerrain(seed: 0));
        }

        [Test]
        public void ResolveSlopeNeighbors_SlopeTilesHaveNoNeighbors()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            // Act & Assert
            for (int y = manager.Bounds.MinY; y <= manager.Bounds.MaxY; y++)
            {
                for (int x = manager.Bounds.MinX; x <= manager.Bounds.MaxX; x++)
                {
                    manager.TryGetChunk(new TerrainChunkGridCoord(x, y), out var chunk);

                    foreach (var tile in chunk.Tiles)
                    {
                        if (tile.IsSlope)
                        {
                            Assert.That(tile.HasAnySlopeNeighbor, Is.False);
                        }
                    }
                }
            }
        }

        [Test]
        public void ResolveSlopeNeighbors_FlatTileAdjacentToSlope_SetsNeighborMask()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            TerrainTile flatTile = null!;
            TerrainTile slopeTile = null!;

            foreach (var y in new[] { 0, 1 })
            {
                manager.TryGetTileFromWorldCoord(new TerrainTileWorldCoord(0, y), out var tile);

                if (tile.IsSlope)
                {
                    slopeTile = tile;
                }
                else
                {
                    flatTile = tile;
                }
            }

            Assume.That(flatTile, Is.Not.Null);
            Assume.That(slopeTile, Is.Not.Null);

            // Act
            // already resolved during generation

            // Assert
            Assert.That(flatTile.HasAnySlopeNeighbor, Is.True);
        }

        [Test]
        public void ResolveSlopeNeighbors_MultipleSlopeNeighbors_CombineMasks()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            // Act & Assert
            for (int y = manager.Bounds.MinY; y <= manager.Bounds.MaxY; y++)
            {
                for (int x = manager.Bounds.MinX; x <= manager.Bounds.MaxX; x++)
                {
                    manager.TryGetChunk(new TerrainChunkGridCoord(x, y), out var chunk);

                    foreach (var tile in chunk.Tiles)
                    {
                        if (tile.IsSlope)
                        {
                            continue;
                        }

                        var mask = tile.SlopeNeighbors;
                        int bitCount = CountBits((int)mask);

                        if (bitCount >= 2)
                        {
                            Assert.That(
                                bitCount,
                                Is.GreaterThanOrEqualTo(2),
                                "Expected multiple slope neighbor directions to be combined");

                            return; // success
                        }
                    }
                }
            }

            Assert.Fail("No flat tile with multiple slope neighbors was found in the generated terrain.");
        }

        [Test]
        public void ResolveSlopeNeighbors_DirectionMaskMatchesHeightTransition()
        {
            // Arrange
            var manager = new TerrainManager(1);
            manager.GenerateInitialTerrain(seed: 0);

            // Act & Assert
            for (int y = manager.Bounds.MinY; y <= manager.Bounds.MaxY; y++)
            {
                for (int x = manager.Bounds.MinX; x <= manager.Bounds.MaxX; x++)
                {
                    manager.TryGetChunk(new TerrainChunkGridCoord(x, y), out var chunk);

                    foreach (var tile in chunk.Tiles)
                    {
                        if (tile.IsSlope || !tile.HasAnySlopeNeighbor)
                        {
                            continue;
                        }

                        foreach (SlopeNeighborMask flag in Enum.GetValues(typeof(SlopeNeighborMask)))
                        {
                            if (flag == SlopeNeighborMask.None || flag == SlopeNeighborMask.All)
                            {
                                continue;
                            }

                            if (!tile.SlopeNeighbors.HasFlag(flag))
                            {
                                continue;
                            }

                            var (dx, dy) = flag switch
                            {
                                SlopeNeighborMask.North => (0, -1),
                                SlopeNeighborMask.East => (1, 0),
                                SlopeNeighborMask.South => (0, 1),
                                SlopeNeighborMask.West => (-1, 0),
                                SlopeNeighborMask.NorthEast => (1, -1),
                                SlopeNeighborMask.NorthWest => (-1, -1),
                                SlopeNeighborMask.SouthEast => (1, 1),
                                SlopeNeighborMask.SouthWest => (-1, 1),
                                _ => throw new InvalidOperationException($"Unexpected flag {flag}")
                            };

                            var neighborCoord = new TerrainTileWorldCoord(tile.WorldCoord.X + dx, tile.WorldCoord.Y + dy);

                            Assert.That(manager.TryGetTileFromWorldCoord(neighborCoord, out var neighbor), Is.True);

                            bool heightDiff =
                                tile.TopLeftHeight != neighbor.TopLeftHeight
                                || tile.TopRightHeight != neighbor.TopRightHeight
                                || tile.BottomLeftHeight != neighbor.BottomLeftHeight
                                || tile.BottomRightHeight != neighbor.BottomRightHeight;

                            Assert.That(heightDiff, Is.True, $"Slope mask {flag} set but no height transition detected.");
                        }
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private static int CountBits(int value)
        {
            int count = 0;
            while (value != 0)
            {
                count += value & 1;
                value >>= 1;
            }

            return count;
        }

        #endregion
    }
}