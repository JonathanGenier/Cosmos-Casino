using CosmosCasino.Core.Game.Map.Terrain.Generation;
using CosmosCasino.Core.Game.Map.Terrain.Tile;

namespace CosmosCasino.Core.Game.Map.Systems
{
    /// <summary>
    /// Handles terrain generation and post-processing logic, including height-based
    /// tile creation and resolution of slope adjacency relationships between tiles.
    /// </summary>
    internal class TerrainSystem
    {
        #region Fields

        private static readonly (SlopeNeighborMask Mask, int Dx, int Dy)[] NeighborOffsets =
        {
            (SlopeNeighborMask.North, 0, -1),
            (SlopeNeighborMask.NorthEast, 1, -1),
            (SlopeNeighborMask.East, 1,  0),
            (SlopeNeighborMask.SouthEast, 1,  1),
            (SlopeNeighborMask.South, 0,  1),
            (SlopeNeighborMask.SouthWest, -1,  1),
            (SlopeNeighborMask.West, -1,  0),
            (SlopeNeighborMask.NorthWest, -1, -1),
        };

        #endregion

        #region Generation

        /// <summary>
        /// Generates terrain tiles for a square map using a deterministic height generator
        /// and emits the results to the provided terrain tile sink.
        /// </summary>
        /// <param name="seed">The seed used to deterministically generate terrain heights.</param>
        /// <param name="mapSize">The number of tiles per axis to generate.</param>
        /// <param name="sink">The sink that receives generated terrain tiles.</param>
        internal void GenerateTerrain(int seed, int mapSize, ITerrainTileSink sink)
        {
            var terrainHeightGenerator = new TerrainHeightGenerator(seed);

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    var coord = new MapCoord(x, y);

                    var topLeftHeight = terrainHeightGenerator.GetHeight(coord.X, coord.Y);
                    var topRightHeight = terrainHeightGenerator.GetHeight(coord.X + 1, coord.Y);
                    var bottomLeftHeight = terrainHeightGenerator.GetHeight(coord.X, coord.Y + 1);
                    var bottomRightHeight = terrainHeightGenerator.GetHeight(coord.X + 1, coord.Y + 1);
                    var terrainTile = new TerrainTile(topLeftHeight, topRightHeight, bottomLeftHeight, bottomRightHeight);

                    sink.ReceiveTerrainTile(coord, terrainTile);
                }
            }
        }

        #endregion

        #region Slope Neighbor Resolution

        /// <summary>
        /// Resolves slope adjacency information for terrain tiles by inspecting neighboring
        /// tiles and recording slope relationships on flat tiles.
        /// </summary>
        /// <param name="coords">The set of map coordinates to process.</param>
        /// <param name="tryGetTerrain">Function used to retrieve terrain tiles by coordinate.</param>
        internal void ResolveSlopeNeighbors(IEnumerable<MapCoord> coords, Func<MapCoord, TerrainTile?> tryGetTerrain)
        {
            foreach (var coord in coords)
            {
                var currentTerrain = tryGetTerrain(coord);

                if (currentTerrain == null)
                {
                    continue;
                }

                currentTerrain.ClearSlopeNeighborMask();

                // We only care about flat tiles
                if (currentTerrain.IsSlope)
                {
                    continue;
                }

                foreach (var (mask, dx, dy) in NeighborOffsets)
                {
                    var neighborCoord = new MapCoord(coord.X + dx, coord.Y + dy);
                    var neighborTerrain = tryGetTerrain(neighborCoord);

                    if (neighborTerrain?.IsSlope == true)
                    {
                        currentTerrain.AddSlopeNeighbor(mask);
                    }
                }
            }
        }

        #endregion
    }
}
