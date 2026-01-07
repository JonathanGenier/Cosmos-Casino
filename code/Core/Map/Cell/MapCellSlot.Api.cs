namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Represents a persistent structural slot within a map cell.
    /// <para>
    /// A slot identifies <em>where</em> something exists in a cell,
    /// not <em>what</em> exists there.
    /// </para>
    /// <para>
    /// Slots are used for logical identity and visual mapping.
    /// They must remain stable and must not encode gameplay intent
    /// or visual details.
    /// </para>
    /// </summary>
    public enum MapCellSlot
    {
        /// <summary>
        /// Floor layer of the cell.
        /// </summary>
        Floor,

        /// <summary>
        /// Structure layer of the cell (walls, pillars, etc).
        /// </summary>
        Structure,

        /// <summary>
        /// Furniture layer of the cell.
        /// </summary>
        Furniture
    }
}
