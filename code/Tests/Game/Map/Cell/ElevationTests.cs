using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class ElevationTests
    {
        #region Construction

        [TestCase(-20f)]
        [TestCase(-1f)]
        [TestCase(0f)]
        [TestCase(1f)]
        [TestCase(19f)]
        [TestCase(20f)]
        public void Constructor_ValidGridStep_PreservesValue(float value)
        {
            var elevation = new Elevation(value);

            Assert.That(elevation.Value, Is.EqualTo(value));
        }

        [Test]
        public void Constructor_IntegerValue_RemainsErgonomic()
        {
            var elevation = new Elevation(4);

            Assert.That(elevation.Value, Is.EqualTo(4f));
        }

        [TestCase(-20.5f)]
        [TestCase(20.5f)]
        [TestCase(-19.5f)]
        [TestCase(-1.5f)]
        [TestCase(-1.75f)]
        [TestCase(-0.5f)]
        [TestCase(-0.25f)]
        [TestCase(0.25f)]
        [TestCase(0.5f)]
        [TestCase(1.25f)]
        [TestCase(1.5f)]
        [TestCase(3.7f)]
        [TestCase(19.5f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Constructor_InvalidValue_ThrowsArgumentOutOfRangeException(float value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Elevation(value));
        }

        [Test]
        public void DefaultElevation_RepresentsZero()
        {
            Assert.That(default(Elevation), Is.EqualTo(new Elevation(0f)));
            Assert.That(default(Elevation).Value, Is.EqualTo(0f));
        }

        #endregion

        #region Equality

        [Test]
        public void GridStepIdentity_DrivesEqualityAndHashing()
        {
            var first = new Elevation(2f);
            var second = new Elevation(2f);

            Assert.That(second, Is.EqualTo(first));
            Assert.That(second.GetHashCode(), Is.EqualTo(first.GetHashCode()));
            Assert.That(second, Is.Not.EqualTo(new Elevation(1f)));
            Assert.That(second, Is.Not.EqualTo(new Elevation(3f)));
        }

        [Test]
        public void EqualGridStepValues_AddressSameDictionaryEntry()
        {
            var stored = new Elevation(2f);
            var lookup = new Elevation(2f);
            var values = new Dictionary<Elevation, string>
            {
                [stored] = "layer",
            };

            Assert.That(values[lookup], Is.EqualTo("layer"));
        }

        #endregion
    }
}
