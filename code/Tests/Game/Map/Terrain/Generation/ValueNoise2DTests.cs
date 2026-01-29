using CosmosCasino.Core.Game.Map.Terrain.Generation;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Generation
{
    [TestFixture]
    internal class ValueNoise2DTests
    {
        #region Sample

        [Test]
        public void Sample_SameSeedSameInput_IsDeterministic()
        {
            // Arrange
            var noiseA = new ValueNoise2D(seed: 123);
            var noiseB = new ValueNoise2D(seed: 123);

            // Act
            float a = noiseA.Sample(10.5f, 20.25f);
            float b = noiseB.Sample(10.5f, 20.25f);

            // Assert
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Sample_DifferentSeeds_ProduceDifferentValues()
        {
            // Arrange
            var noiseA = new ValueNoise2D(seed: 1);
            var noiseB = new ValueNoise2D(seed: 2);

            // Act
            float a = noiseA.Sample(10.5f, 20.25f);
            float b = noiseB.Sample(10.5f, 20.25f);

            // Assert
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Sample_ResultIsWithinNormalizedRange()
        {
            // Arrange
            var noise = new ValueNoise2D(seed: 42);

            // Act
            float v = noise.Sample(100.1f, -55.7f);

            // Assert
            Assert.That(v, Is.InRange(0f, 1f));
        }

        [Test]
        public void Sample_DoesNotProduceNaNOrInfinity()
        {
            // Arrange
            var noise = new ValueNoise2D(seed: 999);

            // Act
            float v = noise.Sample(100_000f, -100_000f);

            // Assert
            Assert.That(float.IsNaN(v), Is.False);
            Assert.That(float.IsInfinity(v), Is.False);
        }

        [Test]
        public void Sample_NegativeCoordinates_AreDeterministic()
        {
            // Arrange
            var noise = new ValueNoise2D(seed: 5);

            // Act
            float a = noise.Sample(-10.75f, -20.5f);
            float b = noise.Sample(-10.75f, -20.5f);

            // Assert
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Sample_IntegerBoundary_IsContinuous()
        {
            // Arrange
            var noise = new ValueNoise2D(seed: 42);

            // Act
            float a = noise.Sample(10.9999f, 20f);
            float b = noise.Sample(11.0001f, 20f);

            // Assert
            Assert.That(MathF.Abs(a - b), Is.LessThan(0.2f));
        }

        #endregion

        #region SampleOctaves

        [Test]
        public void SampleOctaves_SameSeedSameInput_IsDeterministic()
        {
            // Arrange
            var noiseA = new ValueNoise2D(seed: 123);
            var noiseB = new ValueNoise2D(seed: 123);

            // Act
            float a = noiseA.SampleOctaves(10f, 20f, 5, 0.1f, 1f, 0.5f);
            float b = noiseB.SampleOctaves(10f, 20f, 5, 0.1f, 1f, 0.5f);

            // Assert
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void SampleOctaves_ResultIsWithinNormalizedRange()
        {
            // Arrange
            var noise = new ValueNoise2D(seed: 77);

            // Act
            float v = noise.SampleOctaves(0f, 0f, 6, 0.05f, 1f, 0.5f);

            // Assert
            Assert.That(v, Is.InRange(0f, 1f));
        }

        [Test]
        public void SampleOctaves_DoesNotProduceNaNOrInfinity()
        {
            // Arrange
            var noise = new ValueNoise2D(seed: 42);

            // Act
            float v = noise.SampleOctaves(
                100000f,
                -100000f,
                octaves: 4,
                frequency: 0.01f,
                amplitude: 1f,
                persistence: 0.5f
            );

            // Assert
            Assert.That(float.IsNaN(v), Is.False);
            Assert.That(float.IsInfinity(v), Is.False);
        }

        [Test]
        public void SampleOctaves_OneOctave_EqualsSample()
        {
            // Arrange
            var noise = new ValueNoise2D(seed: 7);

            // Act
            float a = noise.Sample(5.3f, 9.7f);
            float b = noise.SampleOctaves(
                5.3f,
                9.7f,
                octaves: 1,
                frequency: 1f,
                amplitude: 1f,
                persistence: 1f
            );

            // Assert
            Assert.That(a, Is.EqualTo(b));
        }

        #endregion
    }
}