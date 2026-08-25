namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Describes whether a furniture change creates or removes an aggregate.
    /// </summary>
    public enum FurnitureChangeResultKind
    {
        /// <summary>
        /// The operation created one authoritative furniture aggregate.
        /// </summary>
        Created,

        /// <summary>
        /// The operation removed one authoritative furniture aggregate.
        /// </summary>
        Removed
    }
}
