namespace CosmosCasino.Core.Configs
{
    /// <summary>
    /// Defines global configuration values for terrain generation
    /// and terrain-related spatial layout.
    /// These values represent tuning parameters used by terrain
    /// generation systems and are intended to be adjusted during
    /// development and balancing rather than at runtime.
    /// </summary>
    public static class TerrainConfigs
    {
        /// <summary>
        /// Current legacy terrain-render/storage chunk dimension.
        /// Terrain chunks use their existing centered coordinate partition until terrain ownership is migrated in CC-88.
        /// </summary>
        public const int ChunkSize = 15;

        /// <summary>
        /// Number of terrain chunks generated along one axis of the playable map.
        /// 
        /// The total playable terrain size is derived from this value in
        /// combination with <see cref="ChunkSize"/>.
        /// </summary>
        public const int ChunkCountPerAxis = 15;

        /// <summary>
        /// Number of noise octaves used when generating terrain height values.
        /// 
        /// Higher values increase terrain detail at the cost of additional
        /// computation during generation.
        /// </summary>
        public const int NoiseOctaves = 7;

        /// <summary>
        /// Base frequency applied to terrain noise sampling.
        /// 
        /// Lower values produce broader, smoother terrain features,
        /// while higher values result in more tightly packed variation.
        /// </summary>
        public const float NoiseFrequency = 0.03f;

        /// <summary>
        /// Base amplitude applied to terrain noise output.
        /// 
        /// This value controls the overall vertical scale of terrain
        /// elevation before any quantization or post-processing.
        /// </summary>
        public const float NoiseAmplitude = 50.0f;

        /// <summary>
        /// Persistence factor applied between successive noise octaves.
        /// 
        /// Each additional octave has its amplitude multiplied by this
        /// value, controlling how quickly higher-frequency detail
        /// diminishes.
        /// </summary>
        public const float NoisePersistence = 0.5f;

        /// <summary>
        /// Total number of terrain tiles generated along one axis of the playable map.
        /// This value must be odd so the map has one unique center tile.
        /// </summary>
        public const int TileCountPerAxis = ChunkSize * ChunkCountPerAxis;
    }
}
