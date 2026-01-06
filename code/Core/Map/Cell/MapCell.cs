using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Furniture;
using CosmosCasino.Core.Structure;

namespace CosmosCasino.Core.Map.Cell
{
    /// <summary>
    /// Represents a single logical cell in the map grid.
    /// A cell may contain a floor, a structure, or furniture,
    /// with enforced rules on which elements may coexist.
    /// </summary>
    internal sealed class MapCell
    {
        #region PROPERTIES

        /// <summary>
        /// The floor type assigned to this cell, if any.
        /// </summary>
        internal FloorType? Floor { get; private set; }

        /// <summary>
        /// The structure occupying this cell, if any.
        /// </summary>
        internal StructureType? Structure { get; private set; }

        /// <summary>
        /// The furniture occupying this cell, if any.
        /// </summary>
        internal FurnitureType? Furniture { get; private set; }

        /// <summary>
        /// Indicates whether this cell currently has a floor.
        /// </summary>
        internal bool HasFloor => Floor != null;

        /// <summary>
        /// Indicates whether this cell currently has a structure.
        /// </summary>
        internal bool HasStructure => Structure != null;

        /// <summary>
        /// Indicates whether this cell currently has furniture.
        /// </summary>
        internal bool HasFurniture => Furniture != null;

        /// <summary>
        /// Indicates whether the cell contains no floor, structure, or furniture.
        /// </summary>
        internal bool IsEmpty => !HasFloor && !HasStructure && !HasFurniture;

        #endregion

        #region Floor Methods

        /// <summary>
        /// Attempts to place a floor in this cell.
        /// Fails if a floor already exists.
        /// </summary>
        /// <param name="floor">
        /// The type of floor to place.
        /// </param>
        /// <returns>
        /// A result indicating whether the floor was placed or the operation failed.
        /// </returns>
        internal MapCellResult TryPlaceFloor(FloorType floor)
        {
            if (HasFloor)
            {
                return MapCellResult.Failed(MapCellFailureReason.Blocked);
            }

            Floor = floor;
            return MapCellResult.Placed();
        }

        /// <summary>
        /// Attempts to replace the existing floor with a different floor type.
        /// </summary>
        /// <param name="floor">
        /// The new floor type to apply.
        /// </param>
        /// <returns>
        /// A result indicating whether the floor was replaced, skipped, or failed.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown in debug builds if the cell has no floor, indicating a contract violation.
        /// </exception>
        internal MapCellResult TryReplaceFloor(FloorType floor)
        {
            // This state should be unreachable.
            // The operation contract was violated by higher-level systems.
            // Indicates a programmer error, not gameplay behavior.
            if (!HasFloor)
            {
#if DEBUG
                throw new InvalidOperationException($"{nameof(TryReplaceFloor)} called but cell has no floor.");
#else
                ConsoleLog.Error(nameof(MapCell), $"Invalid state: {nameof(TryReplaceFloor)} called but cell has no floor.");
                return MapCellResult.Failed(MapCellFailureReason.InternalError);
#endif
            }

            if (Floor == floor)
            {
                return MapCellResult.Skipped(MapCellFailureReason.SameType);
            }

            Floor = floor;
            return MapCellResult.Replaced();
        }

        /// <summary>
        /// Attempts to remove the floor from this cell.
        /// Removal is blocked if a structure or furniture is present.
        /// </summary>
        /// <returns>
        /// A result indicating whether the floor was removed or the operation failed.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown in debug builds if the cell has no floor, indicating a contract violation.
        /// </exception>
        internal MapCellResult TryRemoveFloor()
        {
            // This state should be unreachable.
            // The operation contract was violated by higher-level systems.
            // Indicates a programmer error, not gameplay behavior.
            if (!HasFloor)
            {
#if DEBUG
                throw new InvalidOperationException($"{nameof(TryRemoveFloor)} called but cell already has no floor.");
#else
                ConsoleLog.Error(nameof(MapCell), $"Invalid state: {nameof(TryRemoveFloor)} called but cell already has no floor.");
                return MapCellResult.Failed(MapCellFailureReason.InternalError);
#endif
            }

            if (HasStructure || HasFurniture)
            {
                return MapCellResult.Failed(MapCellFailureReason.Blocked);
            }

            Floor = null;
            return MapCellResult.Removed();
        }

        #endregion

        #region Structure Methods

