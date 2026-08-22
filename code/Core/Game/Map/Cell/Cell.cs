using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents one horizontal map column containing buildable layers at discrete elevations.
    /// </summary>
    internal sealed class Cell
    {
        #region Fields

        /// <summary>
        /// Stores buildable layers by their discrete elevation within this cell.
        /// </summary>
        private readonly Dictionary<Elevation, CellLayer> _cellLayers = [];

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new map cell at the specified coordinate.
        /// </summary>
        /// <param name="coord">The map coordinate identifying this cell.</param>
        internal Cell(MapCoord coord)
        {
            Coord = coord;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the map coordinate that uniquely identifies this cell.
        /// </summary>
        internal MapCoord Coord { get; }

        #endregion

        #region Cell Layer Methods

        /// <summary>
        /// Determines whether this cell contains a floor at the specified elevation.
        /// </summary>
        /// <param name="elevation">The elevation to query.</param>
        /// <returns><see langword="true"/> when the layer contains a floor; otherwise <see langword="false"/>.</returns>
        internal bool HasFloorAt(Elevation elevation)
        {
            CellLayer? layer = TryGetLayer(elevation);

            return layer is not null && layer.HasFloor;
        }

        /// <summary>
        /// Determines whether this cell contains a wall at the specified elevation.
        /// </summary>
        /// <param name="elevation">The elevation to query.</param>
        /// <returns><see langword="true"/> when the layer contains a wall; otherwise <see langword="false"/>.</returns>
        internal bool HasWallAt(Elevation elevation)
        {
            CellLayer? layer = TryGetLayer(elevation);

            return layer is not null && layer.HasWall;
        }

        #endregion

        #region Floor Validation Methods

        /// <summary>
        /// Validates placing a floor in this cell at the specified elevation without mutating layer state.
        /// </summary>
        /// <param name="elevation">The elevation at which to validate floor placement.</param>
        /// <returns>A no-op result when a floor already exists at the elevation; otherwise a valid result.</returns>
        internal CellValidationResult ValidatePlaceFloor(Elevation elevation)
        {
            CellLayer? layer = TryGetLayer(elevation);

            if (layer is not null && layer.HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Validates removing a floor from this cell at the specified elevation without mutating layer state.
        /// </summary>
        /// <param name="elevation">The elevation from which to validate floor removal.</param>
        /// <returns>
        /// A no-op result when no floor exists, an invalid result when a wall blocks removal, or a valid result.
        /// </returns>
        internal CellValidationResult ValidateRemoveFloor(Elevation elevation)
        {
            CellLayer? layer = TryGetLayer(elevation);

            if (layer is null || !layer.HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            if (layer.HasWall)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.Blocked);
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Floor Operation Methods

        /// <summary>
        /// Places a floor at the specified elevation after successful validation, creating the layer if necessary.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing placement.</param>
        /// <param name="floor">The floor object to place.</param>
        /// <param name="elevation">The elevation at which to place the floor.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the layer already has a floor.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="floor"/> is null.</exception>
        internal void PlaceFloor(CellValidationResult validation, Floor floor, Elevation elevation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place floor: validation result is not valid.");
            }

            ArgumentNullException.ThrowIfNull(floor);

            CellLayer layer = GetOrCreateLayer(elevation);
            layer.PlaceFloor(floor);
        }

        /// <summary>
        /// Removes the floor at the specified elevation and discards the layer when it becomes empty.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing removal.</param>
        /// <param name="elevation">The elevation from which to remove the floor.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the layer invariant is violated.</exception>
        internal void RemoveFloor(CellValidationResult validation, Elevation elevation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove floor: validation result is not valid.");
            }

            CellLayer layer = TryGetLayer(elevation)!;
            layer.RemoveFloor();

            if (layer.IsEmpty)
            {
                _cellLayers.Remove(elevation);
            }
        }

        #endregion

        #region Wall Validation Methods

        /// <summary>
        /// Validates placing a wall in this cell at the specified elevation without mutating layer state.
        /// </summary>
        /// <param name="elevation">The elevation at which to validate wall placement.</param>
        /// <returns>
        /// An invalid result when no floor exists at the elevation, a no-op result when a wall already exists,
        /// or a valid result.
        /// </returns>
        internal CellValidationResult ValidatePlaceWall(Elevation elevation)
        {
            CellLayer? layer = TryGetLayer(elevation);

            if (layer is null || !layer.HasFloor)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.NoFloor);
            }

            if (layer.HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Validates removing a wall from this cell at the specified elevation without mutating layer state.
        /// </summary>
        /// <param name="elevation">The elevation from which to validate wall removal.</param>
        /// <returns>A no-op result when no wall exists at the elevation; otherwise a valid result.</returns>
        internal CellValidationResult ValidateRemoveWall(Elevation elevation)
        {
            CellLayer? layer = TryGetLayer(elevation);

            if (layer is null || !layer.HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Wall Operation Methods

        /// <summary>
        /// Places a wall in the existing cell layer at the specified elevation after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing placement.</param>
        /// <param name="wall">The wall object to place.</param>
        /// <param name="elevation">The elevation at which to place the wall.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the layer invariant is violated.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wall"/> is null.</exception>
        internal void PlaceWall(CellValidationResult validation, Wall wall, Elevation elevation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place wall: validation result is not valid.");
            }

            ArgumentNullException.ThrowIfNull(wall);

            CellLayer layer = TryGetLayer(elevation)!;
            layer.PlaceWall(wall);
        }

        /// <summary>
        /// Removes the wall from the existing cell layer at the specified elevation after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing removal.</param>
        /// <param name="elevation">The elevation from which to remove the wall.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the layer invariant is violated.</exception>
        internal void RemoveWall(CellValidationResult validation, Elevation elevation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove wall: validation result is not valid.");
            }

            CellLayer layer = TryGetLayer(elevation)!;
            layer.RemoveWall();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the existing cell layer at the specified elevation without creating one.
        /// </summary>
        /// <param name="elevation">The elevation to query.</param>
        /// <returns>The existing layer, or <see langword="null"/> when none exists.</returns>
        private CellLayer? TryGetLayer(Elevation elevation)
        {
            _cellLayers.TryGetValue(elevation, out CellLayer? layer);
            return layer;
        }

        /// <summary>
        /// Gets the layer at the specified elevation, creating it when necessary.
        /// </summary>
        /// <param name="elevation">The elevation of the layer.</param>
        /// <returns>The existing or newly created layer.</returns>
        private CellLayer GetOrCreateLayer(Elevation elevation)
        {
            if (_cellLayers.TryGetValue(elevation, out CellLayer? layer))
            {
                return layer;
            }

            layer = new CellLayer();
            _cellLayers.Add(elevation, layer);

            return layer;
        }

        #endregion
    }
}
