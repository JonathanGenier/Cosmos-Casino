using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Immutable domain definition shared by furniture instances of the same type.
    /// </summary>
    public sealed class FurnitureDefinition
    {
        #region Initialization

        /// <summary>
        /// Initializes a new furniture definition.
        /// </summary>
        /// <param name="id">The stable definition identity.</param>
        /// <param name="footprint">The deterministic map-cell footprint used by furniture of this definition.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="footprint"/> is null.</exception>
        public FurnitureDefinition(FurnitureDefinitionId id, MapCellFootprint footprint)
        {
            ArgumentNullException.ThrowIfNull(footprint);

            Id = id;
            Footprint = footprint;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the stable identity for this definition.
        /// </summary>
        public FurnitureDefinitionId Id { get; }

        /// <summary>
        /// Gets the immutable deterministic footprint for furniture of this definition.
        /// </summary>
        public MapCellFootprint Footprint { get; }

        #endregion
    }
}
