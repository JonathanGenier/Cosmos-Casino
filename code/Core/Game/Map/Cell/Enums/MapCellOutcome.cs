namespace CosmosCasino.Core.Game.Map.Cell
{
    /// <summary>
    /// Represents the low-level outcome of an operation applied
    /// directly to a map cell.
    /// </summary>
    public enum MapCellOutcome
    {
        /// <summary>
        /// New content was successfully placed on the cell.
        /// </summary>
        Placed,

        /// <summary>
        /// Existing content was successfully removed from the cell.
        /// </summary>
        Removed,

        /// <summary>
        /// The operation was evaluated but intentionally resulted
        /// in no state change.
        /// </summary>
        Skipped,

        /// <summary>
        /// The operation could not be applied due to an invalid
        /// cell state or rule violation.
        /// </summary>
        Failed
    }
}
