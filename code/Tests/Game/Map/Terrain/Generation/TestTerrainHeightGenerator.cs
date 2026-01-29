using CosmosCasino.Core.Game.Map.Terrain.Generation;

namespace CosmosCasino.Tests.Game.Map.Terrain.Generation
{
    internal class TestTerrainHeightGenerator : ITerrainHeightProvider
    {
        private readonly Func<int, int, float> _heightFunc;

        public TestTerrainHeightGenerator(Func<int, int, float> heightFunc)
        {
            _heightFunc = heightFunc;
        }

        float ITerrainHeightProvider.GetHeight(int x, int y)
        {
            return _heightFunc(x, y);
        }
    }
}
