using CosmosCasino.Core.Serialization;
using NUnit.Framework;
using System.Text;
using System.Text.Json;

namespace CosmosCasino.Tests.Serialization
{
    [TestFixture]
    internal class JsonSaveSerializerTests
    {
        JsonSaveSerializer? _serializer;

        private sealed class TestData
        {
            public int Value { get; set; }
        }

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _serializer = new JsonSaveSerializer();
        }

        #endregion

        #region SERIALIZE

        [Test]
        public void Serialize_DataIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => _serializer!.Serialize<object>(null!), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Serialize_ValidObject_ReturnsNonEmptyBytes()
        {
            // Arrange
            var obj = new TestData { Value = 42 };

            // Act
            var bytes = _serializer!.Serialize(obj);

            // Assert
            Assert.That(bytes, Is.Not.Null);
            Assert.That(bytes.Length, Is.GreaterThan(0));
        }

        #endregion

        #region DESERIALIZE

        [Test]
        public void Deserialize_BytesIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => _serializer!.Deserialize<object>(null!), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Deserialize_BytesIsEmpty_ThrowsInvalidDataException()
        {
            // Arrange
            var emptyBytes = Array.Empty<byte>();

            // Act & Assert
            Assert.That(() => _serializer!.Deserialize<object>(emptyBytes), Throws.TypeOf<InvalidDataException>());
        }

        [Test]
        public void Deserialize_InvalidJson_ThrowsJsonException()
        {
            // Arrange
            var invalidJsonBytes = Encoding.UTF8.GetBytes("NOT JSON");

            // Act & Assert
            Assert.That(() => _serializer!.Deserialize<TestData>(invalidJsonBytes), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void Deserialize_ValidJson_ReturnsObject()
        {
            // Arrange
            var original = new TestData { Value = 123 };
            var bytes = _serializer!.Serialize(original);

            // Act
            var result = _serializer!.Deserialize<TestData>(bytes);

            // Assert
            Assert.That(result.Value, Is.EqualTo(123));
        }

        #endregion

        #region Serialize & Deserialize

        [Test]
        public void SerializeDeserialize_RoundTrip_RestoresData()
        {
            var original = new TestData { Value = 99 };

            var bytes = _serializer!.Serialize(original);
            var result = _serializer!.Deserialize<TestData>(bytes);

            Assert.That(result.Value, Is.EqualTo(original.Value));
        }

        #endregion
    }
}
