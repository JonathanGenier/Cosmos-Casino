using CosmosCasino.Core.IO;
using CosmosCasino.Core.Serialization;
using System.IO;
using System.Text.Json;

namespace CosmosCasino.Core.Save
{
    /// <summary>
    /// Coordinates save and load operations by delegating persistence
    /// responsibilities to registered <see cref="ISaveParticipant"/> instances
    /// Ensures save operations are atomic and never corrupt existing save data.
    /// Does not interpret game state, apply policy, or attempt recovery.
    /// </summary>
    public sealed class SaveManager
    {
        private readonly List<ISaveParticipant> _participants = new();
        private readonly ISerializer _serializer;
        private readonly string _path;

        /// <summary>
        /// Initializes a new <see cref="SaveManager"/> with the given serializer
        /// and target save path.
        /// </summary>
        /// <param name="serializer">
        /// Serializer used to convert save data to and from a byte representation.
        /// </param>
        /// <param name="path">
        /// Absolute path where the save file will be written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="serializer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="path"/> is null, empty, or whitespace.
        /// </exception>
        public SaveManager(ISerializer serializer, string path)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if(string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Save path cannot be null or whitespace.", nameof(path));
            }

            _serializer = serializer;
            _path = path;
        }

        /// <summary>
        /// Registers a participant that contributes data to save and load operations.
        /// Each participant may only be registered once.
        /// </summary>
        /// <param name="participant">
        /// The participant responsible for writing and reading its own save data.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="participant"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the participant has already been registered.
        /// </exception>
        public void Register(ISaveParticipant participant)
        {
            if(participant == null)
            {
                throw new ArgumentNullException(nameof(participant));
            }
                
            if(_participants.Contains(participant))
            {
                throw new InvalidOperationException("Participant already registered.");
            }

            _participants.Add(participant);
        }

        /// <summary>
        /// Saves the current game state by collecting data from all registered
        /// participants and writing it to disk atomically.
        /// If a save file already exists, it is replaced safely.
        /// If no participants are registered, an empty but valid save is written.
        /// </summary>
        public void Save()
        {
            var save = new GameSaveData(SaveVersions.Current);

            foreach(var participant in _participants)
            {
                participant.WriteTo(save);
            }

            var bytes = _serializer.Serialize(save);

            FileSystem.AtomicSave(_path, bytes);
        }

        /// <summary>
        /// Loads the game state from disk and restores it into all registered
        /// participants.
        /// If no save file exists, the operation completes without effect.
        /// </summary>
        public void Load()
        {
            if(!FileSystem.TryReadBytes(_path, out var bytes))
            {
                return;
            }

            GameSaveData save;

            try
            {
                save = _serializer.Deserialize<GameSaveData>(bytes);
            }
            catch(JsonException ex)
            {
                throw new InvalidDataException("Save file is invalid.", ex);
            }

            if(save.Version != SaveVersions.Current)
            {
                throw new InvalidDataException(
                    $"Unsupported save version {save.Version}. Expected {SaveVersions.Current}."
                );
            }

            foreach(var participant in _participants)
            {
                participant.ReadFrom(save);
            }
        }
    }
}
