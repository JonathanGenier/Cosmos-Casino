using CosmosCasino.Core.Configs;

namespace CosmosCasino.Core.Game.Map.Terrain.Generation
{
    /// <summary>
    /// Default terrain height generator based on deterministic value noise.
    /// This type provides a concrete implementation of <see cref="ITerrainHeightProvider"/>
    /// using seeded value noise to produce reproducible height values for world
    /// coordinates. The generated heights are quantized to discrete steps to ensure
    /// stability for downstream systems such as slope detection, pathing, and rendering.
    /// Noise characteristics are sourced from <see cref="TerrainConfigs"/> to allow
    /// centralized tuning without modifying generation logic.
    /// </summary>
    internal sealed class TerrainHeightGenerator : ITerrainHeightProvider
    {
        #region Fields

        private readonly ValueNoise2D _noise;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new terrain height generator using the specified seed.
        /// The seed controls the deterministic layout of the generated terrain;
        /// generators constructed with the same seed will produce identical height
        /// values for the same world coordinates.
        /// </summary>
        /// <param name="seed">
        /// Seed value used to initialize the underlying noise generator.
        /// </param>
        internal TerrainHeightGenerator(int seed)
        {
            _noise = new ValueNoise2D(seed);
        }

        #endregion

        #region ITerrainHeightProvider

        /// <summary>
        /// Samples the terrain height at the specified world coordinates.
        /// Height values are generated using multi-octave value noise and then
        /// quantized to fixed increments to provide stable, comparable results
        /// across the terrain system.
        /// </summary>
        /// <param name="x">
        /// World-space X coordinate at which to sample the terrain height.
        /// </param>
        /// <param name="y">
        /// World-space Y coordinate at which to sample the terrain height.
        /// </param>
        /// <returns>
        /// A quantized height value representing the terrain elevation at the
        /// specified world coordinate.
        /// </returns>
        float ITerrainHeightProvider.GetHeight(int x, int y)
        {
            float h = _noise.SampleOctaves(
                x,
                y,
                octaves: TerrainConfigs.NoiseOctaves,
                frequency: TerrainConfigs.NoiseFrequency,
                amplitude: TerrainConfigs.NoiseAmplitude,
                persistence: TerrainConfigs.NoisePersistence
            );

            return (float)(MathF.Round(h * 10f) / 2f);
        }

        #endregion
    }
}