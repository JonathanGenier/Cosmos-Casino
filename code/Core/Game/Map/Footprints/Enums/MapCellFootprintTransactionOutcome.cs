namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Describes the aggregate result of validating or applying a map-cell footprint transaction.
    /// </summary>
    internal enum MapCellFootprintTransactionOutcome
    {
        /// <summary>
        /// Every resolved cell can be mutated by the transaction.
        /// </summary>
        Valid,

        /// <summary>
        /// Every resolved cell already reflects the requested transaction state.
        /// </summary>
        NoOp,

        /// <summary>
        /// The transaction cannot be applied atomically.
        /// </summary>
        Invalid
    }
}
