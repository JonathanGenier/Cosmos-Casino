namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents a discrete vertical layer in the authoritative map domain.
    /// </summary>
    public readonly partial record struct Elevation
    {
        #region Constants

        /// <summary>
        /// Gets the minimum supported elevation value.
        /// </summary>
        internal const int MinValue = -20;

        /// <summary>
        /// Gets the maximum supported elevation value.
        /// </summary>
        internal const int MaxValue = 20;

        /// <summary>
        /// Gets the physical world-space size of one discrete elevation step.
        /// </summary>
        internal const float StepSize = WorldGridMetrics.VerticalGridUnitSize;

        #endregion

        #region Fields

        private readonly int _halfStepIndex;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the vertical <see cref="MapCellCoord.Y"/> coordinate represented by this elevation.
        /// </summary>
        internal int MapCellY => _halfStepIndex;

        #endregion

        #region Factories

        /// <summary>
        /// Floors a world height to the nearest supported discrete elevation at or below it.
        /// </summary>
        /// <param name="worldHeight">The world-space height to resolve.</param>
        /// <returns>The discrete elevation at or immediately below <paramref name="worldHeight"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="worldHeight"/> is non-finite or floors outside the supported range.
        /// </exception>
        internal static Elevation FloorFromWorldHeight(float worldHeight)
        {
            if (!float.IsFinite(worldHeight))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(worldHeight),
                    worldHeight,
                    "World height must be finite.");
            }

            float flooredValue = MathF.Floor(worldHeight / StepSize) * StepSize;
            return new Elevation(flooredValue);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Resolves the exact half-step index represented by the specified elevation value.
        /// </summary>
        /// <param name="value">The discrete elevation value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is outside the inclusive supported range.
        /// </exception>
        private static int GetHalfStepIndex(float value)
        {
            if (!float.IsFinite(value) || value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"Elevation must be finite and between {MinValue} and {MaxValue}.");
            }

            float halfStepIndex = value / StepSize;

            if (halfStepIndex != MathF.Truncate(halfStepIndex))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"Elevation must align to {StepSize} world-unit increments.");
            }

            return (int)halfStepIndex;
        }

        #endregion
    }
}
