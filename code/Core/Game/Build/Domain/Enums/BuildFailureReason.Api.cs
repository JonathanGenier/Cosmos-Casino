namespace CosmosCasino.Core.Game.Build.Domain
{
    /// <summary>
    /// Describes why an aggregate structure build intent failed.
    /// </summary>
    public enum BuildFailureReason
    {
        /// <summary>
        /// No failure occurred.
        /// </summary>
        None,

        /// <summary>
        /// Resolving the requested footprint would exceed the supported map-cell coordinate range.
        /// </summary>
        FootprintCoordinateOverflow,

        /// <summary>
        /// At least one requested footprint cell is outside generated terrain.
        /// </summary>
        OutsideGeneratedWorld,

        /// <summary>
        /// At least one requested footprint cell conflicts with existing occupancy.
        /// </summary>
        OccupancyConflict,

        /// <summary>
        /// The requested footprint has mixed or inconsistent reservation state.
        /// </summary>
        InconsistentReservationState,

        /// <summary>
        /// Two prospective placements in the same intent claim the same map cell.
        /// </summary>
        IntraBatchFootprintOverlap,

        /// <summary>
        /// Core could not assign every requested structure a deterministic identity.
        /// </summary>
        StructureIdAllocationExhausted,

        /// <summary>
        /// A structure identity expected to be available was already stored.
        /// </summary>
        StructureIdAlreadyExists,

        /// <summary>
        /// Existing authoritative structure state does not match map-cell reservations.
        /// </summary>
        StructureStateInconsistent
    }
}
