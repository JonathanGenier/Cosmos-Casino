using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Immutable request to place one furniture definition at one authoritative map-cell anchor.
    /// </summary>
    public sealed class FurniturePlacementRequest
    {
        #region Initialization

        /// <summary>
        /// Initializes a new furniture placement request.
        /// </summary>
        /// <param name="definition">The furniture definition to place.</param>
        /// <param name="anchor">The authoritative map-cell anchor for the furniture.</param>
        /// <param name="rotation">The footprint rotation to apply at placement time.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
        public FurniturePlacementRequest(
            FurnitureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            ArgumentNullException.ThrowIfNull(definition);

            Definition = definition;
            Anchor = anchor;
            Rotation = rotation;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the furniture definition to place.
        /// </summary>
        public FurnitureDefinition Definition { get; }

        /// <summary>
        /// Gets the authoritative map-cell anchor.
        /// </summary>
        public MapCellCoord Anchor { get; }

        /// <summary>
        /// Gets the footprint rotation.
        /// </summary>
        public FootprintRotation Rotation { get; }

        #endregion
    }
}
