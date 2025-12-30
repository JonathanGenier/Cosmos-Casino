namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Identifies the material type of a floor tile within a map cell.
    /// Used to determine visuals, behavior, and placement rules.
    /// </summary>
    internal enum FloorType
    {
        /// <summary>
        /// Industrial metal flooring.
        /// Durable and suitable for high-traffic or mechanical areas.
        /// </summary>
        Metal,

        /// <summary>
        /// Wooden flooring.
        /// Provides a warmer aesthetic, typically used in decorative or low-stress areas.
        /// </summary>
        Wood,
    }
}
