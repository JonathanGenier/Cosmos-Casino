using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Provides build context logic for constructing walls within a map, including cell selection and build intent
/// creation.
/// </summary>
/// <remarks>This class specializes the build process for wall structures by determining which map cells are
/// affected and generating the appropriate build intent. It is intended for use in scenarios where wall placement and
/// construction are required. Instances of this class are immutable and thread-safe.</remarks>
public sealed class WallBuildContext : BuildContextBase
{
    #region Properties

    /// <summary>
    /// Gets the type of build operation represented by this instance.
    /// </summary>
    public override BuildKind Kind => BuildKind.Wall;

    #endregion

    #region Build Intent

    /// <summary>
    /// Attempts to create a build intent representing a wall between the specified start and end cells.
    /// </summary>
    /// <param name="startCell">The starting cell coordinate for the wall.</param>
    /// <param name="endCell">The ending cell coordinate for the wall.</param>
    /// <param name="intent">When this method returns, contains the created build intent if successful; otherwise, null.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public override bool TryCreateBuildIntent(MapCellCoord startCell, MapCellCoord endCell, out BuildIntent intent)
    {
        var cells = GetCells(startCell, endCell);

        if (cells.Count == 0)
        {
            intent = null!;
            return false;
        }

        intent = BuildIntent.BuildWall(cells);
        return true;
    }

    #endregion

    #region Get Cells

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing the straight line path between the specified start
    /// and end cells.
    /// </summary>
    /// <param name="startCell">The coordinate of the cell where the line starts.</param>
    /// <param name="endCell">The coordinate of the cell where the line ends.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> values that trace the line from <paramref name="startCell"/> to
    /// <paramref name="endCell"/>. The list includes both endpoints.</returns>
    public override IReadOnlyList<MapCellCoord> GetCells(MapCellCoord startCell, MapCellCoord endCell)
    {
        return GetCellsLine(startCell, endCell);
    }

    #endregion
}
