using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CosmosCasino.Core.Save
{
    /// <summary>
    /// Represents the root container for all serialized game state.
    /// Acts as a shared data envelope passed between the save system and
    /// individual <see cref="ISaveParticipant"/> implementations.
    /// </summary>
    public sealed class GameSaveData
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GameSaveData"/> with the
        /// specified save format version.
        /// This constructor is used during both save creation and deserialization.
        /// </summary>
        /// <param name="version">The version of the save data format.</param>
        [JsonConstructor]
        public GameSaveData(int version)
        {
            Version = version;
        }

        /// <summary>
        /// Indicates the version of the save data format.
        /// Used to support future migrations and backward compatibility
        /// when the structure of saved data evolves.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Stores per-participant save data, keyed by a unique identifier
        /// owned by each <see cref="ISaveParticipant"/>.
        /// Each participant is responsible for defining, writing, and
        /// interpreting the structure of its own data.
        /// </summary>
        public Dictionary<string, JsonElement> Sections { get; set; } = new();
    }
}
