using CosmosCasino.Core.Save;
using CosmosCasino.Core.Serialization;
using NUnit.Framework;
using System.Text.Json;

namespace CosmosCasino.Tests.Save
{

    [TestFixture]
    internal class SaveManagerTests
    {
        private string _tempPath;
        private JsonSaveSerializer _serializer;
        private SaveManager _saveManager;

        private sealed class TestSaveParticipant : ISaveParticipant
        {
            private readonly string _key;

            public int Value;

            public bool IsWriteCalled { get; private set; }
            public bool IsReadCalled { get; private set; }

            public TestSaveParticipant(string key)
            {
                _key = key;
                IsWriteCalled = false;
                IsReadCalled = false;
            }

            public void WriteTo(GameSaveData save)
            {
                IsWriteCalled = true;
                save.Sections.Write(_key, Value);
            }

            public void ReadFrom(GameSaveData save)
            {
                IsReadCalled = true;
                Value = save.Sections.Read<int>(_key);
            }
        }

        #region SETUP & TEARDOWN

        [SetUp]
        public void SetUp()
        {
            _tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
            _serializer = new JsonSaveSerializer();
            _saveManager = new SaveManager(_serializer, _tempPath);
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(_tempPath))
            {
                File.Delete(_tempPath);
            }
        }

        #endregion

        #region SAVEMANAGER

        [Test]
        public void SaveManager_WhenSerializerIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => new SaveManager(null!, _tempPath), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SaveManager_WhenFilePathIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(() => new SaveManager(_serializer, null!), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new SaveManager(_serializer, ""), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new SaveManager(_serializer, "   "), Throws.TypeOf<ArgumentException>());
        }

        #endregion

        #region REGISTER

        [Test]
        public void Register_WhenParticipantIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => _saveManager.Register(null!), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Register_WhenParticipantIsAlreadyRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            var participant = new TestSaveParticipant("p1");
            _saveManager.Register(participant);

            // Act & Assert
            Assert.That(() => _saveManager.Register(participant), Throws.TypeOf<InvalidOperationException>());
        }

        #endregion

        #region SAVE

        [Test]
        public void Save_WhenNoParticipantsRegistered_DoesNotThrow()
        {
            // Act & Assert
            Assert.That(() => _saveManager.Save(), Throws.Nothing);
        }

        [Test]
        public void Save_CallsWriteToOnRegisteredParticipants()
        {
            // Arrange
            var participant = new TestSaveParticipant("p1");
            _saveManager.Register(participant);

            // Act
            _saveManager.Save();

            // Assert
            Assert.That(participant.IsWriteCalled, Is.True);
        }

        [Test]
        public void Save_WhenNoExistingFile_CreatesSave()
        {
            // Arrange
            var participant = new TestSaveParticipant("p1") { Value = 123 };
            _saveManager.Register(participant);

            // Act
            _saveManager.Save();

            // Assert
            Assert.That(File.Exists(_tempPath), Is.True);
        }

        #endregion

        #region LOAD

        [Test]
        public void Load_CallsReadToOnRegisteredParticipants()
        {
            // Arrange
            var participant = new TestSaveParticipant("p1") { Value = 123 };
            _saveManager.Register(participant);

            // Act
            _saveManager.Save();
            _saveManager.Load();

            // Assert
            Assert.That(participant.IsReadCalled, Is.True);
        }

        [Test]
        public void Load_WhenFileDoesNotExist_DoesNotThrow()
        {
            // Arrange
            var participant = new TestSaveParticipant("p1");
            _saveManager.Register(participant);

            // Act & Assert
            Assert.That(() => _saveManager.Load(), Throws.Nothing);
            Assert.That(participant.IsReadCalled, Is.False);
        }

        [Test]
        public void Load_WhenFileIsInvalid_ThrowsJsonException()
        {
            // Arrange
            File.WriteAllText(_tempPath, "NOT VALID JSON");

            // Act & Assert
            Assert.That(
                () => _saveManager.Load(), 
                Throws.TypeOf<InvalidDataException>().With.InnerException.TypeOf<JsonException>()
                );
        }

        #endregion

        #region SAVE & LOAD

        [Test]
        public void Save_Load_SingleParticipant_RestoresParticipantState()
        {
            // Arrange
            var participant = new TestSaveParticipant("p1") { Value = 42 };
            _saveManager.Register(participant);

            // Act
            _saveManager.Save();
            participant.Value = 0;
            _saveManager.Load();

            // Assert
            Assert.That(participant.Value, Is.EqualTo(42));
        }

        [Test]
        public void Save_Load_WithMultipleParticipants_RestoresAll()
        {
            // Arrange
            var p1 = new TestSaveParticipant("P1") { Value = 1 };
            var p2 = new TestSaveParticipant("P2") { Value = 2 };
            _saveManager.Register(p1);
            _saveManager.Register(p2);

            // Act
            _saveManager.Save();
            p1.Value = 0;
            p2.Value = 0;
            _saveManager.Load();

            // Assert
            Assert.That(p1.Value, Is.EqualTo(1));
            Assert.That(p2.Value, Is.EqualTo(2));
        }

        #endregion
    }
}
