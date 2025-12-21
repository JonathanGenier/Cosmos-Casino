using CosmosCasino.Core.Debug.Environment;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug.Environment
{
    [TestFixture]
    internal class AppEnvironmentTests
    {
        [Test]
        public void Environment_Is_Consistent()
        {
#if DEBUG
            Assert.That(AppEnvironment.IsDev, Is.EqualTo(true));
            Assert.That(AppEnvironment.IsProd, Is.EqualTo(false));
#else
            Assert.That(AppEnvironment.IsProd, Is.EqualTo(true));
            Assert.That(AppEnvironment.IsDev, Is.EqualTo(false));
#endif
        }
    }
}
