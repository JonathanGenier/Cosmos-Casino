using CosmosCasino.Core.Debug.Logging;
using CosmosCasino.Core.Save;

namespace CosmosCasino.Core.Game
{
    public sealed class GameManager : ISaveParticipant
    {
        public GameManager(SaveManager saveManager) 
        {
            DevLog.Info("GameManager", "Initializing GameManager.");
            saveManager.Register(this);

            // Delete this comment. Used to trigger CI.
        }

        public void ReadFrom(GameSaveData save)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(GameSaveData save)
        {
            throw new NotImplementedException();
        }
    }
}
