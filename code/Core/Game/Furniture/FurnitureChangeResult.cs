using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Immutable furniture-level effect planned or committed by a furniture operation.
    /// </summary>
    public sealed class FurnitureChangeResult
    {
        #region Initialization

        /// <summary>
        /// Initializes a new furniture change result.
        /// </summary>
        /// <param name="kind">Whether the effect creates or removes furniture.</param>
        /// <param name="furnitureId">The authoritative furniture identity.</param>
        /// <param name="definitionId">The stable furniture definition identity.</param>
        /// <param name="anchor">The authoritative furniture anchor.</param>
        /// <param name="rotation">The furniture footprint rotation.</param>
        /// <param name="affectedCells">Every affected map cell in deterministic order.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="affectedCells"/> is null.</exception>
        internal FurnitureChangeResult(
            FurnitureChangeResultKind kind,
            FurnitureId furnitureId,
            FurnitureDefinitionId definitionId,
            MapCellCoord anchor,
            FootprintRotation rotation,
            IReadOnlyList<MapCellCoord> affectedCells)
        {
            ArgumentNullException.ThrowIfNull(affectedCells);

            Kind = kind;
            FurnitureId = furnitureId;
            DefinitionId = definitionId;
            Anchor = anchor;
            Rotation = rotation;
            AffectedCells = affectedCells.ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether this result represents creation or removal.
        /// </summary>
        public FurnitureChangeResultKind Kind { get; }

        /// <summary>
        /// Gets the authoritative furniture identity.
        /// </summary>
        public FurnitureId FurnitureId { get; }

        /// <summary>
        /// Gets the stable furniture definition identity.
        /// </summary>
        public FurnitureDefinitionId DefinitionId { get; }

        /// <summary>
        /// Gets the authoritative furniture anchor.
        /// </summary>
        public MapCellCoord Anchor { get; }

        /// <summary>
        /// Gets the furniture footprint rotation.
        /// </summary>
        public FootprintRotation Rotation { get; }

        /// <summary>
        /// Gets every affected map cell in deterministic footprint order.
        /// </summary>
        public IReadOnlyList<MapCellCoord> AffectedCells { get; }

        #endregion
    }
}
