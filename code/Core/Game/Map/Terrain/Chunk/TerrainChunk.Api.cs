using CosmosCasino.Core.Game.Map.Terrain.Tile;

namespace CosmosCasino.Core.Game.Map.Terrain.Chunk
{
    /// <summary>
    /// Public read-only view of a terrain chunk.
    /// Exposes the identity and tile collection of the chunk while preserving
    /// all internal structural invariants enforced at construction time.
    /// </summary>
    public sealed partial class TerrainChunk
    {
        #region Properties

        /// <summary>
        /// Gets the logical coordinate of this chunk within the terrain grid.
        /// </summary>
        public TerrainChunkGridCoord GridCoord { get; }

        /// <summary>
        /// Gets the world-space origin of this chunk used for rendering and spatial
        /// placement.
        /// This coordinate represents the centered visual anchor of the chunk and
        /// must not be used for tile indexing, simulation logic, or deterministic
        /// calculations.
        /// </summary>
        public TerrainChunkWorldOrigin WorldOrigin { get; }

        /// <summary>
        /// Gets the read-only collection of terrain tiles owned by this chunk.
        /// Tiles are stored in row-major order and are guaranteed to form a
        /// complete, validated grid.
        /// </summary>
        public IReadOnlyList<TerrainTile> Tiles { get; }

        #endregion
    }
}