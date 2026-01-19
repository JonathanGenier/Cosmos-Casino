using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using System.Collections.Generic;

/// <summary>
/// Provides build context and logic for constructing floor elements within a map grid.
/// </summary>
/// <remarks>Use this class to interpret user input or cursor positions as floor-building operations. The context
/// determines the set of map cells to be affected and produces build intents specific to floor construction. This type
/// is sealed and cannot be inherited.</remarks>
public sealed class FloorBuildContext : BuildContextBase
{
    #region Properties

    /// <summary>
    /// Gets the type of build represented by this instance.
    /// </summary>
    public override BuildKind Kind => BuildKind.Floor;

    #endregion

    #region Build Intent

    /// <summary>
    /// Attempts to create a build intent for constructing a floor between the specified start and end map cells.
    /// </summary>
    /// <param name="startCell">The starting cell coordinate of the intended floor area.</param>
    /// <param name="endCell">The ending cell coordinate of the intended floor area.</param>
    /// <param name="intent">When this method returns, contains the build intent for the floor if the operation succeeds; otherwise, null.
    /// This parameter is passed uninitialized.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public override bool TryCreateBuildIntent(MapCellCoord startCell, MapCellCoord endCell, out BuildIntent intent)
    {
        var cells = GetCells(startCell, endCell);

        if (cells.Count == 0)
        {
            intent = null!;
            return false;
        }

        intent = BuildIntent.BuildFloor(cells);
        return true;
    }

    #endregion

    #region Get Cells

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing all cells within the rectangular area defined by
    /// the specified start and end cells.
    /// </summary>
    /// <param name="startCell">The coordinate of the first cell that defines one corner of the area to retrieve. Must be within the bounds of
    /// the map.</param>
    /// <param name="endCell">The coordinate of the second cell that defines the opposite corner of the area to retrieve. Must be within the
    /// bounds of the map.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> objects representing all cells within the area defined by
    /// <paramref name="startCell"/> and <paramref name="endCell"/>. The list will be empty if the area contains no
    /// cells.</returns>
    public override IReadOnlyList<MapCellCoord> GetCells(MapCellCoord startCell, MapCellCoord endCell)
    {
        return GetCellsArea(startCell, endCell);
    }

    #endregion
}
