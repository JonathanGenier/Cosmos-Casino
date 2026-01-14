using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Map.Cell;
using CosmosCasino.Core.Game.Map.Grid;
using Godot;
using System;
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
    /// Attempts to create a build intent for constructing a floor between the specified start and end cursor contexts.
    /// </summary>
    /// <remarks>If no valid cells are found between the specified start and end positions, the method returns
    /// false and sets <paramref name="intent"/> to <see langword="null"/>.</remarks>
    /// <param name="start">The starting cursor context, representing the initial world position for the build intent.</param>
    /// <param name="end">The ending cursor context, representing the final world position for the build intent.</param>
    /// <param name="intent">When this method returns, contains the build intent for constructing a floor if successful; otherwise, <see
    /// langword="null"/>.</param>
    /// <returns>true if a valid build intent was created; otherwise, false.</returns>
    public override bool TryCreateBuildIntent(CursorContext start, CursorContext end, out BuildIntent intent)
    {
        var cells = GetCells(start.WorldPosition, end.WorldPosition);

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
    /// Returns a read-only list of map cell coordinates that lie within the area defined by the specified start and end
    /// world positions.
    /// </summary>
    /// <param name="startWorld">The world-space position representing one corner of the area to retrieve cells from.</param>
    /// <param name="endWorld">The world-space position representing the opposite corner of the area to retrieve cells from.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> values representing all cells within the specified area. The list
    /// will be empty if no cells are found in the area.</returns>
    protected override IReadOnlyList<MapCellCoord> GetCells(Vector3 startWorld, Vector3 endWorld)
    {
        return GetCellsArea(startWorld, endWorld);
    }

    #endregion
}
