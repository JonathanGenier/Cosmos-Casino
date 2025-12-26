using CosmosCasino.Core.Console.Command;
using NUnit.Framework;

namespace CosmosCasino.Tests.Console.Command
{
    [TestFixture]
    internal class ConsoleCommandResultTests
    {
        [Test]
        public void Ok_SetsSuccessTrue()
        {
            // Act
            var result = ConsoleCommandResult.Ok();

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void Failed_SetsSuccessFalse()
        {
            // Act
            var result = ConsoleCommandResult.Failed();

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Ok_Defaults_ShowInConsole_True()
        {
            // Act
            var result = ConsoleCommandResult.Ok();

            // Assert
            Assert.That(result.ShowInConsole, Is.True);
        }

        [Test]
        public void Ok_WithShowInConsoleFalse_SetsFlagCorrectly()
        {
            // Act
            var result = ConsoleCommandResult.Ok("cleared", showInConsole: false);

            // Assert
            Assert.That(result.ShowInConsole, Is.False);
        }

        [Test]
        public void Ok_WithNullMessage_UsesEmptyString()
        {
            // Arrange
            string? message = null;

            // Act
            var result = ConsoleCommandResult.Ok(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Failed_WithNullMessage_UsesEmptyString()
        {
            // Arrange
            string? message = null;

            // Act
            var result = ConsoleCommandResult.Failed(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Ok_WithMessage_PreservesMessage()
        {
            // Arrange
            const string message = "hello";

            // Act
            var result = ConsoleCommandResult.Ok(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(message));
        }

        [Test]
        public void Failed_WithMessage_PreservesMessage()
        {
            // Arrange
            const string message = "hello";

            // Act
            var result = ConsoleCommandResult.Failed(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(message));
        }

        [Test]
        public void Failed_Always_ShowsInConsole()
        {
            var result = ConsoleCommandResult.Failed("error");
            Assert.That(result.ShowInConsole, Is.True);
        }

    }
}
