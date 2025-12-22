using CosmosCasino.Core.Debug.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug.Logging
{
    [TestFixture]
    internal class LogEntryTests
    {
        #region CONSTRUCTOR

        [Test]
        public void Constructor_Normalizes_NullCategory()
        {
            var entry = new LogEntry(
                timestampMs: 0,
                level: LogLevel.Info,
                safety: LogSafety.Safe,
                kind: LogKind.General,
                category: null!,
                message: null!
            );

            Assert.That(entry.Category, Is.EqualTo("Undefined"));
            Assert.That(entry.Message, Is.EqualTo(string.Empty));
        }

        #endregion
    }
}
