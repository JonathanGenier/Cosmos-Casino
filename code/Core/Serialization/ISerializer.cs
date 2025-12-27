namespace CosmosCasino.Core.Serialization
{
    /// <summary>
    /// Defines a strategy for converting objects to and from byte arrays.
    /// Implementations are responsible for choosing the underlying data
    /// format (e.g. JSON, binary, compressed, encrypted) while preserving
    /// round-trip integrity.
    /// </summary>
    internal interface ISerializer
    {
        #region METHODS

        /// <summary>
        /// Serializes the specified object into a byte array representation.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="data">The object instance to serialize.</param>
        /// <returns>
        /// A byte array containing the serialized representation of the object.
        /// </returns>
        internal byte[] Serialize<T>(T data);

        /// <summary>
        /// Deserializes the specified byte array into an object instance.
        /// </summary>
        /// <typeparam name="T">The expected type of the deserialized object.</typeparam>
        /// <param name="bytes">The serialized byte data.</param>
        /// <returns>The deserialized object instance.</returns>
        internal T Deserialize<T>(byte[] bytes);

        #endregion
    }
}
