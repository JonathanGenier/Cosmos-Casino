using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Furniture;
using CosmosCasino.Core.Map.Cell;
using CosmosCasino.Core.Structure;

namespace CosmosCasino.Core.Build
{
    /// <summary>
    /// Immutable command describing a single build action to be applied
    /// to one or more target cells.
    /// </summary>
    public sealed partial class BuildIntent
    {
        #region Constructor

        /// <summary>
        /// Initializes a new build intent with the specified parameters.
        /// This constructor is private to enforce creation through
        /// validated factory methods.
        /// </summary>
        /// <param name="cells">
        /// Target cells affected by the build operation.
        /// </param>
        /// <param name="kind">
        /// High-level category of build (floor, structure, furniture).
        /// </param>
        /// <param name="operation">
        /// Operation to perform (place, replace, remove).
        /// </param>
        /// <param name="floor">
        /// Floor type associated with the intent, if applicable.
        /// </param>
        /// <param name="structure">
        /// Structure type associated with the intent, if applicable.
        /// </param>
        /// <param name="furniture">
        /// Furniture type associated with the intent, if applicable.
        /// </param>
        private BuildIntent(
            IReadOnlyList<MapCellCoord> cells,
            BuildKind kind,
            BuildOperation operation,
            FloorType? floor,
            StructureType? structure,
            FurnitureType? furniture)
        {
            Cells = cells;
            Kind = kind;
            Operation = operation;
            FloorType = floor;
            StructureType = structure;
            FurnitureType = furniture;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the high-level category of build this intent represents.
        /// </summary>
        public BuildKind Kind { get; }

        /// <summary>
        /// Gets the specific operation to perform for this build intent.
        /// </summary>
        public BuildOperation Operation { get; }

        /// <summary>
        /// Gets the collection of target cells where the build operation
        /// should be applied.
        /// </summary>
        public IReadOnlyList<MapCellCoord> Cells { get; }

        /// <summary>
        /// Gets the floor type associated with this intent, if any.
        /// </summary>
        public FloorType? FloorType { get; }

        /// <summary>
        /// Gets the structure type associated with this intent, if any.
        /// </summary>
        public StructureType? StructureType { get; }

        /// <summary>
        /// Gets the furniture type associated with this intent, if any.
        /// </summary>
        public FurnitureType? FurnitureType { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a build intent representing a floor placement operation
        /// over the specified target cells.
        /// </summary>
        /// <param name="cells">
        /// Cells where the floor should be placed.
        /// </param>
        /// <param name="floor">
        /// Type of floor to place.
        /// </param>
        /// <returns>
        /// A validated floor placement build intent.
        /// </returns>
        public static BuildIntent BuildFloor(IReadOnlyList<MapCellCoord> cells, FloorType floor)
        {
            ValidateCells(cells);

            return new BuildIntent(
                cells.ToArray(),
                BuildKind.Floor,
                BuildOperation.Place,
                floor,
                null,
                null);
        }

        /// <summary>
        /// Creates a build intent representing a floor removal operation
        /// over the specified target cells.
        /// </summary>
        /// <param name="cells">
        /// Cells where the floor should be removed.
        /// </param>
        /// <returns>
        /// A validated floor removal build intent.
        /// </returns>
        public static BuildIntent RemoveFloor(IReadOnlyList<MapCellCoord> cells)
        {
            ValidateCells(cells);

            return new BuildIntent(
                cells.ToArray(),
                BuildKind.Floor,
                BuildOperation.Remove,
                null,
                null,
                null);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a human-readable representation of the build intent,
        /// suitable for debugging and logging.
        /// </summary>
        /// <returns>
        /// A string describing the build action and its target cell.
        /// </returns>
        public override string ToString()
        {
            return $"{Operation} {Kind} for {Cells.Count} cells";
        }

        #endregion
    }
}

