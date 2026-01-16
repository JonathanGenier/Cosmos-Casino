using CosmosCasino.Core.Game.Build;
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
    /// Attempts to create a build intent for constructing a wall between the specified start and end cursor contexts.
    /// </summary>
    /// <remarks>If no valid cells are found between the specified start and end positions, the method returns
    /// false and the output intent is set to <see langword="null"/>.</remarks>
    /// <param name="start">The starting cursor context, representing the initial position for the wall.</param>
    /// <param name="end">The ending cursor context, representing the final position for the wall.</param>
    /// <param name="intent">When this method returns, contains the build intent for the wall if creation was successful; otherwise, <see
    /// langword="null"/>.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public override bool TryCreateBuildIntent(CursorContext start, CursorContext end, out BuildIntent intent)
    {
        var cells = GetCells(start.WorldPosition, end.WorldPosition);

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
    /// Returns a list of map cell coordinates that lie within the rectangular area defined by two world-space
    /// positions.
    /// </summary>
    /// <remarks>The method assumes that both positions are on the same Z layer. The returned cells are
    /// ordered by X and then by Y coordinate.</remarks>
    /// <param name="startWorld">The world-space position representing one corner of the rectangular area.</param>
    /// <param name="endWorld">The world-space position representing the opposite corner of the rectangular area.</param>
    /// <returns>A read-only list of map cell coordinates that are contained within the rectangle defined by the two world
    /// positions. The list includes all cells between the two corners, inclusive.</returns>
    public override IReadOnlyList<MapCellCoord> GetCells(Vector3 startWorld, Vector3 endWorld)
    {
        return GetCellsLine(startWorld, endWorld);
    }

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
