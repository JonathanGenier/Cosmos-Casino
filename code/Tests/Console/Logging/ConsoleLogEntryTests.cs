using CosmosCasino.Core.Console.Logging;
using NUnit.Framework;

namespace CosmosCasino.Tests.Console.Logging
{
    [TestFixture]
    internal class ConsoleLogEntryTests
    {
        #region CONSTRUCTOR

        [Test]
        public void Constructor_Normalizes_NullCategory()
        {
            var entry = new ConsoleLogEntry(
                timestampMs: 0,
                level: ConsoleLogLevel.Info,
                safety: ConsoleLogSafety.Safe,
                kind: ConsoleLogKind.General,
                category: null!,
                message: null!
            );

            Assert.That(entry.Category, Is.EqualTo("Undefined"));
            Assert.That(entry.Message, Is.EqualTo(string.Empty));
        }

        #endregion
    }
}
