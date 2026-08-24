using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Structures
{
    /// <summary>
    /// Provides canonical built-in structure definitions owned by Core.
    /// </summary>
    public static class StructureDefinitions
    {
        #region Fields

        /// <summary>
        /// The stable built-in definition identity for the basic structural Block.
        /// </summary>
        public const int BlockDefinitionIdValue = 1000;

        /// <summary>
        /// The stable built-in definition identity for the structural Pillar.
        /// </summary>
        public const int PillarDefinitionIdValue = 1001;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the stable built-in definition identity for the basic structural Block.
        /// </summary>
        public static StructureDefinitionId BlockDefinitionId { get; } = new(BlockDefinitionIdValue);

        /// <summary>
        /// Gets the stable built-in definition identity for the structural Pillar.
        /// </summary>
        public static StructureDefinitionId PillarDefinitionId { get; } = new(PillarDefinitionIdValue);

        /// <summary>
        /// Gets the canonical one-cell structural Block definition.
        /// </summary>
        public static StructureDefinition Block { get; } = new(
            BlockDefinitionId,
            new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0)
            }));

        /// <summary>
        /// Gets the canonical one-cell structural Pillar definition.
        /// </summary>
        public static StructureDefinition Pillar { get; } = new(
            PillarDefinitionId,
            new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0)
            }));

        #endregion
    }
}
