namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Describes the aggregate result of a furniture operation.
    /// </summary>
    public enum FurnitureOperationOutcome
    {
        /// <summary>
        /// The requested furniture operation completed and mutated authoritative state.
        /// </summary>
        Valid,

        /// <summary>
        /// The requested furniture operation had no state to mutate.
        /// </summary>
        NoOp,

        /// <summary>
        /// The requested furniture operation was rejected without mutating authoritative state.
        /// </summary>
        Invalid
    }
}
