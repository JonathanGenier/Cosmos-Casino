using CosmosCasino.Core.Application.Console;
using CosmosCasino.Core.Application.Console.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Application.Console.Logging
{
    [TestFixture]
    internal class ConsoleLogTests
    {
        #region FIELDS

        private ConsoleManager? _consoleManager;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void SetUp()
        {
            _consoleManager = new ConsoleManager();
            _consoleManager.TryClearLogs(); // ConsoleManager emits 2 logs (Setting up, Ready)
        }

        [TearDown]
        public void TearDown()
        {
            IDisposable consoleManagerDisposable = _consoleManager!;
            consoleManagerDisposable.Dispose();
        }

        #endregion

        #region LOG SAFETY

#if DEBUG
        [Test]
        public void UnsafeLogs_AreRecorded_InStaging()
        {
            // Act
            ConsoleLog.Verbose("Unsafe message");

            // Assert
            Assert.That(_consoleManager!.Count, Is.EqualTo(1));
        }
#endif

#if !DEBUG
        [Test]
        public void UnsafeLogs_AreNotRecorded_InProduction()
        {
            // Act
            ConsoleLog.Verbose("Unsafe message");

            // Assert
            Assert.That(_consoleManager!.Count, Is.EqualTo(0));
        }
#endif

        [Test]
        public void SafeLogs_AreRecorded_InAllEnvironments()
        {
            // Act
            ConsoleLog.Info("Safe message");

            // Assert
            Assert.That(_consoleManager!.Count, Is.EqualTo(1));
        }

        [Test]
        public void ConsoleLog_UsesCorrectDefaultSafetyPerApi()
        {
            // Act
            ConsoleLog.Info("Safe");
            ConsoleLog.Warning("Safe");
            ConsoleLog.Error("Safe");
            ConsoleLog.Event("Safe");
            ConsoleLog.Verbose("Unsafe");
            ConsoleLog.System("Unsafe");
            ConsoleLog.Debug("Unsafe");

            var logs = _consoleManager!.GetLogs();

            // Assert
            // In prod, unsafe logs are dropped.
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(7));
            Assert.That(logs.Count(e => e.Safety == ConsoleLogSafety.Safe), Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Safety == ConsoleLogSafety.Unsafe), Is.EqualTo(3));
#else
            Assert.That(logs.Count, Is.EqualTo(4));
            Assert.That(logs.All(e => e.Safety == ConsoleLogSafety.Safe));
#endif
        }

        #endregion

        #region LOG LEVEL

        [Test]
        public void LogEntries_UsesCorrectDefaultLogLevelPerApi()
        {
            // Act 
            ConsoleLog.Info("Info");
            ConsoleLog.Verbose("Verbose");  // Unsafe
            ConsoleLog.Warning("Warning");
            ConsoleLog.Error("Error");
            ConsoleLog.Event("Info");
            ConsoleLog.System("Info");      // Unsafe
            ConsoleLog.Debug("Verbose");    // Unsafe

            var logs = _consoleManager!.GetLogs();

            // Assert
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(7));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Info), Is.EqualTo(3));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Verbose), Is.EqualTo(2));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Warning), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Error), Is.EqualTo(1));
#else
            Assert.That(logs.Count, Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Info), Is.EqualTo(2));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Warning), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Error), Is.EqualTo(1));
#endif
        }

        #endregion

        #region LOG KIND

        [Test]
        public void LogEntry_UsesCorrectDefaultLogKindPerApi()
        {
            // Act
            ConsoleLog.Info("General");
            ConsoleLog.Verbose("General");  // Unsafe
            ConsoleLog.Warning("General");
            ConsoleLog.Error("General");
            ConsoleLog.Event("Event");
            ConsoleLog.System("System");    // Unsafe
            ConsoleLog.Debug("Debug");      // Unsafe

            var logs = _consoleManager!.GetLogs();

            // Assert
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(7));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.General), Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.Event), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.System), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.Debug), Is.EqualTo(1));
#else
            Assert.That(logs.Count, Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.General), Is.EqualTo(3));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.Event), Is.EqualTo(1));
#endif
        }

        #endregion

        #region ON LOG

        [Test]
        public void OnLog_IsInvoked_ExactlyOnce_PerLog()
        {
            // Assemble
            int count = 0;
            void Handler(ConsoleLogEntry consoleLogEntry) => count++;
            ConsoleLog.OnLog += Handler;

            // Act
            ConsoleLog.Info("Test", "Hello");
            ConsoleLog.OnLog -= Handler;

            // Assert
            Assert.That(count, Is.EqualTo(1));
        }

        #endregion
    }
}