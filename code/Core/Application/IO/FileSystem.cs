namespace CosmosCasino.Core.Application.IO
{
    /// <summary>
    /// Provides low-level file system utilities with explicit, fail-fast behavior.
    /// Encapsulates safe atomic writes and controlled read operations while
    /// leaving error handling decisions to higher layers.
    /// </summary>
    internal static class FileSystem
    {
        #region METHODS

        /// <summary>
        /// Writes the provided bytes to disk atomically.
        /// The data is first written to a temporary file and then safely
        /// moved or replaced to ensure that the target file is never left
        /// in a partially written state.
        /// If the target file already exists, it is replaced.
        /// If it does not exist, it is created.
        /// </summary>
        /// <param name="path">The destination file path.</param>
        /// <param name="bytes">The data to write.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="bytes"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="path"/> is null, empty, or whitespace.
        /// </exception>
        internal static void AtomicSave(string path, byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

            var tempPath = path + ".tmp";

            try
            {
                WriteBytes(tempPath, bytes);

                if (File.Exists(path))
                {
                    File.Replace(tempPath, path, null);
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        /// <summary>
        /// Attempts to read all bytes from the specified file.
        /// Returns <c>false</c> if the file does not exist.
        /// Throws if the path is invalid or the file cannot be read.
        /// </summary>
        /// <param name="path">The file path to read.</param>
        /// <param name="bytes">
        /// When this method returns <c>true</c>, contains the bytes read from the file.
        /// When it returns <c>false</c>, contains an empty array.
        /// </param>
        /// <returns>
        /// <c>true</c> if the file exists and was read successfully; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="path"/> is null, empty, or whitespace.
        /// </exception>
        internal static bool TryReadBytes(string path, out byte[] bytes)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

            if (!File.Exists(path))
            {
                bytes = Array.Empty<byte>();
                return false;
            }

            bytes = File.ReadAllBytes(path);
            return true;
        }

        /// <summary>
        /// Writes bytes to the specified path, creating the parent directory
        /// if it does not already exist.
        /// This method assumes validated inputs and is intended for internal use only.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="bytes">The data to write.</param>
        private static void WriteBytes(string path, byte[] bytes)
        {
            var dir = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllBytes(path, bytes);
        }

        #endregion
    }
}
