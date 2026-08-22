using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapChunkMetricsTests
    {
        #region Chunk Size

        [Test]
        public void ChunkSize_IsPositive()
        {
            Assert.That(MapChunkMetrics.ChunkSize, Is.Positive);
        }

        [Test]
        public void TerrainChunkSize_IsPositive()
        {
            Assert.That(TerrainConfigs.ChunkSize, Is.Positive);
        }

        [Test]
        public void TerrainTileCountPerAxis_IsDerivedFromLegacyTerrainChunkSize()
        {
            Assert.That(
                TerrainConfigs.TileCountPerAxis,
                Is.EqualTo(TerrainConfigs.ChunkSize * TerrainConfigs.ChunkCountPerAxis));
        }

        #endregion
    }
}
