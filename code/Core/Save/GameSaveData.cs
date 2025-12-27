using System.Text.Json;
using System.Text.Json.Serialization;

namespace CosmosCasino.Core.Save
{
    /// <summary>
    /// Represents the root container for all serialized game state.
    /// Acts as a shared data envelope passed between the save system and
    /// individual <see cref="ISaveParticipant"/> implementations.
    /// </summary>
    internal sealed class GameSaveData
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of <see cref="GameSaveData"/> for save creation.
        /// Used by <see cref="SaveManager"/> to aggregate participant state
        /// prior to serialization.
        /// </summary>
        /// <param name="version">
        /// The version of the save data format.
        /// </param>
        internal GameSaveData(int version)
        {
            Version = version;
            Sections = new();
        }

        /// <summary>
        /// Initializes a new <see cref="GameSaveData"/> instance from
        /// serialized save data.
        /// This constructor is invoked exclusively by
        /// <see cref="System.Text.Json"/> during deserialization.
        /// </summary>
        /// <param name="version">
        /// The version of the save data format.
        /// </param>
        /// <param name="sections">
        /// Serialized participant data keyed by participant identifier.
        /// </param>
        [JsonConstructor]
        internal GameSaveData(
            int version,
            Dictionary<string, JsonElement> sections)
        {
            Version = version;
            Sections = sections ?? new();
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Indicates the version of the save data format.
        /// Used for migrations and backward compatibility.
        /// 
        /// NOTE:
        /// This property is public solely to satisfy System.Text.Json
        /// constructor binding requirements. It is not part of the Core
        /// behavioral API.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Stores per-participant save data, keyed by participant identifier.
        /// Each participant owns the structure of its own data.
        /// 
        /// NOTE:
        /// This property is public only for serialization/deserialization.
        /// It is not intended for direct consumption outside the save pipeline.
        /// </summary>
        public Dictionary<string, JsonElement> Sections { get; set; } = new();

        #endregion
    }
}
