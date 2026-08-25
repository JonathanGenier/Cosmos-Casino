namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Describes the result of an authoritative furniture storage lifecycle operation.
    /// </summary>
    internal enum FurnitureStorageOperationOutcome
    {
        /// <summary>
        /// The requested furniture storage operation completed and mutated authoritative state.
        /// </summary>
        Valid,

        /// <summary>
        /// The requested furniture storage operation had no state to mutate.
        /// </summary>
        NoOp,

        /// <summary>
        /// The requested furniture storage operation was rejected without mutating authoritative state.
        /// </summary>
        Invalid
    }
}
