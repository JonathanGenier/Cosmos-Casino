namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Provides public conversion helpers for elevation and map-cell coordinates.
    /// </summary>
    public readonly partial record struct Elevation
    {
        #region Factories

        /// <summary>
        /// Initializes a new elevation from the authoritative vertical map-cell coordinate.
        /// </summary>
        /// <param name="mapCellY">The vertical map-cell coordinate to convert.</param>
        /// <returns>The elevation represented by <paramref name="mapCellY"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="mapCellY"/> resolves outside the supported elevation range.
        /// </exception>
        public static Elevation FromMapCellY(int mapCellY)
        {
            return new Elevation(mapCellY * StepSize);
        }

        #endregion
    }
}
