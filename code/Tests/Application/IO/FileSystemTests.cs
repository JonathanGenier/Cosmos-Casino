using CosmosCasino.Core.Application.IO;
using NUnit.Framework;

namespace CosmosCasino.Tests.Application.IO
{
    [TestFixture]
    internal class FileSystemTests
    {
        #region FIELDS

        private string? _tempDir;
        private string? _filePath;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _filePath = Path.Combine(_tempDir, "save.dat");
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, recursive: true);
            }
        }

        #endregion

        #region ATOMIC SAVE

        [Test]
        public void AtomicSave_WhenBytesNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => FileSystem.AtomicSave(_filePath!, null!), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void AtomicSave_WhenPathIsNullOrWhitespace_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(() => FileSystem.AtomicSave(null!, new byte[] { 1 }), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => FileSystem.AtomicSave(string.Empty, new byte[] { 2 }), Throws.TypeOf<ArgumentException>());
            Assert.That(() => FileSystem.AtomicSave("   ", new byte[] { 3 }), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void AtomicSave_WhenFileDoesNotExist_CreatesFile()
        {
            // Arrange
            var bytes = new byte[] { 1, 2, 3 };

            // Act
            FileSystem.AtomicSave(_filePath!, bytes);

            // Assert
            Assert.That(File.Exists(_filePath), Is.True);
        }

        [Test]
        public void AtomicSave_WritesExactBytes()
        {
            // Arrange
            var bytes = new byte[] { 10, 20, 30 };

            // Act
            FileSystem.AtomicSave(_filePath!, bytes);

            var result = File.ReadAllBytes(_filePath!);

            // Assert
            Assert.That(result, Is.EqualTo(bytes));
        }

        [Test]
        public void AtomicSave_WhenFileExists_OverwritesContents()
        {
            // Act
            FileSystem.AtomicSave(_filePath!, new byte[] { 1 });
            FileSystem.AtomicSave(_filePath!, new byte[] { 2, 3 });

            var result = File.ReadAllBytes(_filePath!);

            // Assert
            Assert.That(result, Is.EqualTo(new byte[] { 2, 3 }));
        }

        [Test]
        public void AtomicSave_DoesNotLeaveTempFile()
        {
            // Act
            FileSystem.AtomicSave(_filePath!, new byte[] { 1 });

            // Assert
            Assert.That(File.Exists(_filePath + ".tmp"), Is.False);
        }

        #endregion

        #region TRY READ BYTES

        [Test]
        public void TryReadBytes_WhenPathInvalid_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(() => FileSystem.TryReadBytes(string.Empty, out _), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void TryReadBytes_WhenFileMissing_ReturnsFalseAndEmptyArray()
        {
            // Act
            var result = FileSystem.TryReadBytes(_filePath!, out var bytes);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(bytes, Is.Empty);
        }

        [Test]
        public void TryReadBytes_WhenFileExistsButEmpty_ReturnsTrueAndEmptyArray()
        {
            FileSystem.AtomicSave(_filePath!, Array.Empty<byte>());

            var result = FileSystem.TryReadBytes(_filePath!, out var bytes);

            Assert.That(result, Is.True);
            Assert.That(bytes, Is.Empty);
        }

        [Test]
        public void TryReadBytes_WhenFileExists_ReturnsTrueAndBytes()
        {
            // Arrange
            var expected = new byte[] { 9, 8, 7 };

            // Act
            FileSystem.AtomicSave(_filePath!, expected);

            var result = FileSystem.TryReadBytes(_filePath!, out var bytes);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(bytes, Is.EqualTo(expected));
        }

        #endregion
    }
}
