using System.Text;
using System.Text.Json;

namespace CosmosCasino.Core.Serialization
{
    /// <summary>
    /// JSON-based serializer used for persisting game data.
    /// Converts objects to and from UTF-8 encoded JSON byte arrays.
    /// This serializer is intentionally strict and fails fast when
    /// encountering invalid or corrupt data.
    /// </summary>
    internal sealed class JsonSaveSerializer : ISerializer
    {
        #region FIELDS

        /// <summary>
        /// Shared JSON serialization options used for all save operations.
        /// Configured for readability and tolerant property name matching.
        /// </summary>
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };

        #endregion

        #region METHODS

        /// <summary>
        /// Serializes the specified object into a UTF-8 encoded JSON byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="data">The object instance to serialize.</param>
        /// <returns>
        /// A byte array containing the UTF-8 encoded JSON representation
        /// of the provided object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="data"/> is null.
        /// </exception>
        byte[] ISerializer.Serialize<T>(T data)
        {
            ArgumentNullException.ThrowIfNull(data, "data");

            var json = JsonSerializer.Serialize(data, Options);

            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Deserializes a UTF-8 encoded JSON byte array into an object instance.
        /// </summary>
        /// <typeparam name="T">The expected type of the deserialized object.</typeparam>
        /// <param name="bytes">The UTF-8 encoded JSON data.</param>
        /// <returns>The deserialized object instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="bytes"/> is null.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Thrown when the byte array is empty or deserialization fails.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON data is malformed or incompatible with the
        /// target type.
        /// </exception>
        T ISerializer.Deserialize<T>(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));

            if (bytes.Length == 0)
            {
                throw new InvalidDataException("Cannot deserialize empty data.");
            }

            var json = Encoding.UTF8.GetString(bytes);
            var result = JsonSerializer.Deserialize<T>(json, Options);

            return result ?? throw new InvalidDataException("Failed to deserialize saved data.");
        }

        #endregion
    }
}
