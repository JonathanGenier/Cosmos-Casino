using CosmosCasino.Core.Application.Console.Command;
using NUnit.Framework;

namespace CosmosCasino.Tests.Application.Console.Command
{
    [TestFixture]
    internal sealed class ClearConsoleCommandTests
    {
        #region FIELDS

        private const string CommandName = "clear";

        #endregion

        #region CONSTRUCTOR

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenDelegateIsNull()
        {
            // Assert
            Assert.That(() => new ClearConsoleCommand(null!), Throws.ArgumentNullException);
        }

        #endregion

        #region METADATA

        [Test]
        public void Command_Metadata_IsCorrect()
        {
            // Assemble
            IConsoleCommand command = new ClearConsoleCommand(() => true);

            // Assert
            Assert.That(command.Command, Is.EqualTo(CommandName));
            Assert.That(command.Description, Is.Not.Empty);
            Assert.That(command.Safety, Is.EqualTo(ConsoleCommandSafety.Safe));
        }

        #endregion

        #region EXECUTE

        [Test]
        public void Execute_WhenClearSucceeds_ReturnsOk()
        {
            // Assemble
            IConsoleCommand command = new ClearConsoleCommand(() => true);

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
            IConsoleCommand command = new ClearConsoleCommand(() => false);

            // Act
            var result = command.Execute(Array.Empty<string>());

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to clear logs"));
        }

        #endregion
    }
}
