using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CosmosCasino.Core.Save
{
    /// <summary>
    /// Provides typed helper methods for writing to and reading from
    /// save data sections stored as JSON elements.
    /// This class centralizes JSON serialization logic so individual
    /// save participants do not need to manually handle JsonElement
    /// conversion.
    /// </summary>
    public static class SaveSectionSerializer
    {
        /// <summary>
        /// Writes a value into the save sections dictionary under the specified key.
        /// If the key already exists, its value is overwritten.
        /// The value is serialized into a <see cref="JsonElement"/> internally.
        /// </summary>
        /// <typeparam name="T">The type of the value being written.</typeparam>
        /// <param name="sections">The save sections dictionary to write into.</param>
        /// <param name="key">The unique key identifying the saved value.</param>
        /// <param name="value">The value to serialize and store. May be null.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="sections"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="key"/> is null, empty, or whitespace.
        /// </exception>
        public static void Write<T>(this Dictionary<string, JsonElement> sections, string key, T value)
        {
            ArgumentNullException.ThrowIfNull(sections, nameof(sections));
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            sections[key] = JsonSerializer.SerializeToElement(value);
        }

        /// <summary>
        /// Reads and deserializes a value from the save sections dictionary.
        /// This method expects the key to exist and will throw if it does not.
        /// </summary>
        /// <typeparam name="T">The expected type of the stored value.</typeparam>
        /// <param name="sections">The save sections dictionary to read from.</param>
        /// <param name="key">The key identifying the stored value.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="sections"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="key"/> is null, empty, or whitespace.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the specified key does not exist in the dictionary.
        /// </exception>
        public static T Read<T>(this Dictionary<string, JsonElement> sections, string key)
        {
            ArgumentNullException.ThrowIfNull(sections, nameof(sections));
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            return sections[key].Deserialize<T>()!;
        }

        /// <summary>
        /// Attempts to read and deserialize a value from the save sections dictionary.
        /// This method does not throw if the key is missing and instead returns false.
        /// </summary>
        /// <typeparam name="T">The expected type of the stored value.</typeparam>
        /// <param name="sections">The save sections dictionary to read from.</param>
        /// <param name="key">The key identifying the stored value.</param>
        /// <param name="value">
        /// When this method returns, contains the deserialized value if the key
        /// was found; otherwise, the default value of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// True if the key exists and the value was read; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="sections"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="key"/> is null, empty, or whitespace.
        /// </exception>
        public static bool TryRead<T>(this Dictionary<string, JsonElement> sections, string key, out T value)
        {
            ArgumentNullException.ThrowIfNull(sections, nameof(sections));
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            if(!sections.TryGetValue(key, out var element))
            {
                value = default!;
                return false;
            }

            value = element.Deserialize<T>()!;
            return true;
        }
    }
}
