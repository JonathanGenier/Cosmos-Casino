using CosmosCasino.Core.Application.Services;
using NUnit.Framework;

namespace CosmosCasino.Tests.Application.Services
{
    [TestFixture]
    internal class CoreServicesTests
    {
        #region FIELDS

        private CoreServices? _coreServices;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _coreServices = new CoreServices("validPath");
        }

        #endregion

        #region CORESERVICES

        [Test]
        public void Constructor_SavePathIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Assert
            Assert.That(() => new CoreServices(null!), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => new CoreServices(string.Empty), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new CoreServices("   "), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CoreServices_ShouldInitializeSaveManager()
        {
            // Assert
            Assert.That(_coreServices!.SaveManager, Is.Not.Null);
        }

        #endregion

        #region STARTNEWGAME

        [Test]
        public void StartNewGame_WhenGameManagerIsNull_ShouldInitializeGameManager()
        {
            // Act
            _coreServices!.StartGame();

            // Assert
            Assert.That(_coreServices!.GameManager, Is.Not.Null);
        }

        [Test]
        public void StartNewGame_WhenGameManagerIsNull_ShouldReturnTrue()
        {
            // Act
            var result = _coreServices!.StartGame();

            // Assert
            Assert.That(result, Is.True);
        }

#if DEBUG
        [Test]
        public void StartNewGame_WhenGameAlreadyStarted_ThrowsInvalidOperationException_InDebug()
        {
            // Arrange
            _coreServices!.StartGame();

            // Assert
            Assert.That(() => _coreServices!.StartGame(), Throws.TypeOf<InvalidOperationException>());
        }
#endif

#if !DEBUG
        [Test]
        public void StartGame_WhenGameAlreadyStarted_ReturnFalse_InRelease()
        {
            // Arrange
            _coreServices!.StartGame();

            // Assert
            Assert.That(() => _coreServices!.StartGame(), Is.False);
        }
#endif

        #endregion

        #region ENDGAME

        [Test]
        public void EndGame_WhenGameExists_ShouldSetGameManagerToNull()
        {
            // Arrange
            _coreServices!.StartGame();

            // Act
            _coreServices!.EndGame();

            // Assert
            Assert.That(_coreServices!.GameManager, Is.Null);
        }

        [Test]
        public void EndGame_WhenGameExists_ShouldReturnTrue()
        {
            // Arrange
            _coreServices!.StartGame();

            // Act
            var result = _coreServices!.EndGame();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void EndGame_WhenNoGameStarted_ShouldReturnTrue()
        {
            // Act
            var result = _coreServices!.EndGame();

            // Assert
            Assert.That(result, Is.True);
        }
        #endregion
    }
}
