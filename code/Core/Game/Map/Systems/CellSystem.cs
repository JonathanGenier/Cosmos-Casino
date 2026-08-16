using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Game.Map.Systems
{
    /// <summary>
    /// Manages map cells and mediates build validation and operations by delegating
    /// to individual cells, acting as the authoritative cell-level build system.
    /// </summary>
    internal sealed class CellSystem : ITerrainTileSink
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
        /// <param name="coord">The terrain world-tile coordinate of the terrain tile.</param>
        /// <param name="terrainTile">The terrain tile used to initialize the cell.</param>
        void ITerrainTileSink.ReceiveTerrainTile(TerrainTileWorldCoord coord, TerrainTile terrainTile)
        {
            _grid.CreateCell(new MapCoord(coord.X, coord.Y), terrainTile);
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
        /// Determines whether the specified build kind exists at the terrain base elevation of the given coordinate.
        /// </summary>
        /// <param name="buildKind">The type of build element to check.</param>
        /// <param name="coord">The map coordinate to query.</param>
        /// <returns><c>true</c> if the build element exists; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal bool Has(BuildKind buildKind, MapCoord coord)
        {
            if (buildKind is not BuildKind.Floor and not BuildKind.Wall)
            {
                throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported.");
            }

            if (!TryGetCell(coord, out var cell))
            {
                return false;
            }

            Elevation elevation = cell.TerrainTile.BaseElevation;

            return buildKind switch
            {
                BuildKind.Floor => cell.HasFloorAt(elevation),
                BuildKind.Wall => cell.HasWallAt(elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Determines whether the specified build kind exists at the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to check.</param>
        /// <param name="coord">The map coordinate to query.</param>
        /// <param name="elevation">The elevation to query.</param>
        /// <returns><c>true</c> if the build element exists; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal bool Has(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            if (buildKind is not BuildKind.Floor and not BuildKind.Wall)
            {
                throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported.");
            }

            if (!TryGetCell(coord, out var cell))
            {
                return false;
            }

            return buildKind switch
            {
                BuildKind.Floor => cell.HasFloorAt(elevation),
                BuildKind.Wall => cell.HasWallAt(elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Validation API

        /// <summary>
        /// Validates whether the specified build kind can be placed at the terrain base elevation of the given coordinate.
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

            Elevation elevation = cell.TerrainTile.BaseElevation;

            return buildKind switch
            {
                BuildKind.Floor => CanPlaceFloor(cell, elevation),
                BuildKind.Wall => CanPlaceWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Validates whether the specified build kind can be placed at the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <param name="elevation">The elevation to validate.</param>
        /// <returns>The result of the placement validation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult CanPlace(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => CanPlaceFloor(cell, elevation),
                BuildKind.Wall => CanPlaceWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Validates whether the specified build kind can be removed from the terrain base elevation of the given coordinate.
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

            Elevation elevation = cell.TerrainTile.BaseElevation;

            return buildKind switch
            {
                BuildKind.Floor => CanRemoveFloor(cell, elevation),
                BuildKind.Wall => CanRemoveWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Validates whether the specified build kind can be removed from the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate to validate.</param>
        /// <param name="elevation">The elevation to validate.</param>
        /// <returns>The result of the removal validation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult CanRemove(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => CanRemoveFloor(cell, elevation),
                BuildKind.Wall => CanRemoveWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Operations API

        /// <summary>
        /// Attempts to place the specified build kind at the terrain base elevation of the given coordinate.
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

            Elevation elevation = cell.TerrainTile.BaseElevation;

            return buildKind switch
            {
                BuildKind.Floor => TryPlaceFloor(cell, elevation),
                BuildKind.Wall => TryPlaceWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Attempts to place the specified build kind at the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to place.</param>
        /// <param name="coord">The map coordinate at which to place.</param>
        /// <param name="elevation">The elevation at which to place.</param>
        /// <returns>The result of the placement operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult TryPlace(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => TryPlaceFloor(cell, elevation),
                BuildKind.Wall => TryPlaceWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Attempts to remove the specified build kind from the terrain base elevation of the given coordinate.
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

            Elevation elevation = cell.TerrainTile.BaseElevation;

            return buildKind switch
            {
                BuildKind.Floor => TryRemoveFloor(cell, elevation),
                BuildKind.Wall => TryRemoveWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        /// <summary>
        /// Attempts to remove the specified build kind from the given coordinate and elevation.
        /// </summary>
        /// <param name="buildKind">The type of build element to remove.</param>
        /// <param name="coord">The map coordinate from which to remove.</param>
        /// <param name="elevation">The elevation from which to remove.</param>
        /// <returns>The result of the removal operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unsupported build kinds.</exception>
        internal BuildOperationResult TryRemove(BuildKind buildKind, MapCoord coord, Elevation elevation)
        {
            if (!TryGetCell(coord, out var cell))
            {
                return BuildOperationResult.Invalid(coord, BuildOperationFailureReason.NoCell);
            }

            return buildKind switch
            {
                BuildKind.Floor => TryRemoveFloor(cell, elevation),
                BuildKind.Wall => TryRemoveWall(cell, elevation),
                _ => throw new InvalidOperationException($"{nameof(BuildKind)} is not yet supported."),
            };
        }

        #endregion

        #region Floor Validation Methods

        /// <summary>
        /// Validates whether a floor can be placed in the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <param name="elevation">The elevation at which to validate floor placement.</param>
        /// <returns>The result of the floor placement validation.</returns>
        private static BuildOperationResult CanPlaceFloor(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidatePlaceFloor(elevation);
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Validates whether a floor can be removed from the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <param name="elevation">The elevation from which to validate floor removal.</param>
        /// <returns>The result of the floor removal validation.</returns>
        private static BuildOperationResult CanRemoveFloor(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidateRemoveFloor(elevation);
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion

        #region Floor Operation Methods

        /// <summary>
        /// Attempts to place a floor in the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <param name="elevation">The elevation at which to place the floor.</param>
        /// <returns>The result of the floor placement operation.</returns>
        private static BuildOperationResult TryPlaceFloor(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidatePlaceFloor(elevation);

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.PlaceFloor(validationResult, new Floor(), elevation);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Attempts to remove a floor from the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <param name="elevation">The elevation from which to remove the floor.</param>
        /// <returns>The result of the floor removal operation.</returns>
        private static BuildOperationResult TryRemoveFloor(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidateRemoveFloor(elevation);

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.RemoveFloor(validationResult, elevation);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion

        #region Wall Validation Methods

        /// <summary>
        /// Validates whether a wall can be placed in the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <param name="elevation">The elevation at which to validate wall placement.</param>
        /// <returns>The result of the wall placement validation.</returns>
        private static BuildOperationResult CanPlaceWall(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidatePlaceWall(elevation);
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Validates whether a wall can be removed from the specified cell.
        /// </summary>
        /// <param name="cell">The cell to validate.</param>
        /// <param name="elevation">The elevation from which to validate wall removal.</param>
        /// <returns>The result of the wall removal validation.</returns>
        private static BuildOperationResult CanRemoveWall(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidateRemoveWall(elevation);
            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion

        #region Wall Operation Methods

        /// <summary>
        /// Attempts to place a wall in the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <param name="elevation">The elevation at which to place the wall.</param>
        /// <returns>The result of the wall placement operation.</returns>
        private static BuildOperationResult TryPlaceWall(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidatePlaceWall(elevation);

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.PlaceWall(validationResult, new Wall(), elevation);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        /// <summary>
        /// Attempts to remove a wall from the specified cell.
        /// </summary>
        /// <param name="cell">The target cell.</param>
        /// <param name="elevation">The elevation from which to remove the wall.</param>
        /// <returns>The result of the wall removal operation.</returns>
        private static BuildOperationResult TryRemoveWall(Cell cell, Elevation elevation)
        {
            var validationResult = cell.ValidateRemoveWall(elevation);

            if (validationResult.Outcome == BuildOperationOutcome.Valid)
            {
                cell.RemoveWall(validationResult, elevation);
            }

            return BuildOperationResult.FromMapCellValidationResult(validationResult, cell.Coord);
        }

        #endregion
    }
}
