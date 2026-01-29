using CosmosCasino.Core.Configs;

namespace CosmosCasino.Core.Game.Map.Terrain.Tile
{
    /// <summary>
    /// Public, read-only view of a chunk-local terrain tile coordinate.
    /// This type exposes validated tile coordinates and derived indexing
    /// information while preserving all invariants enforced by the
    /// internal constructors.
    /// </summary>
    /// <remarks>
    /// Instances of this type are guaranteed to represent a valid tile
    /// position within a single terrain chunk. Consumers may safely
    /// use <see cref="Index"/> for direct indexing into chunk tile
    /// storage without additional bounds checks.
    /// </remarks>
    public readonly partial record struct TerrainTileLocalCoord
    {
        #region Properties

        /// <summary>
        /// Gets the local X coordinate of the terrain tile within its chunk.
        /// </summary>
        public uint X { get; }

        /// <summary>
        /// Gets the local Y coordinate of the terrain tile within its chunk.
        /// </summary>
        public uint Y { get; }

        /// <summary>
        /// Gets the linear, row-major index of this tile within the chunk's
        /// tile array.
        /// </summary>
        /// <remarks>
        /// The index is computed as:
        /// <c>X + (Y × ChunkSize)</c>.
        /// This value is guaranteed to be within the bounds of the chunk's
        /// tile collection.
        /// </remarks>
        public int Index => (int)(X + (Y * TerrainConfigs.ChunkSize));

        #endregion
    }
}