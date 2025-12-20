using CosmosCasino.Core.Game;
using CosmosCasino.Core.Save;
using CosmosCasino.Core.Serialization;

namespace CosmosCasino.Core.Services
{
    public sealed class CoreServices
    {
        public SaveManager SaveManager { get; init; }
        public GameManager? GameManager { get; private set; }

        public CoreServices(ISerializer serializer, string savePath)
        {
            SaveManager = new SaveManager(serializer, savePath);
        }

        public void StartNewGame(SaveManager saveManager)
        {
            GameManager = new GameManager(saveManager);
        }

        public void EndGame()
        {
            GameManager = null;
        }
    }
}
