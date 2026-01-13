using CosmosCasino.Core.Application.Services;
using NUnit.Framework;

namespace CosmosCasino.Tests.Application.Services
{
    [TestFixture]
    internal class CoreServicesTests
    {
        #region FIELDS

        private CoreServices _coreServices = null!;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _coreServices = new CoreServices("validPath");
        }

        #endregion

        #region Initialization

        [Test]
        public void Constructor_SavePathIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Assert
            Assert.That(() => new CoreServices(null!), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => new CoreServices(string.Empty), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new CoreServices("   "), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CoreServices_ShouldCreateValidServices()
        {
            // Assert
            Assert.That(_coreServices.SaveManager, Is.Not.Null);
            Assert.That(_coreServices.ConsoleManager, Is.Not.Null);
        }

        #endregion
    }
}
