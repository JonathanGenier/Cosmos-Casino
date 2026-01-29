using CosmosCasino.Core.Game.Map.Terrain.Generation;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Generation
{
    [TestFixture]
    internal class TerrainHeightGeneratorTests
    {
        #region GetHeight

        [Test]
        public void GetHeight_SameSeedSameInput_IsDeterministic()
        {
            // Arrange
            var genA = new TerrainHeightGenerator(seed: 123);
            var genB = new TerrainHeightGenerator(seed: 123);
            float h1 = ((ITerrainHeightProvider)genA).GetHeight(10, 20);
            float h2 = ((ITerrainHeightProvider)genB).GetHeight(10, 20);

            // Assert
            Assert.That(h1, Is.EqualTo(h2));
        }

        [Test]
        public void GetHeight_DifferentSeeds_ProduceDifferentValues()
        {
            // Arrange
            var genA = new TerrainHeightGenerator(seed: 1);
            var genB = new TerrainHeightGenerator(seed: 2);
            float h1 = ((ITerrainHeightProvider)genA).GetHeight(10, 20);
            float h2 = ((ITerrainHeightProvider)genB).GetHeight(10, 20);

            // Assert
            Assert.That(h1, Is.Not.EqualTo(h2));
        }

        [Test]
        public void GetHeight_ResultIsQuantized()
        {
            // Arrange
            var gen = new TerrainHeightGenerator(seed: 42);

            // Act
            float h = ((ITerrainHeightProvider)gen).GetHeight(10, 20);

            // Assert
            float doubled = h * 2f;
            Assert.That(MathF.Abs(doubled - MathF.Round(doubled)) < 0.0001f);
        }

        #endregion
    }
}
