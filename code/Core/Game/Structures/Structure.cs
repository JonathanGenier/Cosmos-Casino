using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Structures
{
    /// <summary>
    /// Authoritative instance of one logical structure placed in the map.
    /// </summary>
    internal sealed class Structure
    {
        #region Initialization

        /// <summary>
        /// Initializes a new authoritative structure instance.
        /// </summary>
        /// <param name="id">The unique structure identity.</param>
        /// <param name="definition">The immutable definition shared by this structure type.</param>
        /// <param name="anchor">The global logical map-cell anchor for this structure.</param>
        /// <param name="rotation">The structure's cardinal footprint rotation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
        internal Structure(
            StructureId id,
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            ArgumentNullException.ThrowIfNull(definition);

            Id = id;
            Definition = definition;
            Anchor = anchor;
            Rotation = rotation;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets this structure's unique authoritative identity.
        /// </summary>
        internal StructureId Id { get; }

        /// <summary>
        /// Gets the immutable structure definition used by this instance.
        /// </summary>
        internal StructureDefinition Definition { get; }

        /// <summary>
        /// Gets the global logical map-cell anchor for this structure.
        /// </summary>
        internal MapCellCoord Anchor { get; }

        /// <summary>
        /// Gets this structure's cardinal footprint rotation.
        /// </summary>
        internal FootprintRotation Rotation { get; }

        #endregion

        #region Occupancy

        /// <summary>
        /// Resolves the occupied map cells derived from this structure's definition, anchor, and rotation.
        /// </summary>
        /// <returns>The occupied global map-cell coordinates in deterministic footprint order.</returns>
        internal IReadOnlyList<MapCellCoord> ResolveOccupiedCells()
        {
            return Definition.Footprint.Resolve(Anchor, Rotation);
        }

        #endregion
    }
}
