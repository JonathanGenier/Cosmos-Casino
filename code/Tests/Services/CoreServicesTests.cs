using CosmosCasino.Core.Serialization;
using CosmosCasino.Core.Services;
using NUnit.Framework;

namespace CosmosCasino.Tests.Services
{
    [TestFixture]
    internal class CoreServicesTests
    {
        #region CORESERVICES

        [Test]
        public void Constructor_SerializerIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => new CoreServices(null!, "validPath"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_SavePathIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(() => new CoreServices(new JsonSaveSerializer(), null!), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => new CoreServices(new JsonSaveSerializer(), ""), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new CoreServices(new JsonSaveSerializer(), "   "), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CoreServices_ShouldInitializeSaveManager()
        {
            // Arrange & Act
            var coreServices = new CoreServices(new JsonSaveSerializer(), "validPath");

            // Assert
            Assert.That(coreServices.SaveManager, Is.Not.Null);
        }

        #endregion

        #region STARTNEWGAME

        [Test]
        public void StartNewGame_WhenGameAlreadyStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            var coreServices = new CoreServices(new JsonSaveSerializer(), "path");
            coreServices.StartNewGame();

            // Act & Assert
            Assert.That(() => coreServices.StartNewGame(), Throws.TypeOf<InvalidOperationException>());
        }


        [Test]
        public void StartNewGame_ShouldInitializeGameManager()
        {
            // Arrange
            var coreServices = new CoreServices(new JsonSaveSerializer(), "validPath");

            // Act
            coreServices.StartNewGame();

            // Assert
            Assert.That(coreServices.GameManager, Is.Not.Null);
        }

        #endregion

        #region ENDGAME

        [Test]
        public void EndGame_ShouldSetGameManagerToNull()
        {
            // Arrange
            var coreServices = new CoreServices(new JsonSaveSerializer(), "validPath");
            coreServices.StartNewGame();

            // Act
            coreServices.EndGame();

            // Assert
            Assert.That(coreServices.GameManager, Is.Null);
        }

        [Test]
        public void EndGame_WhenNoGameStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            var coreServices = new CoreServices(new JsonSaveSerializer(), "validPath");

            // Act & Assert
            Assert.That(() => coreServices.EndGame(), Throws.TypeOf<InvalidOperationException>());
        }

        #endregion
    }
}
