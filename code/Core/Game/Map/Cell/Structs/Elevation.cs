namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents a discrete vertical layer in the authoritative map domain.
    /// </summary>
    internal readonly record struct Elevation
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

        #region Initialization

        /// <summary>
        /// Initializes a new elevation with the specified discrete value.
        /// </summary>
        /// <param name="value">The discrete elevation value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is outside the inclusive supported range.
        /// </exception>
        internal Elevation(int value)
        {
            if (value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"Elevation must be between {MinValue} and {MaxValue}.");
            }

            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the discrete elevation value.
        /// </summary>
        internal int Value { get; }

        #endregion
    }
}
