using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Structures
{
    /// <summary>
    /// Immutable domain definition shared by structure instances of the same type.
    /// </summary>
    internal sealed class StructureDefinition
    {
        #region Initialization

        /// <summary>
        /// Initializes a new structure definition.
        /// </summary>
        /// <param name="id">The stable definition identity.</param>
        /// <param name="footprint">The deterministic map-cell footprint used by structures of this definition.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="footprint"/> is null.</exception>
        internal StructureDefinition(StructureDefinitionId id, MapCellFootprint footprint)
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
        internal StructureDefinitionId Id { get; }

        /// <summary>
        /// Gets the immutable deterministic footprint for structures of this definition.
        /// </summary>
        internal MapCellFootprint Footprint { get; }

        #endregion
    }
}
