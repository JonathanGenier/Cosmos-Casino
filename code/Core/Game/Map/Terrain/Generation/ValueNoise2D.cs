namespace CosmosCasino.Core.Game.Map.Terrain.Generation
{
    /// <summary>
    /// Deterministic two-dimensional value noise generator.
    /// This type provides a lightweight, engine-agnostic implementation of
    /// seeded value noise intended for one-time terrain generation and
    /// preprocessing. Given the same seed and input coordinates, it will
    /// always produce identical output values.
    /// This class is designed to be disposable after use and is not intended
    /// for real-time or per-frame sampling.
    /// </summary>
    internal sealed class ValueNoise2D
    {
        #region Fields

        private readonly int _seed;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new value noise generator with the specified seed.
        /// The seed controls the deterministic layout of the generated noise;
        /// generators constructed with the same seed will produce identical
        /// results for the same input coordinates.
        /// </summary>
        /// <param name="seed">
        /// Seed value used to initialize the noise generator.
        /// </param>
        internal ValueNoise2D(int seed)
        {
            _seed = seed;
        }

        #endregion

        /// <summary>
        /// Samples the value noise function at the specified coordinates.
        /// The returned value is produced by hashing the surrounding lattice
        /// points and interpolating between them using a smooth interpolation
        /// curve.
        /// </summary>
        /// <param name="x">
        /// X coordinate at which to sample the noise.
        /// </param>
        /// <param name="y">
        /// Y coordinate at which to sample the noise.
        /// </param>
        /// <returns>
        /// A deterministic noise value in the range [0, 1].
        /// </returns>
        internal float Sample(float x, float y)
        {
            int x0 = FastFloor(x);
            int y0 = FastFloor(y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            float sx = Smooth(x - x0);
            float sy = Smooth(y - y0);

            float n00 = Hash(x0, y0);
            float n10 = Hash(x1, y0);
            float n01 = Hash(x0, y1);
            float n11 = Hash(x1, y1);

            float ix0 = Lerp(n00, n10, sx);
            float ix1 = Lerp(n01, n11, sx);

            return Lerp(ix0, ix1, sy);
        }

        /// <summary>
        /// Samples multi-octave value noise at the specified coordinates.
        /// This method layers multiple noise samples at increasing frequencies
        /// and decreasing amplitudes to produce more complex, natural-looking
        /// terrain variation.
        /// </summary>
        /// <param name="x">
        /// Base X coordinate at which to sample the noise.
        /// </param>
        /// <param name="y">
        /// Base Y coordinate at which to sample the noise.
        /// </param>
        /// <param name="octaves">
        /// Number of noise layers to combine.
        /// </param>
        /// <param name="frequency">
        /// Initial frequency applied to the input coordinates.
        /// </param>
        /// <param name="amplitude">
        /// Initial amplitude applied to the noise output.
        /// </param>
        /// <param name="persistence">
        /// Multiplier applied to the amplitude after each octave.
        /// </param>
        /// <returns>
        /// A normalized noise value resulting from the combined octaves.
        /// </returns>
        internal float SampleOctaves(
            float x,
            float y,
            int octaves,
            float frequency,
            float amplitude,
            float persistence)
        {
            float value = 0f;
            float max = 0f;

            for (int i = 0; i < octaves; i++)
            {
                value += Sample(x * frequency, y * frequency) * amplitude;
                max += amplitude;

                amplitude *= persistence;
                frequency *= 2f;
            }

            return value / max;
        }

        #region Helpers

        /// <summary>
        /// Applies a smoothstep interpolation curve to the given value.
        /// </summary>
        /// <param name="t">
        /// Interpolation factor in the range [0, 1].
        /// </param>
        /// <returns>
        /// A smoothed interpolation value.
        /// </returns>
        private static float Smooth(float t)
        {
            return t * t * (3f - (2f * t));
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="a">
        /// Start value.
        /// </param>
        /// <param name="b">
        /// End value.
        /// </param>
        /// <param name="t">
        /// Interpolation factor in the range [0, 1].
        /// </param>
        /// <returns>
        /// The interpolated value.
        /// </returns>
        private static float Lerp(float a, float b, float t)
        {
            return a + ((b - a) * t);
        }

        /// <summary>
        /// Computes the largest integer less than or equal to the specified value.
        /// </summary>
        /// <param name="f">
        /// Floating-point value to floor.
        /// </param>
        /// <returns>
        /// The floored integer value.
        /// </returns>
        private static int FastFloor(float f)
        {
            return f >= 0 ? (int)f : (int)f - 1;
        }

        /// <summary>
        /// Computes a deterministic pseudo-random value for the given lattice
        /// coordinates using the configured seed.
        /// </summary>
        /// <param name="x">
        /// Integer X coordinate of the lattice point.
        /// </param>
        /// <param name="y">
        /// Integer Y coordinate of the lattice point.
        /// </param>
        /// <returns>
        /// A deterministic pseudo-random value in the range [0, 1].
        /// </returns>
        private float Hash(int x, int y)
        {
            unchecked
            {
                int h = _seed;
                h ^= x * 374761393;
                h ^= y * 668265263;
                h = (h ^ (h >> 13)) * 1274126177;
                return (h & 0x7fffffff) / (float)int.MaxValue;
            }
        }

        #endregion
    }
}