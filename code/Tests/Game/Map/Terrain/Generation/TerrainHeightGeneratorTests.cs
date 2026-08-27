using CosmosCasino.Core.Game.Map;
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
            float h1 = genA.GetHeight(10, 20);
            float h2 = genB.GetHeight(10, 20);

            // Assert
            Assert.That(h1, Is.EqualTo(h2));
        }

        [Test]
        public void GetHeight_DifferentSeeds_ProduceDifferentValues()
        {
            // Arrange
            var genA = new TerrainHeightGenerator(seed: 1);
            var genB = new TerrainHeightGenerator(seed: 2);
            float h1 = genA.GetHeight(10, 20);
            float h2 = genB.GetHeight(10, 20);

            // Assert
            Assert.That(h1, Is.Not.EqualTo(h2));
        }

        [Test]
        public void GetHeight_SampledAreaIsQuantizedToVerticalGridStep()
        {
            // Arrange
            var gen = new TerrainHeightGenerator(seed: 42);

            // Act
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    float h = gen.GetHeight(x, y);

                    // Assert
                    AssertHeightAlignedToVerticalGridStep(h);
                }
            }
        }

        #endregion

        #region Helpers

        private static void AssertHeightAlignedToVerticalGridStep(float height)
        {
            float scaled = height / Elevation.StepSize;
            Assert.That(MathF.Abs(scaled - MathF.Round(scaled)) < 0.0001f);
        }

        #endregion
    }
}
