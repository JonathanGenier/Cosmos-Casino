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
            ConsoleLog.Verbose("Unsafe", "Unsafe message");

            // Assert
            Assert.That(_consoleManager!.Count, Is.EqualTo(1));
        }
#endif

#if !DEBUG
        [Test]
        public void UnsafeLogs_AreNotRecorded_InProduction()
        {
            // Act
            ConsoleLog.Verbose("Unsafe", "Unsafe message");

            // Assert
            Assert.That(_consoleManager!.Count, Is.EqualTo(0));
        }
#endif

        [Test]
        public void SafeLogs_AreRecorded_InAllEnvironments()
        {
            // Act
            ConsoleLog.Info("Unsafe", "Unsafe message");

            // Assert
            Assert.That(_consoleManager!.Count, Is.EqualTo(1));
        }

        [Test]
        public void ConsoleLog_UsesCorrectDefaultSafetyPerApi()
        {
            // Act
            ConsoleLog.Info("Test", "Safe");
            ConsoleLog.Warning("Test", "Safe");
            ConsoleLog.Error("Test", "Safe");
            ConsoleLog.Event("Test", "Safe");
            ConsoleLog.Verbose("Test", "Unsafe");
            ConsoleLog.System("Test", "Unsafe");

            var logs = _consoleManager!.GetLogs();

            // Assert
            // In prod, unsafe logs are dropped.
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(6));
            Assert.That(logs.Count(e => e.Safety == ConsoleLogSafety.Safe), Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Safety == ConsoleLogSafety.Unsafe), Is.EqualTo(2));
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
            ConsoleLog.Info("Test", "Info");
            ConsoleLog.Verbose("Test", "Verbose");  // Unsafe
            ConsoleLog.Warning("Test", "Warning");
            ConsoleLog.Error("Test", "Error");
            ConsoleLog.Event("Test", "Info");
            ConsoleLog.System("Test", "Info");      // Unsafe

            var logs = _consoleManager!.GetLogs();

            // Assert
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(6));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Info), Is.EqualTo(3));
            Assert.That(logs.Count(e => e.Level == ConsoleLogLevel.Verbose), Is.EqualTo(1));
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
            // Verbose is explicitly forced to Safe to avoid prod stripping
            ConsoleLog.Info("Test", "General");
            ConsoleLog.Verbose("Test", "General");
            ConsoleLog.Warning("Test", "General");
            ConsoleLog.Error("Test", "General");
            ConsoleLog.Event("Test", "Event");
            ConsoleLog.System("Test", "Info");

            var logs = _consoleManager!.GetLogs();

            // Assert
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(6));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.General), Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.Event), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Kind == ConsoleLogKind.System), Is.EqualTo(1));
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