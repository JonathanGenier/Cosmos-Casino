using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Systems;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Systems
{
    [TestFixture]
    internal class TerrainSystemTests
    {
        #region GenerateTerrain

        [Test]
        public void GenerateTerrain_MapSizeZero_ProducesNoTiles()
        {
            // Arrange
            var system = new TerrainSystem();
            var sink = new TestTerrainTileSink();

            // Act
            system.GenerateTerrain(seed: 123, mapSize: 0, sink);

            // Assert
            Assert.That(sink.Received, Is.Empty);
        }

        [Test]
        public void GenerateTerrain_MapSizeOne_ProducesSingleTile()
        {
            // Arrange
            var system = new TerrainSystem();
            var sink = new TestTerrainTileSink();

            // Act
            system.GenerateTerrain(seed: 123, mapSize: 1, sink);

            // Assert
            Assert.That(sink.Received.Count, Is.EqualTo(1));
            Assert.That(sink.Received.ContainsKey(new MapCoord(0, 0)), Is.True);
        }

        [Test]
        public void GenerateTerrain_MapSizeN_ProducesExactGrid()
        {
            // Arrange
            const int mapSize = 4;
            var system = new TerrainSystem();
            var sink = new TestTerrainTileSink();

            // Act
            system.GenerateTerrain(seed: 1, mapSize, sink);

            // Assert
            Assert.That(sink.Received.Count, Is.EqualTo(mapSize * mapSize));

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    Assert.That(sink.Received.ContainsKey(new MapCoord(x, y)), Is.True, $"Missing tile at ({x},{y})");
                }
            }
        }

        [Test]
        public void GenerateTerrain_SameSeed_IsDeterministic()
        {
            // Arrange
            var system = new TerrainSystem();
            var sinkA = new TestTerrainTileSink();
            var sinkB = new TestTerrainTileSink();

            // Act
            system.GenerateTerrain(seed: 42, mapSize: 3, sinkA);
            system.GenerateTerrain(seed: 42, mapSize: 3, sinkB);

            // Assert
            foreach (var kvp in sinkA.Received)
            {
                var other = sinkB.Received[kvp.Key];

                Assert.That(kvp.Value.TopLeftHeight, Is.EqualTo(other.TopLeftHeight));
                Assert.That(kvp.Value.TopRightHeight, Is.EqualTo(other.TopRightHeight));
                Assert.That(kvp.Value.BottomLeftHeight, Is.EqualTo(other.BottomLeftHeight));
                Assert.That(kvp.Value.BottomRightHeight, Is.EqualTo(other.BottomRightHeight));
            }
        }

        [Test]
        public void GenerateTerrain_DifferentSeeds_ProduceDifferentHeights()
        {
            // Arrange
            var system = new TerrainSystem();
            var sinkA = new TestTerrainTileSink();
            var sinkB = new TestTerrainTileSink();

            // Act
            system.GenerateTerrain(seed: 1, mapSize: 3, sinkA);
            system.GenerateTerrain(seed: 2, mapSize: 3, sinkB);

            // Assert
            var anyDifferent =
                sinkA.Received.Any(kvp =>
                    kvp.Value.TopLeftHeight != sinkB.Received[kvp.Key].TopLeftHeight);

            Assert.That(anyDifferent, Is.True);
        }

        #endregion

        #region ResolveSlopeNeighbors

        [Test]
        public void ResolveSlopeNeighbors_NullTiles_AreSkipped()
        {
            // Arrange
            var system = new TerrainSystem();
            var coords = new[] { new MapCoord(0, 0) };

            // Act / Assert
            Assert.DoesNotThrow(() =>
                system.ResolveSlopeNeighbors(coords, _ => null));
        }

        [Test]
        public void ResolveSlopeNeighbors_SlopedTile_IsIgnored()
        {
            // Arrange
            var tile = new TerrainTile(1, 2, 1, 1); // slope
            var tiles = new Dictionary<MapCoord, TerrainTile>
            {
                [new MapCoord(0, 0)] = tile
            };

            var lookup = new TerrainLookup(tiles);
            var system = new TerrainSystem();

            // Act
            system.ResolveSlopeNeighbors(tiles.Keys, lookup.TryGet);

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [Test]
        public void ResolveSlopeNeighbors_FlatTile_WithNoSlopedNeighbors_HasEmptyMask()
        {
            // Arrange
            var flat = new TerrainTile(1, 1, 1, 1);

            var tiles = new Dictionary<MapCoord, TerrainTile>
            {
                [new MapCoord(0, 0)] = flat
            };

            var lookup = new TerrainLookup(tiles);
            var system = new TerrainSystem();

            // Act
            system.ResolveSlopeNeighbors(tiles.Keys, lookup.TryGet);

            // Assert
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [Test]
        public void ResolveSlopeNeighbors_FlatTile_DetectsSingleSlopedNeighbor()
        {
            // Arrange
            var center = new TerrainTile(1, 1, 1, 1);
            var north = new TerrainTile(1, 2, 1, 1); // slope

            var tiles = new Dictionary<MapCoord, TerrainTile>
            {
                [new MapCoord(0, 0)] = center,
                [new MapCoord(0, -1)] = north
            };

            var lookup = new TerrainLookup(tiles);
            var system = new TerrainSystem();

            // Act
            system.ResolveSlopeNeighbors(tiles.Keys, lookup.TryGet);

            // Assert
            Assert.That(
                center.SlopeNeighborMask,
                Is.EqualTo(SlopeNeighborMask.North));
        }

        [Test]
        public void ResolveSlopeNeighbors_FlatTile_DetectsMultipleDirections()
        {
            // Arrange
            var center = new TerrainTile(1, 1, 1, 1);

            var tiles = new Dictionary<MapCoord, TerrainTile>
            {
                [new MapCoord(0, 0)] = center,
                [new MapCoord(1, 0)] = new TerrainTile(1, 2, 1, 1),  // East
                [new MapCoord(0, 1)] = new TerrainTile(1, 1, 2, 1),  // South
            };

            var lookup = new TerrainLookup(tiles);
            var system = new TerrainSystem();

            // Act
            system.ResolveSlopeNeighbors(tiles.Keys, lookup.TryGet);

            // Assert
            Assert.That(
                center.SlopeNeighborMask,
                Is.EqualTo(SlopeNeighborMask.East | SlopeNeighborMask.South));
        }

        [Test]
        public void ResolveSlopeNeighbors_ClearsPreviousMask()
        {
            // Arrange
            var center = new TerrainTile(1, 1, 1, 1);
            center.AddSlopeNeighbor(SlopeNeighborMask.North);

            var tiles = new Dictionary<MapCoord, TerrainTile>
            {
                [new MapCoord(0, 0)] = center
            };

            var lookup = new TerrainLookup(tiles);
            var system = new TerrainSystem();

            // Act
            system.ResolveSlopeNeighbors(tiles.Keys, lookup.TryGet);

            // Assert
            Assert.That(center.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [Test]
        public void ResolveSlopeNeighbors_Idempotent_WhenRepeated()
        {
            // Arrange
            var center = new TerrainTile(1, 1, 1, 1);
            var east = new TerrainTile(1, 2, 1, 1);

            var tiles = new Dictionary<MapCoord, TerrainTile>
            {
                [new MapCoord(0, 0)] = center,
                [new MapCoord(1, 0)] = east
            };

            var lookup = new TerrainLookup(tiles);
            var system = new TerrainSystem();

            // Act
            system.ResolveSlopeNeighbors(tiles.Keys, lookup.TryGet);
            var first = center.SlopeNeighborMask;

            system.ResolveSlopeNeighbors(tiles.Keys, lookup.TryGet);
            var second = center.SlopeNeighborMask;

            // Assert
            Assert.That(first, Is.EqualTo(second));
        }

        #endregion

        #region Helper

        private sealed class TerrainLookup
        {
            private readonly Dictionary<MapCoord, TerrainTile> _tiles;

            internal TerrainLookup(Dictionary<MapCoord, TerrainTile> tiles)
            {
                _tiles = tiles;
            }

            internal TerrainTile? TryGet(MapCoord coord)
            {
                return _tiles.TryGetValue(coord, out var tile) ? tile : null;
            }
        }

        private sealed class TestTerrainTileSink : ITerrainTileSink
        {
            private readonly Dictionary<MapCoord, TerrainTile> received = new();

            internal Dictionary<MapCoord, TerrainTile> Received => received;

            public void ReceiveTerrainTile(MapCoord coord, TerrainTile tile)
            {
                Received.Add(coord, tile);
            }
        }

        #endregion
    }
}