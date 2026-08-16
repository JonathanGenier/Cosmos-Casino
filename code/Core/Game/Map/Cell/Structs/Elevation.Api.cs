namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents a discrete vertical layer in the authoritative map domain.
    /// </summary>
    public readonly partial record struct Elevation
    {
        #region Initialization

        /// <summary>
        /// Initializes a new elevation with the specified discrete value.
        /// </summary>
        /// <param name="value">The discrete elevation value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is outside the inclusive supported range.
        /// </exception>
        public Elevation(int value)
        {
            ValidateValue(value);
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the discrete elevation value.
        /// </summary>
        public int Value { get; }

        #endregion
    }
}
