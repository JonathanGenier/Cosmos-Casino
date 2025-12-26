using CosmosCasino.Core.Console;
using CosmosCasino.Core.Console.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Console
{
    internal sealed class ConsoleManagerTests
    {
        #region FIELDS

        private ConsoleManager? _console;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _console = new ConsoleManager(10);
        }

        [TearDown]
        public void TearDown()
        {
            _console!.Dispose();
        }

        #endregion

        #region LOG ENTRY

        [Test]
        public void Console_ReceivesLogs_FromDevLog()
        {
            // Act
            ConsoleLog.Info("Test", "Hello");

            // Assert
            Assert.That(_console!.Count, Is.EqualTo(1));
        }

        [Test]
        public void EntryAdded_IsRaised_WhenLogIsAdded()
        {
            // Assemble
            ConsoleLogEntry? received = null;
            _console!.EntryAdded += e => received = e;

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
            var result = _console!.ExecuteCommand("clear");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("Logs cleared"));
        }

        [Test]
        public void TryClearLogs_ClearsBuffer()
        {
            // Act
            ConsoleLog.Info("Test", "Hello");
            Assert.That(_console!.Count, Is.EqualTo(1));

            var success = _console.TryClearLogs();

            // Assert
            Assert.That(success, Is.True);
            Assert.That(_console.Count, Is.EqualTo(0));
        }

        #endregion
    }
}
