namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Describes why an aggregate furniture operation failed.
    /// </summary>
    public enum FurnitureFailureReason
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
        /// A structure already occupies one of the requested footprint cells.
        /// </summary>
        StructurePresent,

        /// <summary>
        /// Furniture already occupies one of the requested footprint cells.
        /// </summary>
        FurniturePresent,

        /// <summary>
        /// At least one requested footprint cell conflicts with existing occupancy.
        /// </summary>
        OccupancyConflict,

        /// <summary>
        /// The requested footprint has mixed or inconsistent reservation state.
        /// </summary>
        InconsistentReservationState,

        /// <summary>
        /// Core could not assign the requested furniture a deterministic identity.
        /// </summary>
        FurnitureIdAllocationExhausted,

        /// <summary>
        /// A furniture identity expected to be available was already stored.
        /// </summary>
        FurnitureIdAlreadyExists,

        /// <summary>
        /// Existing authoritative furniture state does not match map-cell reservations.
        /// </summary>
        FurnitureStateInconsistent
    }
}
