namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Describes why an authoritative furniture storage operation failed.
    /// </summary>
    internal enum FurnitureStorageOperationFailureReason
    {
        /// <summary>
        /// No failure occurred.
        /// </summary>
        None,

        /// <summary>
        /// A furniture aggregate with the requested identity already exists.
        /// </summary>
        FurnitureIdAlreadyExists,

        /// <summary>
        /// The requested footprint reservation could not be completed.
        /// </summary>
        FootprintReservationFailed,

        /// <summary>
        /// Existing authoritative furniture state is inconsistent with map-cell reservations.
        /// </summary>
        InconsistentState
    }
}
