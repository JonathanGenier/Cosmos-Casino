using CosmosCasino.Core.Debug;
using CosmosCasino.Core.Debug.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug.Logging
{
#if DEBUG
    // Tests DevLog behavior during bootstrap, before any DebugConsole exists.
    [TestFixture]
    internal sealed class DevLogBootstrapTests
    {
        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            DevLog.ResetForUnitTests(); // Only exists in Debug
        }

        #endregion

        #region EARLY LOGS BUFFER

        [Test]
        public void EarlyLogs_AreBuffered_AndDrainedIntoConsole()
        {
            // Assemble

            DevLog.Info("Boot", "Early log");

            // Act
            var console = new ConsoleManager();

            // Assert
            var logs = console.GetLogs();
            Assert.That(logs.Count, Is.EqualTo(1));
            Assert.That(logs[0].Category, Is.EqualTo("Boot"));
        }

        #endregion
    }
#endif
}
