using CosmosCasino.Core.Application.Console;
using CosmosCasino.Core.Application.Console.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Application.Console.Logging
{
#if DEBUG
    // Tests ConsoleLog behavior during bootstrap, before any DebugConsole exists.
    [TestFixture]
    internal sealed class ConsoleLogBootstrapTests
    {
        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            ConsoleLog.ResetForUnitTests(); // Only exists in Debug
        }

        #endregion

        #region EARLY LOGS BUFFER

        [Test]
        public void EarlyLogs_AreBuffered_AndDrainedIntoConsole()
        {
            // Assemble

            ConsoleLog.Info("Boot", "Early log");

            // Act
            var consoleManager = new ConsoleManager();

            // Assert
            var logs = consoleManager.GetLogs();
            Assert.That(logs.Count, Is.EqualTo(3)); // ConsoleManager init emit 2 logs.
            Assert.That(logs[0].Category, Is.EqualTo("Boot"));
        }

        #endregion
    }
#endif
}