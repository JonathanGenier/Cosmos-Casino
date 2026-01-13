using CosmosCasino.Core.Game;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game
{
    [TestFixture]
    internal class GameSessionTests
    {
        #region Create new session

        [Test]
        public void CreateNewSession_ShouldCreateValidSession()
        {
            // Act
            GameSession session = GameSession.CreateNewSession();

            // Assert
            Assert.That(session, Is.Not.Null);
            Assert.That(session.MapManager, Is.Not.Null);
            Assert.That(session.BuildManager, Is.Not.Null);
        }

        #endregion
    }
}
