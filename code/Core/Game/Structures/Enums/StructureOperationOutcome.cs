namespace CosmosCasino.Core.Game.Structures
{
    /// <summary>
    /// Describes the result of an authoritative structure lifecycle operation.
    /// </summary>
    internal enum StructureOperationOutcome
    {
        /// <summary>
        /// The requested structure operation completed and mutated authoritative state.
        /// </summary>
        Valid,

        /// <summary>
        /// The requested structure operation had no state to mutate.
        /// </summary>
        NoOp,

        /// <summary>
        /// The requested structure operation was rejected without mutating authoritative state.
        /// </summary>
        Invalid
    }
}
