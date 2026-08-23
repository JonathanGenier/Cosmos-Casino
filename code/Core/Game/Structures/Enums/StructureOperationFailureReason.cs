namespace CosmosCasino.Core.Game.Structures
{
    /// <summary>
    /// Describes why an authoritative structure operation failed.
    /// </summary>
    internal enum StructureOperationFailureReason
    {
        /// <summary>
        /// No failure occurred.
        /// </summary>
        None,

        /// <summary>
        /// A structure with the requested identity already exists.
        /// </summary>
        StructureIdAlreadyExists,

        /// <summary>
        /// The requested footprint reservation could not be completed.
        /// </summary>
        FootprintReservationFailed,

        /// <summary>
        /// Existing authoritative structure state is inconsistent with map-cell reservations.
        /// </summary>
        InconsistentState
    }
}
