using CosmosCasino.Core.Game.Map;

/// <summary>
/// Inclusive global logical cell bounds for one Client structure render section.
/// </summary>
internal readonly struct StructureRenderSectionBounds
{
    #region Initialization

    /// <summary>
    /// Initializes inclusive logical bounds for one render section.
    /// </summary>
    /// <param name="min">The minimum global cell coordinate.</param>
    /// <param name="max">The maximum global cell coordinate.</param>
    internal StructureRenderSectionBounds(MapCellCoord min, MapCellCoord max)
    {
        Min = min;
        Max = max;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the inclusive minimum global cell coordinate.
    /// </summary>
    internal MapCellCoord Min { get; }

    /// <summary>
    /// Gets the inclusive maximum global cell coordinate.
    /// </summary>
    internal MapCellCoord Max { get; }

    #endregion

    #region Queries

    /// <summary>
    /// Determines whether the specified global cell is inside these bounds.
    /// </summary>
    /// <param name="cell">The global logical cell to test.</param>
    /// <returns><c>true</c> when the cell is inside the bounds; otherwise, <c>false</c>.</returns>
    internal bool Contains(MapCellCoord cell)
    {
        return cell.X >= Min.X
            && cell.X <= Max.X
            && cell.Y >= Min.Y
            && cell.Y <= Max.Y
            && cell.Z >= Min.Z
            && cell.Z <= Max.Z;
    }

    #endregion
}
