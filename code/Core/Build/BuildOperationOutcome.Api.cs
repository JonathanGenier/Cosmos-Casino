namespace CosmosCasino.Core.Build
{
    /// <summary>
    /// Describes the per-cell outcome of a build operation after it
    /// has been evaluated and applied to the game state.
    /// </summary>
    public enum BuildOperationOutcome
    {
        /// <summary>
        /// The build operation successfully placed new content
        /// on the target cell.
        /// </summary>
        Placed,

        /// <summary>
        /// The build operation successfully replaced existing
        /// content on the target cell.
        /// </summary>
        Replaced,

        /// <summary>
        /// The build operation successfully removed existing
        /// content from the target cell.
        /// </summary>
        Removed,

        /// <summary>
        /// The build operation was intentionally ignored because
        /// it would not result in a meaningful change.
        /// </summary>
        Skipped,

        /// <summary>
        /// The build operation could not be performed due to
        /// invalid state or rule violations.
        /// </summary>
        Failed
    }
}
