using CosmosCasino.Core.Services;
using NUnit.Framework;

namespace CosmosCasino.Tests.Services
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
            _coreServices!.StartNewGame();

            // Assert
            Assert.That(_coreServices!.GameManager, Is.Not.Null);
        }

        [Test]
        public void StartNewGame_WhenGameManagerIsNull_ShouldReturnTrue()
        {
            // Act
            var result = _coreServices!.StartNewGame();

            // Assert
            Assert.That(result, Is.True);
        }

#if DEBUG
        [Test]
        public void StartNewGame_WhenGameAlreadyStarted_ThrowsInvalidOperationException_InDebug()
        {
            // Arrange
            _coreServices!.StartNewGame();

            // Assert
            Assert.That(() => _coreServices!.StartNewGame(), Throws.TypeOf<InvalidOperationException>());
        }
#endif

#if !DEBUG
        [Test]
        public void StartNewGame_WhenGameAlreadyStarted_ReturnFalse_InRelease()
        {
            // Arrange
            _coreServices!.StartNewGame();

            // Assert
            Assert.That(() => _coreServices!.StartNewGame(), Is.False);
        }
#endif

        #endregion

        #region ENDGAME

        [Test]
        public void EndGame_WhenGameExists_ShouldSetGameManagerToNull()
        {
            // Arrange
            _coreServices!.StartNewGame();

            // Act
            _coreServices!.EndGame();

            // Assert
            Assert.That(_coreServices!.GameManager, Is.Null);
        }

        [Test]
        public void EndGame_WhenGameExists_ShouldReturnTrue()
        {
            // Arrange
            _coreServices!.StartNewGame();

            // Act
            var result = _coreServices!.EndGame();

            // Assert
            Assert.That(result, Is.True);
        }

#if DEBUG
        [Test]
        public void EndGame_WhenNoGameStarted_ThrowsInvalidOperationException_InDebug()
        {
            // Assert
            Assert.That(() => _coreServices!.EndGame(), Throws.TypeOf<InvalidOperationException>());
        }
#endif

#if !DEBUG
        [Test]
        public void EndGame_WhenNoGameStarted_ReturnFalse_InRelease()
        {
            // Assert
            Assert.That(_coreServices!.EndGame(), Is.False);
        }
#endif

        #endregion
    }
}
