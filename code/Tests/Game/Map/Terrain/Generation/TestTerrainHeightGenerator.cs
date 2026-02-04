using CosmosCasino.Core.Game.Map.Terrain.Generation;

namespace CosmosCasino.Tests.Game.Map.Terrain.Generation
{
    internal class TestTerrainHeightGenerator
    {
        private readonly Func<float, float, float> _heightFunc;

        public TestTerrainHeightGenerator(Func<float, float, float> heightFunc)
        {
            _heightFunc = heightFunc;
        }

        internal float GetHeight(float x, float y)
        {
            return _heightFunc(x, y);
        }
    }
}
