using CosmosCasino.Core.Game.Structures;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Owns authoritative structure aggregate storage and coordinates structure lifecycle operations.
    /// </summary>
    public sealed partial class MapManager
    {
        #region Fields

        private readonly Dictionary<StructureId, Structure> _structures = new();
        private long _nextStructureIdValue = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of authoritative structures currently stored in the map.
        /// </summary>
        internal int StructureCount => _structures.Count;

        #endregion

        #region Identity Allocation

        /// <summary>
        /// Attempts to preview the next deterministic structure identities without mutating allocator state.
        /// </summary>
        /// <param name="count">The number of identities needed.</param>
        /// <param name="structureIds">The candidate identities in deterministic allocation order.</param>
        /// <returns><c>true</c> when every requested identity can be represented; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is negative.</exception>
        internal bool TryPreviewNextStructureIds(int count, out IReadOnlyList<StructureId> structureIds)
        {
            if (!TryCollectNextStructureIds(count, out StructureId[] ids, out _))
            {
                structureIds = Array.Empty<StructureId>();
                return false;
            }

            structureIds = Array.AsReadOnly(ids);
            return true;
        }

        /// <summary>
        /// Advances the structure identity allocator after a valid build plan has been selected for execution.
        /// </summary>
        /// <param name="expectedIds">The exact identities produced by the current planning pass.</param>
        /// <returns><c>true</c> when allocator state matched the plan and was advanced; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expectedIds"/> is null.</exception>
        internal bool TryConsumeNextStructureIds(IReadOnlyList<StructureId> expectedIds)
        {
            ArgumentNullException.ThrowIfNull(expectedIds);

            if (!TryCollectNextStructureIds(expectedIds.Count, out StructureId[] ids, out long nextValue))
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

            _nextStructureIdValue = nextValue;
            return true;
        }

        #endregion

        #region Creation

        /// <summary>
        /// Attempts to create one authoritative structure and reserve its complete footprint.
        /// </summary>
        /// <param name="id">The unique structure identity to create.</param>
        /// <param name="definition">The immutable structure definition.</param>
        /// <param name="anchor">The global logical map-cell anchor.</param>
        /// <param name="rotation">The structure footprint rotation.</param>
        /// <returns>The result of the structure creation operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
        internal StructureOperationResult TryCreateStructure(
            StructureId id,
            StructureDefinition definition,
            MapCellCoord anchor,
            FootprintRotation rotation)
        {
            ArgumentNullException.ThrowIfNull(definition);

            if (_structures.ContainsKey(id))
            {
                return StructureOperationResult.Invalid(StructureOperationFailureReason.StructureIdAlreadyExists);
            }

            MapCellFootprintTransactionResult reservationResult = TryReserveStructureFootprint(
                anchor,
                definition.Footprint,
                rotation,
                id);

            if (reservationResult.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return StructureOperationResult.Invalid(
                    StructureOperationFailureReason.FootprintReservationFailed,
                    reservationResult);
            }

            _structures.Add(id, new Structure(id, definition, anchor, rotation));
            return StructureOperationResult.Valid();
        }

        #endregion

        #region Lookup

        /// <summary>
        /// Attempts to retrieve the authoritative structure with the specified identity.
        /// </summary>
        /// <param name="id">The structure identity to query.</param>
        /// <param name="structure">The authoritative structure, if found.</param>
        /// <returns><c>true</c> when the structure exists; otherwise, <c>false</c>.</returns>
        internal bool TryGetStructure(StructureId id, [NotNullWhen(true)] out Structure? structure)
        {
            return _structures.TryGetValue(id, out structure);
        }

        /// <summary>
        /// Attempts to resolve the authoritative structure referenced by the specified occupied map cell.
        /// </summary>
        /// <param name="coord">The global logical cell coordinate to query.</param>
        /// <param name="structure">The authoritative structure referenced by the cell, if any.</param>
        /// <returns><c>true</c> when the cell references an existing structure; otherwise, <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the cell contains a structure identity that is missing from authoritative structure storage.
        /// </exception>
        internal bool TryGetStructureAt(MapCellCoord coord, [NotNullWhen(true)] out Structure? structure)
        {
            structure = null;

            if (!TryGetCell(coord, out var cell) || !cell.StructureId.HasValue)
            {
                return false;
            }

            StructureId structureId = cell.StructureId.Value;

            if (!_structures.TryGetValue(structureId, out structure))
            {
                throw new InvalidOperationException(
                    $"Map cell {coord} references missing structure {structureId}.");
            }

            return true;
        }

        #endregion

        #region Removal

        /// <summary>
        /// Attempts to remove the specified authoritative structure and release its complete footprint.
        /// </summary>
        /// <param name="id">The structure identity to remove.</param>
        /// <returns>The result of the structure removal operation.</returns>
        internal StructureOperationResult TryRemoveStructure(StructureId id)
        {
            if (!_structures.TryGetValue(id, out var structure))
            {
                return StructureOperationResult.NoOp();
            }

            MapCellFootprintTransactionResult releaseValidation = ValidateReleaseStructureFootprint(
                structure.Anchor,
                structure.Definition.Footprint,
                structure.Rotation,
                structure.Id);

            if (releaseValidation.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return StructureOperationResult.Invalid(
                    StructureOperationFailureReason.InconsistentState,
                    releaseValidation);
            }

            MapCellFootprintTransactionResult releaseResult = TryReleaseStructureFootprint(
                structure.Anchor,
                structure.Definition.Footprint,
                structure.Rotation,
                structure.Id);

            if (releaseResult.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return StructureOperationResult.Invalid(
                    StructureOperationFailureReason.InconsistentState,
                    releaseResult);
            }

            if (!_structures.Remove(id))
            {
                throw new InvalidOperationException($"Structure {id} was missing after its footprint was released.");
            }

            return StructureOperationResult.Valid();
        }

        #endregion

        #region Helpers

        private bool TryCollectNextStructureIds(
            int count,
            out StructureId[] structureIds,
            out long nextValue)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Structure identity count cannot be negative.");
            }

            structureIds = new StructureId[count];
            long candidateValue = _nextStructureIdValue;

            for (int i = 0; i < count; i++)
            {
                bool assigned = false;

                while (candidateValue <= int.MaxValue)
                {
                    var candidate = new StructureId((int)candidateValue);
                    candidateValue++;

                    if (_structures.ContainsKey(candidate))
                    {
                        continue;
                    }

                    structureIds[i] = candidate;
                    assigned = true;
                    break;
                }

                if (!assigned)
                {
                    structureIds = Array.Empty<StructureId>();
                    nextValue = _nextStructureIdValue;
                    return false;
                }
            }

            nextValue = candidateValue;
            return true;
        }

        #endregion
    }
}
