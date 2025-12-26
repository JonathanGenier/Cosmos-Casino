using CosmosCasino.Core.Debug.Command;
using CosmosCasino.Core.Debug.Command.Commands.Log;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug.Command.Commands.Log
{
    [TestFixture]
    internal sealed class LogClearCommandTests
    {
        #region CONSTRUCTOR

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenDelegateIsNull()
        {
            // Assert
            Assert.That(() => new LogClearCommand(null!), Throws.ArgumentNullException);
        }

        #endregion

        #region METADATA

        [Test]
        public void Command_Metadata_IsCorrect()
        {
            // Assemmble
            var command = new LogClearCommand(() => true);

            // Assert
            Assert.That(command.Command, Is.EqualTo("log.clear"));
            Assert.That(command.Description, Is.Not.Empty);
            Assert.That(command.Safety, Is.EqualTo(CommandSafety.Safe));
        }

        #endregion

        #region EXECUTE

        [Test]
        public void Execute_WhenClearSucceeds_ReturnsOk()
        {
            // Assemble
            var command = new LogClearCommand(() => true);

            // Act
            var result = command.Execute(Array.Empty<string>());

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Logs cleared"));
        }

        [Test]
        public void Execute_WhenClearFails_ReturnsFailed()
        {
            // Assemble
            var command = new LogClearCommand(() => false);

            // Act
            var result = command.Execute(Array.Empty<string>());

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to clear logs"));
        }

        #endregion
    }
}
