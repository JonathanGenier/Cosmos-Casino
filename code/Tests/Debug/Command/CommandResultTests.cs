using CosmosCasino.Core.Debug.Command;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug.Command
{
    [TestFixture]
    internal class CommandResultTests
    {
        [Test]
        public void Ok_SetsSuccessTrue()
        {
            // Act
            var result = CommandResult.Ok();

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void Failed_SetsSuccessFalse()
        {
            // Act
            var result = CommandResult.Failed();

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Ok_WithNullMessage_UsesEmptyString()
        {
            // Arrange
            string? message = null;

            // Act
            var result = CommandResult.Ok(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Failed_WithNullMessage_UsesEmptyString()
        {
            // Arrange
            string? message = null;

            // Act
            var result = CommandResult.Failed(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Ok_WithMessage_PreservesMessage()
        {
            // Arrange
            const string message = "hello";

            // Act
            var result = CommandResult.Ok(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(message));
        }

        [Test]
        public void Failed_WithMessage_PreservesMessage()
        {
            // Arrange
            const string message = "hello";

            // Act
            var result = CommandResult.Failed(message);

            // Assert
            Assert.That(result.Message, Is.EqualTo(message));
        }
    }
}
