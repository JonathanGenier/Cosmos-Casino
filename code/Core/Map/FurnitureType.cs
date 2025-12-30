namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Identifies the type of furniture placed within a map cell.
    /// Furniture represents interactive objects that provide gameplay
    /// functionality rather than structural support.
    /// </summary>
    internal enum FurnitureType
    {
        /// <summary>
        /// A gambling machine that players or NPCs can interact with
        /// to generate income through chance-based gameplay.
        /// </summary>
        SlotMachine,

        /// <summary>
        /// A table used for card-based gambling games involving one or
        /// more participants and higher interaction complexity.
        /// </summary>
        PokerTable
    }
}
