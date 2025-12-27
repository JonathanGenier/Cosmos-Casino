using CosmosCasino.Core.Save;
using NUnit.Framework;
using System.Text.Json;

namespace CosmosCasino.Tests.Save
{
    [TestFixture]
    internal class SaveSectionSerializerTests
    {
        #region FIELDS

        private Dictionary<string, JsonElement>? _sections;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _sections = new Dictionary<string, JsonElement>();
        }

        #endregion

        #region WRITE

        [Test]
        public void Write_SectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            _sections = null!;

            // Act & Assert
            Assert.That(() => _sections.Write("test", 1), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]

        public void Write_KeyIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(() => _sections!.Write(null!, 1), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => _sections!.Write(string.Empty, 1), Throws.TypeOf<ArgumentException>());
            Assert.That(() => _sections!.Write("   ", 1), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Write_AddsNewValue()
        {
            // Act
            _sections!.Write("test", 3);

            // Assert
            var result = _sections!.Read<int>("test");
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void Write_OverridesExistingValue()
        {
            // Arrange
            _sections!.Write("test", 1);
            _sections!.Write("test", 2);

            // Act
            var result = _sections!.Read<int>("test");

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        #endregion

        #region READ

        [Test]
        public void Read_SectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            _sections = null!;

            // Act & Assert
            Assert.That(() => _sections.Read<int>("missing"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]

        public void Read_KeyIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(() => _sections!.Read<int>(null!), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => _sections!.Read<int>(string.Empty), Throws.TypeOf<ArgumentException>());
            Assert.That(() => _sections!.Read<int>("   "), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Read_WhenKeyMissing_Throws()
        {
            // Act & Assert
            Assert.That(() => _sections!.Read<int>("missing"), Throws.TypeOf<KeyNotFoundException>());
        }

        #endregion

        #region TRY READ
        [Test]
        public void TryRead_SectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            _sections = null!;

            // Act & Assert
            Assert.That(() => _sections.TryRead<int>("missing", out _), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]

        public void TryRead_KeyIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(() => _sections!.TryRead<int>(null!, out _), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => _sections!.TryRead<int>(string.Empty, out _), Throws.TypeOf<ArgumentException>());
            Assert.That(() => _sections!.TryRead<int>("   ", out _), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void TryRead_WhenKeyMissing_ReturnsFalse()
        {
            // Act
            var success = _sections!.TryRead<int>("missing", out var value);

            // Assert
            Assert.That(success, Is.False);
            Assert.That(value, Is.EqualTo(default(int)));
        }

        [Test]
        public void TryRead_WhenKeyExists_ReturnsTrueAndValue()
        {
            // Arrange
            _sections!.Write("test", 42);

            // Act
            var success = _sections!.TryRead<int>("test", out var value);

            // Assert
            Assert.That(success, Is.True);
            Assert.That(value, Is.EqualTo(42));
        }

        #endregion

        #region WRITE & READ

        [Test]
        public void Write_Read_RoundTrip_ReturnsValue()
        {
            // Arrange
            _sections!.Write("test", 100);

            // Act
            var result = _sections!.Read<int>("test");

            // Assert
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void Write_Read_ValueIsNullable_RoundTripIsNull()
        {
            // Arrange
            var sections = new Dictionary<string, JsonElement>();

            // Act
            sections.Write<object>("nullable", null!);
            var result = sections.Read<object>("nullable");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region HELPERS

        private sealed class TestData
        {
            public string Name { get; set; } = "TestParticipant";
        }

        #endregion
    }
}
