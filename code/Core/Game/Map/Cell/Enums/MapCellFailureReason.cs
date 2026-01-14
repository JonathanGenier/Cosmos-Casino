namespace CosmosCasino.Core.Game.Map.Cell
{
    /// <summary>
    /// Enumerates the possible reasons a map cell operation
    /// may fail or be skipped at the cell level.
    /// </summary>
    internal enum MapCellFailureReason
    {
        /// <summary>
        /// No failure occurred.
        /// The cell operation completed successfully.
        /// </summary>
        None,

        /// <summary>
        /// The cell does not contain a floor where one
        /// is required by the operation.
        /// </summary>
        NoFloor,

        /// <summary>
        /// The cell does not contain a wall where one
        /// is required by the operation.
        /// </summary>
        NoWall,

        /// <summary>
        /// The requested cell does not exist and could not
        /// be resolved for the operation.
        /// </summary>
        NoCell,

        /// <summary>
        /// The operation is blocked by another element
        /// currently occupying the cell.
        /// </summary>
        Blocked,

        /// <summary>
        /// An invalid or unexpected internal cell state
        /// was encountered, indicating a programmer error.
        /// </summary>
        InternalError,
    }
}
