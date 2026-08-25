using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Provides canonical built-in furniture definitions owned by Core.
    /// </summary>
    public static class FurnitureDefinitions
    {
        #region Fields

        /// <summary>
        /// The stable built-in definition identity for the Casino Table.
        /// </summary>
        public const int CasinoTableDefinitionIdValue = 2000;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the stable built-in definition identity for the Casino Table.
        /// </summary>
        public static FurnitureDefinitionId CasinoTableDefinitionId { get; } = new(CasinoTableDefinitionIdValue);

        /// <summary>
        /// Gets the canonical multi-cell Casino Table definition.
        /// </summary>
        public static FurnitureDefinition CasinoTable { get; } = new(
            CasinoTableDefinitionId,
            new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(0, 0, 1),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(1, 0, 1),
                new MapCellOffset(2, 0, 0),
                new MapCellOffset(2, 0, 1)
            }));

        #endregion
    }
}
