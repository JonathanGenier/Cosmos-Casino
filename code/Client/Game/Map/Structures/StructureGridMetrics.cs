using CosmosCasino.Core.Game.Map;
using Godot;

/// <summary>
/// Client-owned physical presentation metrics for authoritative Structure cells.
/// </summary>
internal static class StructureGridMetrics
{
    #region Constants

    /// <summary>
    /// Width, height, and depth of one canonical structural cell in Godot world units.
    /// </summary>
    internal const float CellSize = WorldGridMetrics.GridUnitSize;

    /// <summary>
    /// Half extent of one canonical structural cell in Godot world units.
    /// </summary>
    internal const float HalfCellSize = CellSize * 0.5f;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the local bounds of one canonical structural cell.
    /// </summary>
    internal static Vector3 CellBoundsSize { get; } = new(CellSize, CellSize, CellSize);

    #endregion

    #region Coordinates

    /// <summary>
    /// Converts a global logical Structure cell into the Godot world-space center of that authoritative cell.
    /// </summary>
    /// <param name="cell">The authoritative Structure cell coordinate.</param>
    /// <returns>The Godot world-space center for <paramref name="cell"/>.</returns>
    internal static Vector3 ToGodotCenter(MapCellCoord cell)
    {
        return cell.ToGodotCenter();
    }

    /// <summary>
    /// Converts a global logical Structure cell into a section-local center using authoritative coordinate spacing.
    /// </summary>
    /// <param name="originCell">The section's minimum global cell.</param>
    /// <param name="cell">The global logical Structure cell.</param>
    /// <returns>The section-local center for <paramref name="cell"/>.</returns>
    internal static Vector3 ToSectionLocalCenter(
        MapCellCoord originCell,
        MapCellCoord cell)
    {
        return ToGodotCenter(cell) - ToGodotCenter(originCell);
    }

    #endregion
}
