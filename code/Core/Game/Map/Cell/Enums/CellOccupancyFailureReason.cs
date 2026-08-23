namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Describes why a local map cell occupancy reservation is invalid.
    /// </summary>
    internal enum CellOccupancyFailureReason
    {
        /// <summary>
        /// No failure occurred.
        /// </summary>
        None,

        /// <summary>
        /// A structure already occupies the cell.
        /// </summary>
        StructurePresent,

        /// <summary>
        /// Furniture already occupies the cell.
        /// </summary>
        FurniturePresent,

        /// <summary>
        /// One or more items already occupy the cell.
        /// </summary>
        ItemsPresent,

        /// <summary>
        /// The requested release identity does not own the current reservation.
        /// </summary>
        ReservationMismatch
    }
}
