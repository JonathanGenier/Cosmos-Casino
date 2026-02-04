using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Game.Map.Systems
{
    /// <summary>
    /// Manages map cells and mediates build validation and operations by delegating
    /// to individual cells, acting as the authoritative cell-level build system.
    /// </summary>
    internal class CellSystem : ITerrainTileSink
    {
        #region Fields

        private readonly Grid _grid;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new cell system with an empty backing grid.
        /// </summary>
        internal CellSystem()
        {
            _grid = new Grid();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of cells currently managed by the system.
        /// </summary>
        internal int CellCount => _grid.CellCount;

        #endregion

        #region ITerrainTileSink Implementation

        /// <summary>
        /// Receives a generated terrain tile and creates a corresponding cell.
        /// </summary>
        /// <param name="coord">The map coordinate of the terrain tile.</param>
        /// <param name="terrainTile">The terrain tile used to initialize the cell.</param>
        void ITerrainTileSink.ReceiveTerrainTile(MapCoord coord, TerrainTile terrainTile)
        {
            _grid.CreateCell(coord, terrainTile);
        }

        #endregion

        #region Cell API

        /// <summary>
        /// Enumerates all map coordinates currently associated with cells.
        /// </summary>
        /// <returns>An enumerable of all existing cell coordinates.</returns>
        internal IEnumerable<MapCoord> EnumerateAllCoords()
        {
            return _grid.AllCoords;
        }

        /// <summary>
        /// Attempts to retrieve the cell at the specified map coordinate.
        /// </summary>
        /// <param name="coord">The map coordinate to query.</param>
        /// <param name="cell">The cell at the coordinate if found.</param>
        /// <returns><c>true</c> if the cell exists; otherwise <c>false</c>.</returns>
        internal bool TryGetCell(MapCoord coord, [NotNullWhen(true)] out Cell? cell)
        {
            cell = _grid.GetCell(coord);
            return cell != null;
        }

        #endregion

        #region Has API

        /// <summary>
        /// Determines whether the specified build kind exists at the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to check.</param>
        /// <param name="coord">The map coordinate to query.</param>
        /// <returns><c>true</c> if the build element exists; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal bool Has(BuildKind buildKind, MapCoord coord)
        {
            return buildKind switch
            {
                BuildKind.Floor => TryGetCell(coord, out var cell) && cell.HasFloor,
                BuildKind.Wall => TryGetCell(coord, out var cell) && cell.HasWall,
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Validation API

        /// <summary>
        /// Validates whether the specified build kind can be placed at the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <returns>The result of the placement validation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult CanPlace(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => CanPlaceFloor(cell),
                BuildKind.Wall => CanPlaceWall(cell),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Validates whether the specified build kind can be removed from the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <returns>The result of the removal validation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult CanRemove(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => CanRemoveFloor(cell),
                BuildKind.Wall => CanRemoveWall(cell),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Operations API

        /// <summary>
        /// Attempts to place the specified build kind at the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate at which to place.</param>
        /// <returns>The result of the placement operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult TryPlace(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => TryPlaceFloor(cell),
                BuildKind.Wall => TryPlaceWall(cell),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Attempts to remove the specified build kind from the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate from which to remove.</param>
        /// <returns>The result of the removal operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult TryRemove(BuildKind buildKind, MapCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => TryRemoveFloor(cell),
                BuildKind.Wall => TryRemoveWall(cell),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Floor Validation Methods

        /// <summary>
        /// Validates whether a floor can be placed in the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <returns>The result of the floor placement validation.</returns>
        private BuildOperationResult CanPlaceFloor(Cell cell)
        {
            var validationResult = cell.ValidatePlaceFloor();
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Validates whether a floor can be removed from the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <returns>The result of the floor removal validation.</returns>
        private BuildOperationResult CanRemoveFloor(Cell cell)
        {
            var validationResult = cell.ValidateRemoveFloor();
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion

        #region Floor Operation Methods

        /// <summary>
        /// Attempts to place a floor in the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <returns>The result of the floor placement operation.</returns>
        private BuildOperationResult TryPlaceFloor(Cell cell)
        {
            var validationResult = cell.ValidatePlaceFloor();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.PlaceFloor(validationResult);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Attempts to remove a floor from the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <returns>The result of the floor removal operation.</returns>
        private BuildOperationResult TryRemoveFloor(Cell cell)
        {
            var validationResult = cell.ValidateRemoveFloor();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.RemoveFloor(validationResult);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion

        #region Wall Validation Methods

        /// <summary>
        /// Validates whether a wall can be placed in the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <returns>The result of the wall placement validation.</returns>
        private BuildOperationResult CanPlaceWall(Cell cell)
        {
            var validationResult = cell.ValidatePlaceWall();
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Validates whether a wall can be removed from the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <returns>The result of the wall removal validation.</returns>
        private BuildOperationResult CanRemoveWall(Cell cell)
        {
            var validationResult = cell.ValidateRemoveWall();
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion

        #region Wall Operation Methods

        /// <summary>
        /// Attempts to place a wall in the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <returns>The result of the wall placement operation.</returns>
        private BuildOperationResult TryPlaceWall(Cell cell)
        {
            var validationResult = cell.ValidatePlaceWall();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.PlaceWall(validationResult);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Attempts to remove a wall from the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <returns>The result of the wall removal operation.</returns>
        private BuildOperationResult TryRemoveWall(Cell cell)
        {
            var validationResult = cell.ValidateRemoveWall();

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.RemoveWall(validationResult);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion
    }
}