        /// <summary>
        /// Attempts to place a structure in this cell.
        /// Requires a floor and no existing structure or furniture.
        /// </summary>
        /// <param name="structure">
        /// The structure type to place.
        /// </param>
        /// <returns>
        /// A result indicating whether the structure was placed or the operation failed.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown in debug builds if a structure already exists, indicating a contract violation.
        /// </exception>
        internal MapCellResult TryPlaceStructure(StructureType structure)
        {
            // This state should be unreachable.
            // The operation contract was violated by higher-level systems.
            // Indicates a programmer error, not gameplay behavior.
            if (HasStructure)
            {
#if DEBUG
                throw new InvalidOperationException($"{nameof(TryPlaceStructure)} called but already has structure. Call {nameof(TryReplaceStructure)} instead.");
#else
                ConsoleLog.Error(nameof(MapCell), $"Invalid state: {nameof(TryPlaceStructure)} called but already has structure. Call {nameof(TryReplaceStructure)} instead.");
                return MapCellResult.Failed(MapCellFailureReason.InternalError);
#endif
            }

            if (!HasFloor)
            {
                return MapCellResult.Failed(MapCellFailureReason.NoFloor);
            }

            if (HasFurniture)
            {
                return MapCellResult.Failed(MapCellFailureReason.Blocked);
            }

            Structure = structure;
            return MapCellResult.Placed();
        }

        /// <summary>
        /// Attempts to replace the existing structure with a different structure type.
        /// </summary>
        /// <param name="newStructure">
        /// The new structure type to apply.
        /// </param>
        /// <returns>
        /// A result indicating whether the structure was replaced, skipped, or failed.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown in debug builds if the cell state violates replacement contracts.
        /// </exception>
        internal MapCellResult TryReplaceStructure(StructureType newStructure)
        {
            // This state should be unreachable.
            // The operation contract was violated by higher-level systems.
            // Indicates a programmer error, not gameplay behavior.
            if (!HasFloor)
            {
#if DEBUG
                throw new InvalidOperationException($"{nameof(TryReplaceStructure)} called but cell has no floor.");
#else
                ConsoleLog.Error(nameof(MapCell), $"{nameof(TryReplaceStructure)} called but cell has no floor.");
                return MapCellResult.Failed(MapCellFailureReason.InternalError);
#endif
            }

            // This state should be unreachable.
            // The operation contract was violated by higher-level systems.
            // Indicates a programmer error, not gameplay behavior.
            if (!HasStructure)
            {
#if DEBUG
                throw new InvalidOperationException($"{nameof(TryReplaceStructure)} called but cell has no structure.");
#else
                ConsoleLog.Error(nameof(MapCell), $"{nameof(TryReplaceStructure)} called but cell has no structure.");
                return MapCellResult.Failed(MapCellFailureReason.InternalError);
#endif
            }

            // This state should be unreachable.
            // The operation contract was violated by higher-level systems.
            // Indicates a programmer error, not gameplay behavior.
            if (HasFurniture)
            {
#if DEBUG
                throw new InvalidOperationException($"{nameof(TryReplaceStructure)} called but cell has furniture.");
#else
                ConsoleLog.Error(nameof(MapCell), $"{nameof(TryReplaceStructure)} called but cell has furniture.");
                return MapCellResult.Failed(MapCellFailureReason.InternalError);
#endif
            }

            if (Structure == newStructure)
            {
                return MapCellResult.Skipped(MapCellFailureReason.SameType);
            }

            Structure = newStructure;
            return MapCellResult.Replaced();
        }

        /// <summary>
        /// Attempts to remove the structure from this cell.
        /// </summary>
        /// <returns>
        /// A result indicating whether the structure was removed or skipped.
        /// </returns>
        internal MapCellResult TryRemoveStructure()
        {
            if (!HasStructure)
            {
                return MapCellResult.Skipped(MapCellFailureReason.NoStructure);
            }

            Structure = null;
            return MapCellResult.Removed();
        }

        #endregion

        #region Furniture Methods

        /// <summary>
        /// Attempts to place furniture in this cell.
        /// Requires a floor and no existing structure or furniture.
        /// </summary>
        /// <param name="newFurniture">
        /// The furniture type to place.
        /// </param>
        /// <returns>
        /// A result indicating whether the furniture was placed or the operation failed.
        /// </returns>
        internal MapCellResult TryPlaceFurniture(FurnitureType newFurniture)
        {
            if (!HasFloor)
            {
                return MapCellResult.Failed(MapCellFailureReason.NoFloor);
            }

            if (HasFurniture || HasStructure)
            {
                return MapCellResult.Failed(MapCellFailureReason.Blocked);
            }

            Furniture = newFurniture;
            return MapCellResult.Placed();
        }

        /// <summary>
        /// Attempts to remove the furniture from this cell.
        /// </summary>
        /// <returns>
        /// A result indicating whether the furniture was removed or skipped.
        /// </returns>
        internal MapCellResult TryRemoveFurniture()
        {
            if (!HasFurniture)
            {
                return MapCellResult.Skipped(MapCellFailureReason.NoFurniture);
            }

            Furniture = null;
            return MapCellResult.Removed();
        }

        #endregion
    }
}