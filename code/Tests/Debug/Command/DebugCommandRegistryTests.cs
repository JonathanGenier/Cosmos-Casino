using CosmosCasino.Core.Debug;
using CosmosCasino.Core.Debug.Command;
using NUnit.Framework;
using System.Reflection;

namespace CosmosCasino.Tests.Debug.Command
{
    [TestFixture]
    internal sealed class DebugCommandRegistryTests
    {
        #region FIELDS

        private DebugCommandRegistry? _registry;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            var debugConsole = new ConsoleManager();
            _registry = new DebugCommandRegistry(debugConsole);
        }

        #endregion

        #region CONTRUCTOR

        [Test]
        public void Constructor_ThrowArgumentNullException_WhenDebugConsoleIsNull()
        {
            // Arrange
            ConsoleManager debugConsole = null!;

            // Assert
            Assert.That(() => new DebugCommandRegistry(debugConsole), Throws.ArgumentNullException);
        }

        #endregion

        #region EXECUTE

        [Test]
        public void Execute_EmptyInput_ReturnsFailed()
        {
            // Act
            var result1 = _registry!.Execute("   ");
            var result2 = _registry!.Execute(string.Empty);

            // Assert
            Assert.That(result1.Success, Is.False);
            Assert.That(result1.Message, Is.EqualTo("Empty command"));
            Assert.That(result2.Success, Is.False);
            Assert.That(result2.Message, Is.EqualTo("Empty command"));
        }

        [Test]
        public void Execute_UnknownCommand_ReturnsFailed()
        {
            // Act
            var result = _registry!.Execute("does.not.exist");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("Unknown command"));
        }

        [Test]
        public void Execute_ValidCommand_ReturnsCommandResult()
        {
            // Arrange
            var command = new TestCommand("test", CommandResult.Ok("ok"));
            RegisterCommandToRegistry(_registry!, command);

            // Act
            var result = _registry!.Execute("test");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("ok"));
        }

        [Test]
        public void Execute_CommandWithNoMessage_GeneratesFeedback_WhenSuccess()
        {
            // Arrange
            var command = new TestCommand("test", CommandResult.Ok());
            RegisterCommandToRegistry(_registry!, command);

            // Act
            var result = _registry!.Execute("test");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("returned no feedback"));
            Assert.That(result.Message, Does.Contain("succeeded"));
        }

        [Test]
        public void Execute_CommandWithNoMessage_GeneratesFeedback_WhenFailed()
        {
            // Arrange
            var command = new TestCommand("test", CommandResult.Failed());
            RegisterCommandToRegistry(_registry!, command);

            // Act
            var result = _registry!.Execute("test");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("returned no feedback"));
            Assert.That(result.Message, Does.Contain("failed"));
        }

        [Test]
        public void Execute_CommandWithExtraArguments_PassesArgumentsToCommand()
        {
            // Arrange
            var command = new TestArgsCommand("test");
            RegisterCommandToRegistry(_registry!, command);

            // Act
            var result = _registry!.Execute("test a b c");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("Received all args"));
        }

        #endregion

        #region REGISTER

#if DEBUG
        [Test]
        public void UnsafeCommand_IsRegistered_InDebug()
        {
            // Arrange
            var command = new UnsafeTestCommand();
            RegisterCommandToRegistry(_registry!, command);

            // Act
            var result = _registry!.Execute("unsafe");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("Should run in debug only."));
        }
#else
        [Test]
        public void UnsafeCommand_IsNotRegistered_InProd()
        {
            // Arrange
            var command = new UnsafeTestCommand();
            RegisterCommandToRegistry(_registry!, command);

            // Act
            var result = _registry!.Execute("unsafe");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("Unknown command"));
        }
#endif

        [Test]
        public void DuplicateCommandRegistration_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new TestCommand("duplicate", CommandResult.Ok("ok"));

            // Act
            RegisterCommandToRegistry(_registry!, command);

            // Assert
            var exception = Assert.Throws<TargetInvocationException>(() => RegisterCommandToRegistry(_registry!, command));

            Assert.That(exception!.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(exception.InnerException!.Message, Does.Contain("Duplicate command"));
        }

        #endregion

        #region HELPERS

        // Register via reflection (internal, intentional test seam)
        private static void RegisterCommandToRegistry(DebugCommandRegistry registry, IDebugCommand command)
        {
            registry.GetType()
                    .GetMethod("Register", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(registry, new object[] { command });
        }

        private sealed class TestCommand : IDebugCommand
        {
            private readonly CommandResult _result;

            public TestCommand(string command, CommandResult result)
            {
                Command = command;
                Description = "Test command";
                Safety = CommandSafety.Safe;
                _result = result;
            }

            public string Command { get; }
            public string Description { get; }
            public CommandSafety Safety { get; }

            public CommandResult Execute(string[] args) => _result;
        }

        private sealed class UnsafeTestCommand : IDebugCommand
        {
            public string Command => "unsafe";
            public string Description => "Unsafe command";
            public CommandSafety Safety => CommandSafety.Unsafe;

            public CommandResult Execute(string[] args)
                => CommandResult.Ok("Should run in debug only.");
        }

        private sealed class TestArgsCommand : IDebugCommand
        {
            public TestArgsCommand(string command)
            {
                Command = command;
                Description = "Test command";
                Safety = CommandSafety.Safe;
            }

            public string Command { get; }
            public string Description { get; }
            public CommandSafety Safety { get; }

            public CommandResult Execute(string[] args)
            {
                return args.SequenceEqual(new[] { "a", "b", "c" })
                    ? CommandResult.Ok("Received all args")
                    : CommandResult.Failed("Did not receive args");
            }
        }

        #endregion
    }
}
