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

        #endregion

        #region Validation

        /// <summary>
        /// Validates the specified discrete elevation value.
        /// </summary>
        /// <param name="value">The discrete elevation value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is outside the inclusive supported range.
        /// </exception>
        private static void ValidateValue(int value)
        {
            if (value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"Elevation must be between {MinValue} and {MaxValue}.");
            }
        }

        #endregion
    }
}
