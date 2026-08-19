namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents a discrete vertical layer in the authoritative map domain.
    /// </summary>
    public readonly partial record struct Elevation
    {
        #region Initialization

        /// <summary>
        /// Initializes a new elevation with the specified whole world-unit value.
        /// </summary>
        /// <param name="value">The whole world-unit elevation value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is outside the inclusive supported range.
        /// </exception>
        public Elevation(int value)
            : this((float)value)
        {
        }

        /// <summary>
        /// Initializes a new elevation with the specified discrete world-unit value.
        /// </summary>
        /// <param name="value">A finite world-unit value aligned to a half-unit elevation step.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is non-finite, outside the inclusive supported range,
        /// or not aligned to a half-unit elevation step.
        /// </exception>
        public Elevation(float value)
        {
            _halfStepIndex = GetHalfStepIndex(value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the exact world-unit elevation represented by this discrete half-step value.
        /// </summary>
        public float Value => _halfStepIndex * StepSize;

        #endregion
    }
}
