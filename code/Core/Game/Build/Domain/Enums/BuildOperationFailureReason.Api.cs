namespace CosmosCasino.Core.Game.Build.Domain
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
        /// The target cell does not contain a wall where one
        /// is required by the operation.
        /// </summary>
        NoWall,

        /// <summary>
        /// The operation is blocked by another entity occupying
        /// the target cell (e.g., wall, furniture, or rule constraint).
        /// </summary>
        Blocked,

        /// <summary>
        /// The target cell does not exist and could not be resolved
        /// for the requested operation.
        /// </summary>
        NoCell
    }
}
