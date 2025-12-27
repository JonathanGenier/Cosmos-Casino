using CosmosCasino.Core.Console;
using CosmosCasino.Core.Console.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Console
{
    internal sealed class ConsoleManagerTests
    {
        #region FIELDS

        private ConsoleManager? _consoleManager;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _consoleManager = new ConsoleManager(10);
        }

        [TearDown]
        public void TearDown()
        {
            ((IDisposable)_consoleManager!).Dispose();
        }

        #endregion

        #region LOG ENTRY

        [Test]
        public void Console_ReceivesLogs_FromConsoleLog()
        {
            // Act
            ConsoleLog.Info("Test", "Hello");

            // Assert
            Assert.That(_consoleManager!.Count, Is.EqualTo(1));
        }

        [Test]
        public void EntryAdded_IsRaised_WhenLogIsAdded()
        {
            // Assemble
            ConsoleLogEntry? received = null;
            _consoleManager!.EntryAdded += e => received = e;

            // Act
            ConsoleLog.Info("Test", "Hello");

            // Assert
            Assert.That(received, Is.Not.Null);
        }

        #endregion

        #region COMMANDS

        [Test]
        public void ExecuteCommand_DelegatesToRegistry()
        {
            // Act
            var result = _consoleManager!.ExecuteCommand("clear");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("Logs cleared"));
        }

        [Test]
        public void TryClearLogs_ClearsBuffer()
        {
            // Act
            ConsoleLog.Info("Test", "Hello");
            Assert.That(_consoleManager!.Count, Is.EqualTo(1));

            var success = _consoleManager.TryClearLogs();

            // Assert
            Assert.That(success, Is.True);
            Assert.That(_consoleManager.Count, Is.EqualTo(0));
        }

        #endregion
    }
}
