using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class ElevationTests
    {
        #region Construction

        [TestCase(Elevation.MinValue)]
        [TestCase(Elevation.MaxValue)]
        public void Constructor_BoundaryValue_IsValid(int value)
        {
            var elevation = new Elevation(value);

            Assert.That(elevation.Value, Is.EqualTo(value));
        }

        [Test]
        public void Constructor_BelowMinimum_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Elevation(Elevation.MinValue - 1));
        }

        [Test]
        public void Constructor_AboveMaximum_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Elevation(Elevation.MaxValue + 1));
        }

        #endregion

        #region Equality

        [Test]
        public void EqualValues_AddressSameDictionaryEntry()
        {
            var first = new Elevation(4);
            var second = new Elevation(4);
            var values = new Dictionary<Elevation, string>
            {
                [first] = "layer",
            };

            Assert.That(second, Is.EqualTo(first));
            Assert.That(values[second], Is.EqualTo("layer"));
        }

        #endregion
    }
}
