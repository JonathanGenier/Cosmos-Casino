using CosmosCasino.Core.Game.Furniture;
using System.Diagnostics.CodeAnalysis;
using FurnitureAggregate = CosmosCasino.Core.Game.Furniture.Furniture;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Owns authoritative furniture aggregate storage and coordinates furniture lifecycle operations.
    /// </summary>
    public sealed partial class MapManager
    {
        #region Fields

        private readonly Dictionary<FurnitureId, FurnitureAggregate> _furniture = new();
        private long _nextFurnitureIdValue = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of authoritative furniture aggregates currently stored in the map.
        /// </summary>
        internal int FurnitureCount => _furniture.Count;

        #endregion

        #region Identity Allocation

        /// <summary>
        /// Attempts to preview the next deterministic furniture identities without mutating allocator state.
        /// </summary>
        /// <param name="count">The number of identities needed.</param>
        /// <param name="furnitureIds">The candidate identities in deterministic allocation order.</param>
        /// <returns><c>true</c> when every requested identity can be represented; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is negative.</exception>
        internal bool TryPreviewNextFurnitureIds(int count, out IReadOnlyList<FurnitureId> furnitureIds)
        {
            if (!TryCollectNextFurnitureIds(count, out FurnitureId[] ids, out _))
            {
                furnitureIds = Array.Empty<FurnitureId>();
                return false;
            }

            furnitureIds = Array.AsReadOnly(ids);
            return true;
        }

        /// <summary>
        /// Advances the furniture identity allocator after a valid operation plan has been selected for execution.
        /// </summary>
        /// <param name="expectedIds">The exact identities produced by the current planning pass.</param>
        /// <returns><c>true</c> when allocator state matched the plan and was advanced; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expectedIds"/> is null.</exception>
        internal bool TryConsumeNextFurnitureIds(IReadOnlyList<FurnitureId> expectedIds)
        {
            ArgumentNullException.ThrowIfNull(expectedIds);

            if (!TryCollectNextFurnitureIds(expectedIds.Count, out FurnitureId[] ids, out long nextValue))
            {
                return false;
            }

            for (int i = 0; i < expectedIds.Count; i++)
            {
                if (ids[i] != expectedIds[i])
                {
                    return false;
                }
            }

            _nextFurnitureIdValue = nextValue;
            return true;
        }

        #endregion

        #region Creation

        /// <summary>
        /// Attempts to create one authoritative furniture aggregate and reserve its complete footprint.
        /// </summary>
        /// <param name="id">The unique furniture identity to create.</param>
        /// <param name="definition">The immutable furniture definition.</param>
        /// <param name="anchor">The global logical map-cell anchor.</param>
        /// <param name="rotation">The furniture footprint rotation.</param>
        /// <returns>The result of the furniture creation operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
        internal FurnitureStorageOperationResult TryCreateFurniture(
            FurnitureId id,
            FurnitureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            ArgumentNullException.ThrowIfNull(definition);

            if (_furniture.ContainsKey(id))
            {
                return FurnitureStorageOperationResult.Invalid(FurnitureStorageOperationFailureReason.FurnitureIdAlreadyExists);
            }

            MapCellFootprintTransactionResult reservationResult = TryReserveFurnitureFootprint(
                anchor,
                definition.Footprint,
                rotation,
                id);

            if (reservationResult.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return FurnitureStorageOperationResult.Invalid(
                    FurnitureStorageOperationFailureReason.FootprintReservationFailed,
                    reservationResult);
            }

            _furniture.Add(id, new FurnitureAggregate(id, definition, anchor, rotation));
            return FurnitureStorageOperationResult.Valid();
        }

        #endregion

        #region Lookup

        /// <summary>
        /// Attempts to retrieve the authoritative furniture aggregate with the specified identity.
        /// </summary>
        /// <param name="id">The furniture identity to query.</param>
        /// <param name="furniture">The authoritative furniture aggregate, if found.</param>
        /// <returns><c>true</c> when the furniture exists; otherwise, <c>false</c>.</returns>
        internal bool TryGetFurniture(FurnitureId id, [NotNullWhen(true)] out FurnitureAggregate? furniture)
        {
            return _furniture.TryGetValue(id, out furniture);
        }

        /// <summary>
        /// Attempts to resolve the authoritative furniture referenced by the specified occupied map cell.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to query.</param>
        /// <param name="furniture">The authoritative furniture aggregate referenced by the cell, if any.</param>
        /// <returns><c>true</c> when the cell references existing furniture; otherwise, <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the cell contains a furniture identity that is missing from authoritative furniture storage.
        /// </exception>
        internal bool TryGetFurnitureAt(MapCellCoord coord, [NotNullWhen(true)] out FurnitureAggregate? furniture)
        {
            furniture = null;

            if (!TryGetCell(coord, out var cell) || !cell.FurnitureId.HasValue)
            {
                return false;
            }

            FurnitureId furnitureId = cell.FurnitureId.Value;

            if (!_furniture.TryGetValue(furnitureId, out furniture))
            {
                throw new InvalidOperationException(
                    $"Map cell {coord} references missing furniture {furnitureId}.");
            }

            return true;
        }

        #endregion

        #region Removal

        /// <summary>
        /// Attempts to remove the specified authoritative furniture aggregate and release its complete footprint.
        /// </summary>
        /// <param name="id">The furniture identity to remove.</param>
        /// <returns>The result of the furniture removal operation.</returns>
        internal FurnitureStorageOperationResult TryRemoveFurniture(FurnitureId id)
        {
            if (!_furniture.TryGetValue(id, out var furniture))
            {
                return FurnitureStorageOperationResult.NoOp();
            }

            MapCellFootprintTransactionResult releaseValidation = ValidateReleaseFurnitureFootprint(
                furniture.Anchor,
                furniture.Definition.Footprint,
                furniture.Rotation,
                furniture.Id);

            if (releaseValidation.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return FurnitureStorageOperationResult.Invalid(
                    FurnitureStorageOperationFailureReason.InconsistentState,
                    releaseValidation);
            }

            MapCellFootprintTransactionResult releaseResult = TryReleaseFurnitureFootprint(
                furniture.Anchor,
                furniture.Definition.Footprint,
                furniture.Rotation,
                furniture.Id);

            if (releaseResult.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return FurnitureStorageOperationResult.Invalid(
                    FurnitureStorageOperationFailureReason.InconsistentState,
                    releaseResult);
            }

            if (!_furniture.Remove(id))
            {
                throw new InvalidOperationException($"Furniture {id} was missing after its footprint was released.");
            }

            return FurnitureStorageOperationResult.Valid();
        }

        #endregion

        #region Helpers

        private bool TryCollectNextFurnitureIds(
            int count,
            out FurnitureId[] furnitureIds,
            out long nextValue)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Furniture identity count cannot be negative.");
            }

            furnitureIds = new FurnitureId[count];
            long candidateValue = _nextFurnitureIdValue;

            for (int i = 0; i < count; i++)
            {
                bool assigned = false;

                while (candidateValue <= int.MaxValue)
                {
                    var candidate = new FurnitureId((int)candidateValue);
                    candidateValue++;

                    if (_furniture.ContainsKey(candidate))
                    {
                        continue;
                    }

                    furnitureIds[i] = candidate;
                    assigned = true;
                    break;
                }

                if (!assigned)
                {
                    furnitureIds = Array.Empty<FurnitureId>();
                    nextValue = _nextFurnitureIdValue;
                    return false;
                }
            }

            nextValue = candidateValue;
            return true;
        }

        #endregion
    }
}
