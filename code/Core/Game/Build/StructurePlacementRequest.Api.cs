using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Immutable request to place one structure definition at one authoritative map-cell anchor.
    /// </summary>
    public sealed class StructurePlacementRequest
    {
        #region Initialization

        /// <summary>
        /// Initializes a new structure placement request.
        /// </summary>
        /// <param name="definition">The structure definition to place.</param>
        /// <param name="anchor">The authoritative map-cell anchor for the structure.</param>
        /// <param name="rotation">The footprint rotation to apply at placement time.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
        public StructurePlacementRequest(
            StructureDefinition definition,
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
        /// Gets the structure definition to place.
        /// </summary>
        public StructureDefinition Definition { get; }

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
