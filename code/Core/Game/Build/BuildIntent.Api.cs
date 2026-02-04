using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Build
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
        /// High-level category of build.
        /// </param>
        /// <param name="operation">
        /// Operation to perform (place, replace, remove).
        /// </param>
        private BuildIntent(IReadOnlyList<MapCoord> cells, BuildKind kind, BuildOperation operation)
        {
            Cells = cells;
            Kind = kind;
            Operation = operation;
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
        public IReadOnlyList<MapCoord> Cells { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a build intent to place a floor on the specified map cells.
        /// </summary>
        /// <param name="cells">A read-only list of map cell coordinates where the floor will be placed. Cannot be null or empty.</param>
        /// <returns>A BuildIntent representing the operation to place a floor on the specified cells.</returns>
        public static BuildIntent PlaceFloor(IReadOnlyList<MapCoord> cells)
        {
            ValidateCells(cells);

            return new BuildIntent(
                cells.ToArray(),
                BuildKind.Floor,
                BuildOperation.Place);
        }

        /// <summary>
        /// Creates a build intent that removes the floor from the specified map cells.
        /// </summary>
        /// <param name="cells">A read-only list of map cell coordinates identifying the cells from which the floor should be removed.
        /// Cannot be null or empty.</param>
        /// <returns>A BuildIntent representing the removal of the floor from the specified cells.</returns>
        public static BuildIntent RemoveFloor(IReadOnlyList<MapCoord> cells)
        {
            ValidateCells(cells);

            return new BuildIntent(
                cells.ToArray(),
                BuildKind.Floor,
                BuildOperation.Remove);
        }

        /// <summary>
        /// Creates a build intent to place a wall on the specified map cells.
        /// </summary>
        /// <param name="cells">A read-only list of map cell coordinates where the wall will be placed. Cannot be null or empty.</param>
        /// <returns>A BuildIntent representing the action to place a wall on the specified cells.</returns>
        public static BuildIntent PlaceWall(IReadOnlyList<MapCoord> cells)
        {
            ValidateCells(cells);

            return new BuildIntent(
                cells.ToArray(),
                BuildKind.Wall,
                BuildOperation.Place);
        }

        /// <summary>
        /// Creates a build intent to remove wall structures from the specified map cells.
        /// </summary>
        /// <param name="cells">A read-only list of map cell coordinates that identify the locations from which walls should be removed.
        /// Cannot be null or empty.</param>
        /// <returns>A BuildIntent representing the removal of walls from the specified cells.</returns>
        public static BuildIntent RemoveWall(IReadOnlyList<MapCoord> cells)
        {
            ValidateCells(cells);

            return new BuildIntent(
                cells.ToArray(),
                BuildKind.Wall,
                BuildOperation.Remove);
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

