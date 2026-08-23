using CosmosCasino.Core.Game.Map.Terrain;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Coordinates atomic occupancy reservations over reusable multi-cell footprints.
    /// </summary>
    public sealed partial class MapManager
    {
        #region Delegates

        private delegate CellOccupancyValidationResult CellOccupancyValidator(Cell cell);

        private delegate void CellOccupancyCommitter(Cell cell, CellOccupancyValidationResult validation);

        #endregion

        #region Structure Footprint API

        /// <summary>
        /// Validates reserving every cell in the specified footprint for a structure.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to reserve.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="structureId">The structure identity to reserve.</param>
        /// <returns>The aggregate transaction validation result.</returns>
        internal MapCellFootprintTransactionResult ValidateReserveStructureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            StructureId structureId)
        {
            return ValidateFootprintTransaction(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReserveStructure(structureId),
                CellOccupancyValidationResult.Valid());
        }

        /// <summary>
        /// Attempts to reserve every cell in the specified footprint for a structure.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to reserve.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="structureId">The structure identity to reserve.</param>
        /// <returns>The aggregate transaction result.</returns>
        internal MapCellFootprintTransactionResult TryReserveStructureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            StructureId structureId)
        {
            MapCellFootprintTransactionResult result = ValidateReserveStructureFootprint(
                anchor,
                footprint,
                rotation,
                structureId);

            if (result.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return result;
            }

            CommitReserveFootprint(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReserveStructure(structureId),
                (cell, validation) => cell.ReserveStructure(validation, structureId));

            return result;
        }

        /// <summary>
        /// Validates releasing every cell in the specified footprint from a structure.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to release.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="structureId">The structure identity to release.</param>
        /// <returns>The aggregate transaction validation result.</returns>
        internal MapCellFootprintTransactionResult ValidateReleaseStructureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            StructureId structureId)
        {
            return ValidateFootprintTransaction(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReleaseStructure(structureId),
                CellOccupancyValidationResult.NoOp());
        }

        /// <summary>
        /// Attempts to release every cell in the specified footprint from a structure.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to release.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="structureId">The structure identity to release.</param>
        /// <returns>The aggregate transaction result.</returns>
        internal MapCellFootprintTransactionResult TryReleaseStructureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            StructureId structureId)
        {
            MapCellFootprintTransactionResult result = ValidateReleaseStructureFootprint(
                anchor,
                footprint,
                rotation,
                structureId);

            if (result.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return result;
            }

            CommitReleaseFootprint(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReleaseStructure(structureId),
                (cell, validation) => cell.ReleaseStructure(validation, structureId));

            return result;
        }

        #endregion

        #region Furniture Footprint API

        /// <summary>
        /// Validates reserving every cell in the specified footprint for furniture.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to reserve.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="furnitureId">The furniture identity to reserve.</param>
        /// <returns>The aggregate transaction validation result.</returns>
        internal MapCellFootprintTransactionResult ValidateReserveFurnitureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            FurnitureId furnitureId)
        {
            return ValidateFootprintTransaction(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReserveFurniture(furnitureId),
                CellOccupancyValidationResult.Valid());
        }

        /// <summary>
        /// Attempts to reserve every cell in the specified footprint for furniture.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to reserve.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="furnitureId">The furniture identity to reserve.</param>
        /// <returns>The aggregate transaction result.</returns>
        internal MapCellFootprintTransactionResult TryReserveFurnitureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            FurnitureId furnitureId)
        {
            MapCellFootprintTransactionResult result = ValidateReserveFurnitureFootprint(
                anchor,
                footprint,
                rotation,
                furnitureId);

            if (result.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return result;
            }

            CommitReserveFootprint(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReserveFurniture(furnitureId),
                (cell, validation) => cell.ReserveFurniture(validation, furnitureId));

            return result;
        }

        /// <summary>
        /// Validates releasing every cell in the specified footprint from furniture.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to release.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="furnitureId">The furniture identity to release.</param>
        /// <returns>The aggregate transaction validation result.</returns>
        internal MapCellFootprintTransactionResult ValidateReleaseFurnitureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            FurnitureId furnitureId)
        {
            return ValidateFootprintTransaction(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReleaseFurniture(furnitureId),
                CellOccupancyValidationResult.NoOp());
        }

        /// <summary>
        /// Attempts to release every cell in the specified footprint from furniture.
        /// </summary>
        /// <param name="anchor">The global coordinate used as the footprint origin.</param>
        /// <param name="footprint">The reusable footprint to release.</param>
        /// <param name="rotation">The rotation to apply before resolving coordinates.</param>
        /// <param name="furnitureId">The furniture identity to release.</param>
        /// <returns>The aggregate transaction result.</returns>
        internal MapCellFootprintTransactionResult TryReleaseFurnitureFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            FurnitureId furnitureId)
        {
            MapCellFootprintTransactionResult result = ValidateReleaseFurnitureFootprint(
                anchor,
                footprint,
                rotation,
                furnitureId);

            if (result.Outcome != MapCellFootprintTransactionOutcome.Valid)
            {
                return result;
            }

            CommitReleaseFootprint(
                anchor,
                footprint,
                rotation,
                cell => cell.ValidateReleaseFurniture(furnitureId),
                (cell, validation) => cell.ReleaseFurniture(validation, furnitureId));

            return result;
        }

        #endregion

        #region Transaction Helpers

        private static MapCellFootprintTransactionResult ToInvalidOccupancyResult(
            MapCellCoord coord,
            CellOccupancyFailureReason occupancyFailureReason)
        {
            return MapCellFootprintTransactionResult.Invalid(
                MapCellFootprintTransactionFailureReason.OccupancyConflict,
                coord,
                occupancyFailureReason);
        }

        private MapCellFootprintTransactionResult ValidateFootprintTransaction(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            CellOccupancyValidator validateExistingCell,
            CellOccupancyValidationResult missingCellValidation)
        {
            ArgumentNullException.ThrowIfNull(footprint);
            ArgumentNullException.ThrowIfNull(validateExistingCell);

            IReadOnlyList<MapCellCoord> coords;

            try
            {
                coords = footprint.Resolve(anchor, rotation);
            }
            catch (ArgumentOutOfRangeException)
            {
                return MapCellFootprintTransactionResult.Invalid(
                    MapCellFootprintTransactionFailureReason.CoordinateOverflow);
            }

            bool hasValid = false;
            bool hasNoOp = false;
            MapCellCoord? firstNoOpCoord = null;

            foreach (MapCellCoord coord in coords)
            {
                if (!HasGeneratedTerrainAt(coord))
                {
                    return MapCellFootprintTransactionResult.Invalid(
                        MapCellFootprintTransactionFailureReason.OutsideGeneratedWorld,
                        coord);
                }

                CellOccupancyValidationResult cellValidation = TryGetCell(coord, out var cell)
                    ? validateExistingCell(cell)
                    : missingCellValidation;

                if (cellValidation.Outcome == CellOccupancyOutcome.Invalid)
                {
                    return ToInvalidOccupancyResult(coord, cellValidation.FailureReason);
                }

                if (cellValidation.Outcome == CellOccupancyOutcome.Valid)
                {
                    hasValid = true;
                    continue;
                }

                hasNoOp = true;
                firstNoOpCoord ??= coord;
            }

            if (hasValid && hasNoOp)
            {
                return MapCellFootprintTransactionResult.Invalid(
                    MapCellFootprintTransactionFailureReason.InconsistentReservationState,
                    firstNoOpCoord);
            }

            return hasValid
                ? MapCellFootprintTransactionResult.Valid()
                : MapCellFootprintTransactionResult.NoOp();
        }

        private void CommitReserveFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            CellOccupancyValidator validateCell,
            CellOccupancyCommitter commitCell)
        {
            CommitFootprint(
                anchor,
                footprint,
                rotation,
                validateCell,
                commitCell,
                createMissingCells: true);
        }

        private void CommitReleaseFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            CellOccupancyValidator validateCell,
            CellOccupancyCommitter commitCell)
        {
            CommitFootprint(
                anchor,
                footprint,
                rotation,
                validateCell,
                commitCell,
                createMissingCells: false);
        }

        private void CommitFootprint(
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation,
            CellOccupancyValidator validateCell,
            CellOccupancyCommitter commitCell,
            bool createMissingCells)
        {
            foreach (MapCellCoord coord in footprint.Resolve(anchor, rotation))
            {
                Cell cell = createMissingCells
                    ? GetOrCreateCell(coord)
                    : GetExistingFootprintCell(coord);
                CellOccupancyValidationResult validation = validateCell(cell);

                if (validation.Outcome != CellOccupancyOutcome.Valid)
                {
                    throw new InvalidOperationException("Cannot commit footprint transaction: validation result is not valid.");
                }

                commitCell(cell, validation);

                if (!createMissingCells && cell.IsEmpty)
                {
                    TryRemoveCell(coord);
                }
            }
        }

        private Cell GetExistingFootprintCell(MapCellCoord coord)
        {
            if (!TryGetCell(coord, out var cell))
            {
                throw new InvalidOperationException("Cannot commit footprint release: target cell was not present.");
            }

            return cell;
        }

        private bool HasGeneratedTerrainAt(MapCellCoord coord)
        {
            return TryGetTerrain(new TerrainTileWorldCoord(coord.X, coord.Z), out _);
        }

        #endregion
    }
}
