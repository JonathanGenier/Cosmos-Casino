using CosmosCasino.Core.Game.Map.Cell;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Immutable command describing a single build action to be applied
    /// to one or more target cells.
    /// </summary>
    public sealed partial class BuildIntent
    {
        #region Validation

        /// <summary>
        /// Validates the collection of target cells used to construct a build intent.
        /// Ensures the intent operates on at least one valid cell.
        /// </summary>
        /// <param name="cells">
        /// Collection of target cell coordinates.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the cell collection is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the cell collection is empty.
        /// </exception>
        private static void ValidateCells(IReadOnlyList<MapCellCoord> cells)
        {
            ArgumentNullException.ThrowIfNull(cells);

            if (cells.Count == 0)
            {
                throw new ArgumentException("BuildIntent requires at least one cell.", nameof(cells));
            }
        }

        #endregion
    }
}
