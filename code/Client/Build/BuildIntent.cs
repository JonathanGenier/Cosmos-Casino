using CosmosCasino.Core.Map;

/// <summary>
/// Immutable command representing a single build action
/// requested by the player.
/// </summary>
public sealed class BuildIntent
{
    #region CONSTRUCTOR

    private BuildIntent(FloorType floorType, CellCoord cell)
    {
        FloorType = floorType;
        Cell = cell;
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Type of floor to build.
    /// </summary>
    public FloorType FloorType { get; }

    /// <summary>
    /// Target cell where the build should occur.
    /// </summary>
    public CellCoord Cell { get; }

    #endregion

    #region FACTORIES

    /// <summary>
    /// Creates a build intent representing placement of a floor tile
    /// at the specified grid cell.
    /// </summary>
    /// <param name="floorType">
    /// The type of floor to build.
    /// </param>
    /// <param name="cell">
    /// Target grid cell for the build action.
    /// </param>
    /// <returns>
    /// An immutable build intent describing the requested floor placement.
    /// </returns>
    public static BuildIntent CreateFloor(FloorType floorType, CellCoord cell)
    {
        return new BuildIntent(floorType, cell);
    }

    #endregion

    #region OVERRIDES

    /// <summary>
    /// Returns a human-readable representation of the build intent,
    /// suitable for debugging and logging.
    /// </summary>
    /// <returns>
    /// A string describing the build action and its target cell.
    /// </returns>
    public override string ToString()
    {
        return $"BuildFloor {FloorType} at {Cell}";
    }

    #endregion
}
