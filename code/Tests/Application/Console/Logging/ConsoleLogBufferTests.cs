using CosmosCasino.Core.Application.Console.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Application.Console.Logging
{
    [TestFixture]
    internal class ConsoleLogBufferTests
    {
        #region CONSTRUCTOR

        [Test]
        public void Constructor_ThrowsArgumentOutOfRangeException_WhenCapacityIsZeroOrNegative()
        {
            Assert.That(() => new ConsoleLogBuffer(0), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ConsoleLogBuffer(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        #endregion

        #region ADD

        [Test]
        public void Add_IncrementsCount_WhenBelowCapacity()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(1);
            var previousCount = buffer.Count;

            // Act
            buffer.Add(CreateLogEntry());

            // Act & Assert
            Assert.That(buffer.Count, Is.EqualTo(previousCount + 1));
        }

        [Test]
        public void Add_CountDoesNotExceedCapacity()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(3);
            buffer.Add(CreateLogEntry("One"));
            buffer.Add(CreateLogEntry("Two"));
            buffer.Add(CreateLogEntry("Three"));

            // Act
            buffer.Add(CreateLogEntry("Four"));

            // Assert
            Assert.That(buffer.Count, Is.EqualTo(buffer.Capacity));
        }

        [Test]
        public void Add_VersionShouldIncrement_WhenLogEntryIsAdded()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(1);
            var previousVersion = buffer.Version;

            // Act
            buffer.Add(CreateLogEntry());

            // Act & Assert
            Assert.That(buffer.Version, Is.EqualTo(previousVersion + 1));
        }

        [Test]
        public void Add_VersionIncrements_WhenOverwriting()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(3);
            buffer.Add(CreateLogEntry("One"));
            buffer.Add(CreateLogEntry("Two"));
            buffer.Add(CreateLogEntry("Three"));

            var previousVersion = buffer.Version;

            // Act
            buffer.Add(CreateLogEntry("Four"));

            // Assert
            Assert.That(buffer.Version, Is.EqualTo(previousVersion + 1));
        }

        [Test]
        public void Add_WhenCapacityExceeded_OverwritesOldestEntry()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(3);
            buffer.Add(CreateLogEntry("One"));
            buffer.Add(CreateLogEntry("Two"));
            buffer.Add(CreateLogEntry("Three"));

            // Act
            buffer.Add(CreateLogEntry("Four"));
            var entries = buffer.Snapshot();

            // Assert
            Assert.That(entries, Has.Count.EqualTo(3));
            Assert.That(entries[0].Message, Is.EqualTo("Two"));
            Assert.That(entries[1].Message, Is.EqualTo("Three"));
            Assert.That(entries[2].Message, Is.EqualTo("Four"));
        }

        #endregion

        #region SNAPSHOT

        [Test]
        public void Snapshot_ReturnsCopy_NotLiveView()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(2);
            buffer.Add(CreateLogEntry("One"));
            var snapshot = buffer.Snapshot();
            buffer.Add(CreateLogEntry("Two"));

            // Assert
            Assert.That(snapshot.Count, Is.EqualTo(1));
            Assert.That(snapshot[0].Message, Is.EqualTo("One"));

            var newSnapShot = buffer.Snapshot();
            Assert.That(newSnapShot.Count, Is.EqualTo(2));
        }

        [Test]
        public void Snapshot_ShouldReturnEntriesInOrder()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(2);
            buffer.Add(CreateLogEntry("First"));
            buffer.Add(CreateLogEntry("Second"));

            // Act
            var snapshot = buffer.Snapshot();

            // Assert
            Assert.That(snapshot.Count, Is.EqualTo(2));
            Assert.That(snapshot[0].Message, Is.EqualTo("First"));
            Assert.That(snapshot[1].Message, Is.EqualTo("Second"));
        }

        [Test]
        public void Snapshot_WhenEmpty_ReturnsEmptyArray()
        {
            var buffer = new ConsoleLogBuffer(3);
            var snapshot = buffer.Snapshot();

            Assert.That(snapshot, Is.Empty);
        }

        #endregion

        #region LOG ENTRY

        [Test]
        public void LogEntry_IsRecordedCorrectly()
        {
            // Arrange & Act
            var buffer = new ConsoleLogBuffer(1);
            buffer.Add(CreateLogEntry("This is a test!"));
            var entry = buffer.Snapshot()[0];

            // Assert
            Assert.That(buffer.Count, Is.EqualTo(1));
            Assert.That(entry.TimestampMs, Is.EqualTo(52));
            Assert.That(entry.Level, Is.EqualTo(ConsoleLogLevel.Info));
            Assert.That(entry.Safety, Is.EqualTo(ConsoleLogSafety.Safe));
            Assert.That(entry.Kind, Is.EqualTo(ConsoleLogKind.General));
            Assert.That(entry.Category, Is.EqualTo("Test"));
            Assert.That(entry.Message, Is.EqualTo("This is a test!"));
        }

        #endregion

        #region CLEAR

        [Test]
        public void Clear_RemovesAllEntries_AndResetsVersion()
        {
            // Arrange
            var buffer = new ConsoleLogBuffer(1);
            buffer.Add(CreateLogEntry());

            // Act
            buffer.Clear();

            // Assert
            Assert.That(buffer.Count, Is.EqualTo(0));
            Assert.That(buffer.Version, Is.EqualTo(0));
            Assert.That(buffer.Snapshot(), Is.Empty);
        }

        #endregion

        #region HELPER

        private static ConsoleLogEntry CreateLogEntry(string message = "Test")
        {
            return new ConsoleLogEntry(
                timestampMs: 52,
                level: ConsoleLogLevel.Info,
                safety: ConsoleLogSafety.Safe,
                kind: ConsoleLogKind.General,
                category: "Test",
                message: message
            );
        }

        #endregion
    }
}
