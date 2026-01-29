using CosmosCasino.Core.Game.Map.Terrain.Generation;

namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Internal implementation details for terrain tile generation.
    /// This partial definition contains lifecycle state, initialization,
    /// and generation logic used to populate the tile's height and slope data.
    /// All generation operations are one-time and enforced by internal guards.
    /// </summary>
    public sealed partial class TerrainTile
    {
        #region Fields

        /// <summary>
        /// Indicates whether height generation has already been performed.
        /// This flag enforces the invariant that a terrain tile's height data
        /// may only be generated once during its lifetime.
        /// </summary>
        private bool _heightGenerated;

        #endregion

        #region Initialization


        /// <summary>
        /// Initializes a new terrain tile with the specified local and world coordinates.
        /// The tile is created in an ungenerated state; height and slope data
        /// are populated later during the terrain generation phase.
        /// </summary>
        /// <param name="localCoord">
        /// The tile's coordinate relative to its containing chunk.
        /// </param>
        /// <param name="worldCoord">
        /// The tile's coordinate in world space, used for height sampling.
        /// </param>
        internal TerrainTile(TerrainTileLocalCoord localCoord, TerrainTileWorldCoord worldCoord)
        {
            LocalCoord = localCoord;
            WorldCoord = worldCoord;
        }

        #endregion

        #region Generation

        /// <summary>
        /// Generates and assigns the tile's corner height values using the provided
        /// terrain height provider.
        /// Height sampling is performed using the tile's world coordinates, and
        /// slope detection is derived from the resulting corner heights.
        /// This method may only be called once per tile instance.
        /// </summary>
        /// <param name="heightGenerator">
        /// The height provider used to sample terrain heights at world coordinates.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if height generation has already been performed for this tile.
        /// </exception>
        internal void GenerateHeights(ITerrainHeightProvider heightGenerator)
        {
            if (_heightGenerated)
            {
                throw new InvalidOperationException("TerrainTile heights already generated.");
            }

            TopLeftHeight = heightGenerator.GetHeight(WorldCoord.X, WorldCoord.Y);
            TopRightHeight = heightGenerator.GetHeight(WorldCoord.X + 1, WorldCoord.Y);
            BottomLeftHeight = heightGenerator.GetHeight(WorldCoord.X, WorldCoord.Y + 1);
            BottomRightHeight = heightGenerator.GetHeight(WorldCoord.X + 1, WorldCoord.Y + 1);
            IsSlope = TopLeftHeight != TopRightHeight || TopLeftHeight != BottomLeftHeight || TopLeftHeight != BottomRightHeight;

            _heightGenerated = true;
        }

        /// <summary>
        /// Records the presence of a neighboring sloped tile in the specified direction.
        /// This information is used by higher-level systems (e.g., rendering or
        /// smoothing logic) to determine how flat tiles should visually or
        /// logically transition into adjacent slopes.
        /// </summary>
        /// <param name="direction">
        /// The direction of the neighboring sloped tile to record.
        /// </param>
        internal void AddSlopeNeighbor(SlopeNeighborMask direction)
        {
            SlopeNeighbors |= direction;
        }

        /// <summary>
        /// Clears all slope neighbor flags, resetting the SlopeNeighbors property to indicate no neighbors are present.
        /// </summary>
        internal void ClearSlopeNeighbors()
        {
            SlopeNeighbors = SlopeNeighborMask.None;
        }

        #endregion
    }
}