namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Enumerates the possible reasons a build operation may fail
    /// or be skipped at the build-system level.
    /// </summary>
    public enum BuildOperationFailureReason
    {
        /// <summary>
        /// No failure occurred.
        /// The operation completed successfully.
        /// </summary>
        None,

        /// <summary>
        /// The target cell does not contain a floor where one
        /// is required by the operation.
        /// </summary>
        NoFloor,

        /// <summary>
        /// The target cell does not contain a structure where one
        /// is required by the operation.
        /// </summary>
        NoStructure,

        /// <summary>
        /// The target cell does not contain furniture where one
        /// is required by the operation.
        /// </summary>
        NoFurniture,

        /// <summary>
        /// The requested build operation targets the same type
        /// that is already present, resulting in no state change.
        /// </summary>
        SameType,

        /// <summary>
        /// The operation is blocked by another entity occupying
        /// the target cell (e.g., structure, furniture, or rule constraint).
        /// </summary>
        Blocked,

        /// <summary>
        /// An unexpected or invalid internal state was encountered.
        /// Indicates a programmer error rather than gameplay behavior.
        /// </summary>
        InternalError,

        /// <summary>
        /// The target cell does not exist and could not be resolved
        /// for the requested operation.
        /// </summary>
        NoCell,

        /// <summary>
        /// The operation could not be completed due to insufficient
        /// available funds.
        /// </summary>
        NoFunds
    }
}
