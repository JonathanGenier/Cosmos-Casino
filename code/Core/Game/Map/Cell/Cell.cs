using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Buildables;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Represents one sparse logical map cell at a specific global X/Y/Z coordinate.
    /// </summary>
    internal sealed class Cell
    {
        #region Fields

        private readonly CellLayer _contents = new();
        private readonly HashSet<ItemId> _itemIds = new();

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new map cell at the specified global coordinate.
        /// </summary>
        /// <param name="coord">The global X/Y/Z coordinate identifying this cell.</param>
        internal Cell(MapCellCoord coord)
        {
            Coord = coord;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the global X/Y/Z coordinate that uniquely identifies this cell.
        /// </summary>
        internal MapCellCoord Coord { get; }

        /// <summary>
        /// Gets the structure occupying this cell, or <see langword="null"/> when none exists.
        /// </summary>
        internal StructureId? StructureId { get; private set; }

        /// <summary>
        /// Gets the furniture occupying this cell, or <see langword="null"/> when none exists.
        /// </summary>
        internal FurnitureId? FurnitureId { get; private set; }

        /// <summary>
        /// Gets the items occupying this cell.
        /// </summary>
        internal IReadOnlyCollection<ItemId> ItemIds => _itemIds;

        /// <summary>
        /// Gets a value indicating whether this cell contains no authoritative state.
        /// </summary>
        internal bool IsEmpty => _contents.IsEmpty && !HasOccupancy;

        /// <summary>
        /// Gets a value indicating whether this cell contains a structure reservation.
        /// </summary>
        internal bool HasStructure => StructureId.HasValue;

        /// <summary>
        /// Gets a value indicating whether this cell contains a furniture reservation.
        /// </summary>
        internal bool HasFurniture => FurnitureId.HasValue;

        /// <summary>
        /// Gets a value indicating whether this cell contains any item reservations.
        /// </summary>
        internal bool HasItems => _itemIds.Count > 0;

        /// <summary>
        /// Gets a value indicating whether this cell contains any local occupancy reservation.
        /// </summary>
        internal bool HasOccupancy => HasStructure || HasFurniture || HasItems;

        #endregion

        #region Cell Content Methods

        /// <summary>
        /// Determines whether this cell contains a floor.
        /// </summary>
        /// <returns><see langword="true"/> when the cell contains a floor; otherwise <see langword="false"/>.</returns>
        internal bool HasFloor()
        {
            return _contents.HasFloor;
        }

        /// <summary>
        /// Determines whether this cell contains a floor at the specified legacy elevation.
        /// </summary>
        /// <param name="elevation">The legacy elevation to query.</param>
        /// <returns><see langword="true"/> when this cell matches <paramref name="elevation"/> and contains a floor.</returns>
        internal bool HasFloorAt(Elevation elevation)
        {
            return Coord.Y == elevation.MapCellY && HasFloor();
        }

        /// <summary>
        /// Determines whether this cell contains a wall.
        /// </summary>
        /// <returns><see langword="true"/> when the cell contains a wall; otherwise <see langword="false"/>.</returns>
        internal bool HasWall()
        {
            return _contents.HasWall;
        }

        /// <summary>
        /// Determines whether this cell contains a wall at the specified legacy elevation.
        /// </summary>
        /// <param name="elevation">The legacy elevation to query.</param>
        /// <returns><see langword="true"/> when this cell matches <paramref name="elevation"/> and contains a wall.</returns>
        internal bool HasWallAt(Elevation elevation)
        {
            return Coord.Y == elevation.MapCellY && HasWall();
        }

        /// <summary>
        /// Determines whether this cell contains the specified item reservation.
        /// </summary>
        /// <param name="itemId">The item identity to query.</param>
        /// <returns><see langword="true"/> when the item is present; otherwise <see langword="false"/>.</returns>
        internal bool HasItem(ItemId itemId)
        {
            return _itemIds.Contains(itemId);
        }

        #endregion

        #region Occupancy Validation Methods

        /// <summary>
        /// Validates reserving this cell for the specified structure without mutating state.
        /// </summary>
        /// <param name="structureId">The structure identity to reserve.</param>
        /// <returns>The local occupancy validation result.</returns>
        internal CellOccupancyValidationResult ValidateReserveStructure(StructureId structureId)
        {
            if (StructureId.HasValue)
            {
                return StructureId.Value == structureId
                    ? CellOccupancyValidationResult.NoOp()
                    : CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.StructurePresent);
            }

            if (HasFurniture)
            {
                return CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.FurniturePresent);
            }

            if (HasItems)
            {
                return CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.ItemsPresent);
            }

            return CellOccupancyValidationResult.Valid();
        }

        /// <summary>
        /// Validates releasing the specified structure reservation without mutating state.
        /// </summary>
        /// <param name="structureId">The structure identity to release.</param>
        /// <returns>The local occupancy validation result.</returns>
        internal CellOccupancyValidationResult ValidateReleaseStructure(StructureId structureId)
        {
            if (!StructureId.HasValue)
            {
                return CellOccupancyValidationResult.NoOp();
            }

            if (StructureId.Value != structureId)
            {
                return CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.ReservationMismatch);
            }

            return CellOccupancyValidationResult.Valid();
        }

        /// <summary>
        /// Validates reserving this cell for the specified furniture without mutating state.
        /// </summary>
        /// <param name="furnitureId">The furniture identity to reserve.</param>
        /// <returns>The local occupancy validation result.</returns>
        internal CellOccupancyValidationResult ValidateReserveFurniture(FurnitureId furnitureId)
        {
            if (HasStructure)
            {
                return CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.StructurePresent);
            }

            if (FurnitureId.HasValue)
            {
                return FurnitureId.Value == furnitureId
                    ? CellOccupancyValidationResult.NoOp()
                    : CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.FurniturePresent);
            }

            return CellOccupancyValidationResult.Valid();
        }

        /// <summary>
        /// Validates releasing the specified furniture reservation without mutating state.
        /// </summary>
        /// <param name="furnitureId">The furniture identity to release.</param>
        /// <returns>The local occupancy validation result.</returns>
        internal CellOccupancyValidationResult ValidateReleaseFurniture(FurnitureId furnitureId)
        {
            if (!FurnitureId.HasValue)
            {
                return CellOccupancyValidationResult.NoOp();
            }

            if (FurnitureId.Value != furnitureId)
            {
                return CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.ReservationMismatch);
            }

            return CellOccupancyValidationResult.Valid();
        }

        /// <summary>
        /// Validates reserving this cell for the specified item without mutating state.
        /// </summary>
        /// <param name="itemId">The item identity to reserve.</param>
        /// <returns>The local occupancy validation result.</returns>
        internal CellOccupancyValidationResult ValidateReserveItem(ItemId itemId)
        {
            if (HasStructure)
            {
                return CellOccupancyValidationResult.Invalid(CellOccupancyFailureReason.StructurePresent);
            }

            if (_itemIds.Contains(itemId))
            {
                return CellOccupancyValidationResult.NoOp();
            }

            return CellOccupancyValidationResult.Valid();
        }

        /// <summary>
        /// Validates releasing the specified item reservation without mutating state.
        /// </summary>
        /// <param name="itemId">The item identity to release.</param>
        /// <returns>The local occupancy validation result.</returns>
        internal CellOccupancyValidationResult ValidateReleaseItem(ItemId itemId)
        {
            return _itemIds.Contains(itemId)
                ? CellOccupancyValidationResult.Valid()
                : CellOccupancyValidationResult.NoOp();
        }

        #endregion

        #region Occupancy Operation Methods

        /// <summary>
        /// Reserves this cell for the specified structure after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing the reservation.</param>
        /// <param name="structureId">The structure identity to reserve.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid.</exception>
        internal void ReserveStructure(CellOccupancyValidationResult validation, StructureId structureId)
        {
            if (validation.Outcome != CellOccupancyOutcome.Valid ||
                ValidateReserveStructure(structureId).Outcome != CellOccupancyOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot reserve structure: validation result is not valid.");
            }

            StructureId = structureId;
        }

        /// <summary>
        /// Releases the specified structure reservation after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing the release.</param>
        /// <param name="structureId">The structure identity to release.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid.</exception>
        internal void ReleaseStructure(CellOccupancyValidationResult validation, StructureId structureId)
        {
            if (validation.Outcome != CellOccupancyOutcome.Valid ||
                ValidateReleaseStructure(structureId).Outcome != CellOccupancyOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot release structure: validation result is not valid.");
            }

            StructureId = null;
        }

        /// <summary>
        /// Reserves this cell for the specified furniture after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing the reservation.</param>
        /// <param name="furnitureId">The furniture identity to reserve.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid.</exception>
        internal void ReserveFurniture(CellOccupancyValidationResult validation, FurnitureId furnitureId)
        {
            if (validation.Outcome != CellOccupancyOutcome.Valid ||
                ValidateReserveFurniture(furnitureId).Outcome != CellOccupancyOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot reserve furniture: validation result is not valid.");
            }

            FurnitureId = furnitureId;
        }

        /// <summary>
        /// Releases the specified furniture reservation after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing the release.</param>
        /// <param name="furnitureId">The furniture identity to release.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid.</exception>
        internal void ReleaseFurniture(CellOccupancyValidationResult validation, FurnitureId furnitureId)
        {
            if (validation.Outcome != CellOccupancyOutcome.Valid ||
                ValidateReleaseFurniture(furnitureId).Outcome != CellOccupancyOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot release furniture: validation result is not valid.");
            }

            FurnitureId = null;
        }

        /// <summary>
        /// Reserves this cell for the specified item after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing the reservation.</param>
        /// <param name="itemId">The item identity to reserve.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid.</exception>
        internal void ReserveItem(CellOccupancyValidationResult validation, ItemId itemId)
        {
            if (validation.Outcome != CellOccupancyOutcome.Valid ||
                ValidateReserveItem(itemId).Outcome != CellOccupancyOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot reserve item: validation result is not valid.");
            }

            _itemIds.Add(itemId);
        }

        /// <summary>
        /// Releases the specified item reservation after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing the release.</param>
        /// <param name="itemId">The item identity to release.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid.</exception>
        internal void ReleaseItem(CellOccupancyValidationResult validation, ItemId itemId)
        {
            if (validation.Outcome != CellOccupancyOutcome.Valid ||
                ValidateReleaseItem(itemId).Outcome != CellOccupancyOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot release item: validation result is not valid.");
            }

            _itemIds.Remove(itemId);
        }

        #endregion

        #region Floor Validation Methods

        /// <summary>
        /// Validates placing a floor in this cell without mutating state.
        /// </summary>
        /// <returns>A no-op result when a floor already exists; otherwise a valid result.</returns>
        internal CellValidationResult ValidatePlaceFloor()
        {
            if (_contents.HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Validates removing a floor from this cell without mutating state.
        /// </summary>
        /// <returns>
        /// A no-op result when no floor exists, an invalid result when a wall blocks removal, or a valid result.
        /// </returns>
        internal CellValidationResult ValidateRemoveFloor()
        {
            if (!_contents.HasFloor)
            {
                return CellValidationResult.NoOp();
            }

            if (_contents.HasWall)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.Blocked);
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Floor Operation Methods

        /// <summary>
        /// Places a floor after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing placement.</param>
        /// <param name="floor">The floor object to place.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell already has a floor.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="floor"/> is null.</exception>
        internal void PlaceFloor(CellValidationResult validation, Floor floor)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place floor: validation result is not valid.");
            }

            _contents.PlaceFloor(floor);
        }

        /// <summary>
        /// Removes the floor after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing removal.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell invariant is violated.</exception>
        internal void RemoveFloor(CellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove floor: validation result is not valid.");
            }

            _contents.RemoveFloor();
        }

        #endregion

        #region Wall Validation Methods

        /// <summary>
        /// Validates placing a wall in this cell without mutating state.
        /// </summary>
        /// <returns>
        /// An invalid result when no floor exists, a no-op result when a wall already exists, or a valid result.
        /// </returns>
        internal CellValidationResult ValidatePlaceWall()
        {
            if (!_contents.HasFloor)
            {
                return CellValidationResult.Invalid(BuildOperationFailureReason.NoFloor);
            }

            if (_contents.HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        /// <summary>
        /// Validates removing a wall from this cell without mutating state.
        /// </summary>
        /// <returns>A no-op result when no wall exists; otherwise a valid result.</returns>
        internal CellValidationResult ValidateRemoveWall()
        {
            if (!_contents.HasWall)
            {
                return CellValidationResult.NoOp();
            }

            return CellValidationResult.Valid();
        }

        #endregion

        #region Wall Operation Methods

        /// <summary>
        /// Places a wall after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing placement.</param>
        /// <param name="wall">The wall object to place.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell invariant is violated.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wall"/> is null.</exception>
        internal void PlaceWall(CellValidationResult validation, Wall wall)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot place wall: validation result is not valid.");
            }

            _contents.PlaceWall(wall);
        }

        /// <summary>
        /// Removes the wall after successful validation.
        /// </summary>
        /// <param name="validation">The successful validation result authorizing removal.</param>
        /// <exception cref="InvalidOperationException">Thrown when validation is not valid or the cell invariant is violated.</exception>
        internal void RemoveWall(CellValidationResult validation)
        {
            if (validation.Outcome != BuildOperationOutcome.Valid)
            {
                throw new InvalidOperationException("Cannot remove wall: validation result is not valid.");
            }

            _contents.RemoveWall();
        }

        #endregion
    }
}
