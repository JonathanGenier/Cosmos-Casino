using CosmosCasino.Core.Debug;
using CosmosCasino.Core.Debug.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug
{
    internal sealed class DebugConsoleTests
    {
        #region FIELDS

        private DebugConsole? _console;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _console = new DebugConsole(10);
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
            DevLog.Info("Test", "Hello");

            // Assert
            Assert.That(_console!.Count, Is.EqualTo(1));
        }

        [Test]
        public void EntryAdded_IsRaised_WhenLogIsAdded()
        {
            // Assemble
            LogEntry? received = null;
            _console!.EntryAdded += e => received = e;

            // Act
            DevLog.Info("Test", "Hello");

            // Assert
            Assert.That(received, Is.Not.Null);
        }

        #endregion

        #region COMMANDS

        [Test]
        public void ExecuteCommand_DelegatesToRegistry()
        {
            // Act
            var result = _console!.ExecuteCommand("log.clear");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("Logs cleared"));
        }

        [Test]
        public void TryClearLogs_ClearsBuffer()
        {
            // Act
            DevLog.Info("Test", "Hello");
            Assert.That(_console!.Count, Is.EqualTo(1));

            var success = _console.TryClearLogs();

            // Assert
            Assert.That(success, Is.True);
            Assert.That(_console.Count, Is.EqualTo(0));
        }

        #endregion
    }
}
