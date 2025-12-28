using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Save;

namespace CosmosCasino.Core.Game
{
    /// <summary>
    /// Coordinates core game state and participates in the save system
    /// to persist and restore game-related data.
    /// </summary>
    public sealed class GameManager : ISaveParticipant
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes the game manager and registers it with the save system.
        /// </summary>
        /// <param name="saveManager">
        /// Save manager responsible for coordinating save and load operations.
        /// </param>
        internal GameManager(SaveManager saveManager)
        {
            using (ConsoleLog.SystemScope(nameof(GameManager)))
            {
                saveManager.Register(this);
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Restores game state from the provided save data.
        /// </summary>
        /// <param name="save">
        /// Save data containing previously persisted game state.
        /// </param>
        void ISaveParticipant.ReadFrom(GameSaveData save)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Persists current game state into the provided save data.
        /// </summary>
        /// <param name="save">
        /// Save data used to store the current game state.
        /// </param>
        void ISaveParticipant.WriteTo(GameSaveData save)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
