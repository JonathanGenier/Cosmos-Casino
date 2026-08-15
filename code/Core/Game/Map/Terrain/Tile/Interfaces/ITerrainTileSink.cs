using CosmosCasino.Core.Game.Map.Terrain;

namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Defines a sink for receiving generated terrain tiles during terrain
    /// generation, allowing systems to consume and store tiles as they are produced.
    /// </summary>
    internal interface ITerrainTileSink
    {
        /// <summary>
        /// Receives a generated terrain tile at the specified terrain world-tile coordinate.
        /// </summary>
        /// <param name="coord">The terrain world-tile coordinate of the generated terrain tile.</param>
        /// <param name="terrainTile">The generated terrain tile instance.</param>
        void ReceiveTerrainTile(TerrainTileWorldCoord coord, TerrainTile terrainTile);
    }
}
