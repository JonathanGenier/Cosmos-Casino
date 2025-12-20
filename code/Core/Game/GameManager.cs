using CosmosCasino.Core.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosCasino.Core.Game
{
    public sealed class GameManager : ISaveParticipant
    {
        public GameManager(SaveManager saveManager) 
        {
            saveManager.Register(this);
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
