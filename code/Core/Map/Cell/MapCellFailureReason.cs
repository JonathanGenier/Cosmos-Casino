namespace CosmosCasino.Core.Map.Cell
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
        /// The cell does not contain a structure where one
        /// is required by the operation.
        /// </summary>
        NoStructure,

        /// <summary>
        /// The cell does not contain furniture where one
        /// is required by the operation.
        /// </summary>
        NoFurniture,

        /// <summary>
        /// The operation targets the same type already present
        /// in the cell, resulting in no state change.
        /// </summary>
        SameType,

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

        /// <summary>
        /// The requested cell does not exist and could not
        /// be resolved for the operation.
        /// </summary>
        NoCell,
    }
}
