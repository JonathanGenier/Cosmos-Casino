namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Describes why a map-cell footprint transaction failed.
    /// </summary>
    internal enum MapCellFootprintTransactionFailureReason
    {
        /// <summary>
        /// No failure occurred.
        /// </summary>
        None,

        /// <summary>
        /// Resolving one or more footprint coordinates would exceed the representable map-cell range.
        /// </summary>
        CoordinateOverflow,

        /// <summary>
        /// One or more footprint coordinates are outside generated horizontal terrain.
        /// </summary>
        OutsideGeneratedWorld,

        /// <summary>
        /// One or more resolved cells conflict with existing occupancy state.
        /// </summary>
        OccupancyConflict,

        /// <summary>
        /// The footprint contains a mix of applicable and already-applied or missing reservations.
        /// </summary>
        InconsistentReservationState
    }
}
