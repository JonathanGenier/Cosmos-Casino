using CosmosCasino.Core.Debug.Logging;
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
        public GameManager(SaveManager saveManager) 
        {
            DevLog.Info("GameManager", "Initializing GameManager.");
            saveManager.Register(this);
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Restores game state from the provided save data.
        /// </summary>
        /// <param name="save">
        /// Save data containing previously persisted game state.
        /// </param>
        public void ReadFrom(GameSaveData save)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Persists current game state into the provided save data.
        /// </summary>
        /// <param name="save">
        /// Save data used to store the current game state.
        /// </param>
        public void WriteTo(GameSaveData save)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
