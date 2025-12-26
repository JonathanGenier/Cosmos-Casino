using CosmosCasino.Core.Debug;
using CosmosCasino.Core.Debug.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug.Logging
{
    [TestFixture]
    internal class DevLogTests
    {
        #region FIELDS

        private DebugConsole? _debugConsole;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void SetUp()
        {
            _debugConsole = new DebugConsole();
        }

        [TearDown]
        public void TearDown()
        {
            _debugConsole!.Dispose();
        }
        #endregion

        #region LOG SAFETY

#if DEBUG
        [Test]
        public void UnsafeLogs_AreRecorded_InStaging()
        {
            // Act
            DevLog.Info("Unsafe", "Unsafe message", LogSafety.Unsafe);

            // Assert
            Assert.That(_debugConsole!.Count, Is.EqualTo(1));
        }
#endif

#if !DEBUG
        [Test]
        public void UnsafeLogs_AreNotRecorded_InProduction()
        {
            // Act
            DevLog.Info("Unsafe", "Unsafe message", LogSafety.Unsafe);

            // Assert
            Assert.That(_debugConsole!.Count, Is.EqualTo(0));
        }
#endif

        [Test]
        public void SafeLogs_AreRecorded_InAllEnvironments()
        {
            // Act
            DevLog.Info("Unsafe", "Unsafe message", LogSafety.Safe);

            // Assert
            Assert.That(_debugConsole!.Count, Is.EqualTo(1));
        }

        [Test]
        public void DevLog_UsesCorrectDefaultSafetyPerApi()
        {
            // Act
            DevLog.Info("Test", "Safe");
            DevLog.Warning("Test", "Safe");
            DevLog.Error("Test", "Safe");
            DevLog.Event("Test", "Safe");
            DevLog.Command("Test", "Safe");
            DevLog.Verbose("Test", "Unsafe");
            DevLog.System("Test", "Unsafe");

            var logs = _debugConsole!.GetLogs();

            // Assert
            // In prod, unsafe logs are dropped.
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(7));
            Assert.That(logs.Count(e => e.Safety == LogSafety.Safe), Is.EqualTo(5));
            Assert.That(logs.Count(e => e.Safety == LogSafety.Unsafe), Is.EqualTo(2));
#else
            Assert.That(logs.Count, Is.EqualTo(5));
            Assert.That(logs.All(e => e.Safety == LogSafety.Safe));
#endif
        }

        [Test]
        public void DevLog_AllowsExplicitSafetyOverride_ExceptForWarningErrorAndSystem()
        {
            // Act
            DevLog.Info("Test", "Info", LogSafety.Unsafe);
            DevLog.Verbose("Test", "Verbose", LogSafety.Safe);
            DevLog.Event("Test", "Event", LogSafety.Unsafe);
            DevLog.Command("Test", "Command", LogSafety.Unsafe);

            var logs = _debugConsole!.GetLogs();

            // Assert
            // In prod, unsafe logs are dropped.
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Safety == LogSafety.Safe), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Safety == LogSafety.Unsafe), Is.EqualTo(3));
#else
            Assert.That(logs.Count, Is.EqualTo(1));
            Assert.That(logs.All(e => e.Safety == LogSafety.Safe));
#endif
        }

        #endregion

        #region LOG LEVEL

        [Test]
        public void LogEntries_UsesCorrectDefaultLogLevelPerApi()
        {
            // Act 
            // Verbose is explicitly forced to Safe to avoid prod stripping
            DevLog.Info("Test", "Info");
            DevLog.Verbose("Test", "Verbose", LogSafety.Safe);
            DevLog.Warning("Test", "Warning");
            DevLog.Error("Test", "Error");
            DevLog.Event("Test", "Info");
            DevLog.Command("Test", "Info");
            DevLog.System("Test", "Info");

            var logs = _debugConsole!.GetLogs();

            // Assert
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(7));
            Assert.That(logs.Count(e => e.Level == LogLevel.Info), Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Level == LogLevel.Verbose), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Level == LogLevel.Warning), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Level == LogLevel.Error), Is.EqualTo(1));
#else
            Assert.That(logs.Count, Is.EqualTo(6));
            Assert.That(logs.Count(e => e.Level == LogLevel.Info), Is.EqualTo(3));
            Assert.That(logs.Count(e => e.Level == LogLevel.Verbose), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Level == LogLevel.Warning), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Level == LogLevel.Error), Is.EqualTo(1));
#endif
        }

        #endregion

        #region LOG KIND

        [Test]
        public void LogEntry_UsesCorrectDefaultLogKindPerApi()
        {
            // Act
            // Verbose is explicitly forced to Safe to avoid prod stripping
            DevLog.Info("Test", "General");
            DevLog.Verbose("Test", "General", LogSafety.Safe);
            DevLog.Warning("Test", "General");
            DevLog.Error("Test", "General");
            DevLog.Event("Test", "Event");
            DevLog.Command("Test", "Command");
            DevLog.System("Test", "Info");

            var logs = _debugConsole!.GetLogs();

            // Assert
#if DEBUG
            Assert.That(logs.Count, Is.EqualTo(7));
            Assert.That(logs.Count(e => e.Kind == LogKind.General), Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Kind == LogKind.Event), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Kind == LogKind.Command), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Kind == LogKind.System), Is.EqualTo(1));
#else
            Assert.That(logs.Count, Is.EqualTo(6));
            Assert.That(logs.Count(e => e.Kind == LogKind.General), Is.EqualTo(4));
            Assert.That(logs.Count(e => e.Kind == LogKind.Event), Is.EqualTo(1));
            Assert.That(logs.Count(e => e.Kind == LogKind.Command), Is.EqualTo(1));
#endif
        }

        #endregion

        #region ON LOG

        [Test]
        public void OnLog_IsInvoked_ExactlyOnce_PerLog()
        {
            // Assemble
            int count = 0;
            void Handler(LogEntry logEntry) => count++;
            DevLog.OnLog += Handler;

            // Act
            DevLog.Info("Test", "Hello");
            DevLog.OnLog -= Handler;

            // Assert
            Assert.That(count, Is.EqualTo(1));
        }

        #endregion
    }
}
