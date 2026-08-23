using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;

namespace CosmosCasino.Core.Game.Map.Systems
{
    /// <summary>
    /// Mediates build validation and operations against sparse map cells without owning world storage.
    /// </summary>
    internal sealed class CellSystem
    {
        #region Has API

        /// <summary>
        /// Determines whether the specified build kind exists in the given sparse cell state.
        /// </summary>
        /// <param name="buildKind">The type of build element to check.</param>
        /// <param name="cell">The existing sparse cell, or <c>null</c> for implicit empty state.</param>
        /// <returns><c>true</c> if the build element exists; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal bool Has(BuildKind buildKind, Cell? cell)
        {
            if (buildKind is not BuildKind.Floor and not BuildKind.Wall)
            {
                throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported.");
            }

            if (cell is null)
            {
                return false;
            }

            return buildKind switch
            {
                BuildKind.Floor => cell.HasFloor(),
                BuildKind.Wall => cell.HasWall(),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Validation API

        /// <summary>
        /// Validates whether the specified build kind can be placed in the current sparse cell state.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The legacy map coordinate associated with the build operation.</param>
        /// <param name="cell">The existing sparse cell, or <c>null</c> for implicit empty state.</param>
        /// <returns>The result of the placement validation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult CanPlace(BuildKind buildKind, MapCoord coord, Cell? cell)
        {
            CellValidationResult validationResult = buildKind switch
            {
                BuildKind.Floor => cell?.ValidatePlaceFloor() ?? CellValidationResult.Valid(),
                BuildKind.Wall => cell?.ValidatePlaceWall() ?? CellValidationResult.Invalid(BuildOperationFailureReason.NoFloor),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };

            return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
        }

        /// <summary>
        /// Validates whether the specified build kind can be removed from the current sparse cell state.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The legacy map coordinate associated with the build operation.</param>
        /// <param name="cell">The existing sparse cell, or <c>null</c> for implicit empty state.</param>
        /// <returns>The result of the removal validation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult CanRemove(BuildKind buildKind, MapCoord coord, Cell? cell)
        {
            CellValidationResult validationResult = buildKind switch
            {
                BuildKind.Floor => cell?.ValidateRemoveFloor() ?? CellValidationResult.NoOp(),
                BuildKind.Wall => cell?.ValidateRemoveWall() ?? CellValidationResult.NoOp(),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };

            return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
        }

        #endregion

        #region Operations API

        /// <summary>
        /// Attempts to place the specified build kind, creating sparse cell storage only if state commits.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The legacy map coordinate associated with the build operation.</param>
        /// <param name="cell">The existing sparse cell, or <c>null</c> for implicit empty state.</param>
        /// <param name="getOrCreateCell">Creates sparse storage only after validation succeeds.</param>
        /// <returns>The result of the placement operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult TryPlace(
            BuildKind buildKind,
            MapCoord coord,
            Cell? cell,
            Func<Cell> getOrCreateCell)
        {
            ArgumentNullException.ThrowIfNull(getOrCreateCell);

            return buildKind switch
            {
                BuildKind.Floor => TryPlaceFloor(coord, cell, getOrCreateCell),
                BuildKind.Wall => TryPlaceWall(coord, cell),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Attempts to remove the specified build kind from the current sparse cell state.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The legacy map coordinate associated with the build operation.</param>
        /// <param name="cell">The existing sparse cell, or <c>null</c> for implicit empty state.</param>
        /// <returns>The result of the removal operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult TryRemove(BuildKind buildKind, MapCoord coord, Cell? cell)
        {
            return buildKind switch
            {
                BuildKind.Floor => TryRemoveFloor(coord, cell),
                BuildKind.Wall => TryRemoveWall(coord, cell),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Floor Methods

        private static BuildOperationResult TryPlaceFloor(MapCoord coord, Cell? cell, Func<Cell> getOrCreateCell)
        {
            CellValidationResult validationResult = cell?.ValidatePlaceFloor() ?? CellValidationResult.Valid();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                Cell targetCell = cell ?? getOrCreateCell();
                targetCell.PlaceFloor(validationResult, new Floor());
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
        }

        private static BuildOperationResult TryRemoveFloor(MapCoord coord, Cell? cell)
        {
            CellValidationResult validationResult = cell?.ValidateRemoveFloor() ?? CellValidationResult.NoOp();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell!.RemoveFloor(validationResult);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
        }

        #endregion

        #region Wall Methods

        private static BuildOperationResult TryPlaceWall(MapCoord coord, Cell? cell)
        {
            CellValidationResult validationResult = cell?.ValidatePlaceWall() ?? CellValidationResult.Invalid(BuildOperationFailureReason.NoFloor);

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell!.PlaceWall(validationResult, new Wall());
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
        }

        private static BuildOperationResult TryRemoveWall(MapCoord coord, Cell? cell)
        {
            CellValidationResult validationResult = cell?.ValidateRemoveWall() ?? CellValidationResult.NoOp();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell!.RemoveWall(validationResult);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, coord);
        }

        #endregion
    }
}
