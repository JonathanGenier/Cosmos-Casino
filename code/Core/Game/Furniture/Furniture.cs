using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Authoritative instance of one logical furniture object placed in the map.
    /// </summary>
    internal sealed class Furniture
    {
        #region Initialization

        /// <summary>
        /// Initializes a new authoritative furniture instance.
        /// </summary>
        /// <param name="id">The unique furniture identity.</param>
        /// <param name="definition">The immutable definition shared by this furniture type.</param>
        /// <param name="anchor">The global logical map-cell anchor for this furniture.</param>
        /// <param name="rotation">The furniture footprint rotation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
        internal Furniture(
            FurnitureId id,
            FurnitureDefinition definition,
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
        /// Gets this furniture's unique authoritative identity.
        /// </summary>
        internal FurnitureId Id { get; }

        /// <summary>
        /// Gets the immutable furniture definition used by this instance.
        /// </summary>
        internal FurnitureDefinition Definition { get; }

        /// <summary>
        /// Gets the global logical map-cell anchor for this furniture.
        /// </summary>
        internal MapCellCoord Anchor { get; }

        /// <summary>
        /// Gets this furniture's cardinal footprint rotation.
        /// </summary>
        internal FootprintRotation Rotation { get; }

        #endregion

        #region Occupancy

        /// <summary>
        /// Resolves the occupied map cells derived from this furniture's definition, anchor, and rotation.
        /// </summary>
        /// <returns>The occupied global map-cell coordinates in deterministic footprint order.</returns>
        internal IReadOnlyList<MapCellCoord> ResolveOccupiedCells()
        {
            return Definition.Footprint.Resolve(Anchor, Rotation);
        }

        #endregion
    }
}
