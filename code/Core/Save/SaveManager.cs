using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.IO;
using CosmosCasino.Core.Serialization;
using System.Text.Json;

namespace CosmosCasino.Core.Save
{
    /// <summary>
    /// Coordinates save and load operations by delegating persistence
    /// responsibilities to registered <see cref="ISaveParticipant"/> instances
    /// Ensures save operations are atomic and never corrupt existing save data.
    /// Does not interpret game state, apply policy, or attempt recovery.
    /// </summary>
    internal sealed class SaveManager
    {
        #region FIELDS

        private readonly List<ISaveParticipant> _participants = new();
        private readonly ISerializer _serializer;
        private readonly string _path;

        #endregion

        #region CONSTRUCTORS

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
        internal SaveManager(ISerializer serializer, string path)
        {
            ArgumentNullException.ThrowIfNull(serializer, nameof(serializer));
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

            _serializer = serializer;
            _path = path;

            ConsoleLog.System("SaveManager", "Ready");
        }

        #endregion

        #region METHODS

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
        /// Thrown in debug builds when the participant has already been registered.
        /// </exception>
        internal void Register(ISaveParticipant participant)
        {
            ArgumentNullException.ThrowIfNull(participant, nameof(participant));

            if (_participants.Contains(participant))
            {
                ConsoleLog.Warning("Save", "Participant is already registered to SaveManager. No consequences, just a useless call.");
#if DEBUG
                throw new InvalidOperationException("Participant is already registered to SaveManager.");
#else
                return;
#endif
            }

            _participants.Add(participant);
        }

        /// <summary>
        /// Saves the current game state by collecting data from all registered
        /// participants and writing it to disk atomically.
        /// If a save file already exists, it is replaced safely.
        /// If no participants are registered, an empty but valid save is written.
        /// </summary>
        internal void Save()
        {
            var save = new GameSaveData(SaveVersions.Current);

            foreach (var participant in _participants)
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
        internal void Load()
        {
            if (!FileSystem.TryReadBytes(_path, out var bytes))
            {
                return;
            }

            GameSaveData save;

            try
            {
                save = _serializer.Deserialize<GameSaveData>(bytes);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Save file is invalid.", ex);
            }

            if (save.Version != SaveVersions.Current)
            {
                throw new InvalidDataException(
                    $"Unsupported save version {save.Version}. Expected {SaveVersions.Current}."
                );
            }

            foreach (var participant in _participants)
            {
                participant.ReadFrom(save);
            }
        }

        #endregion
    }
}
