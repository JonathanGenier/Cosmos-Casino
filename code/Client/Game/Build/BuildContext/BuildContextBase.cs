using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using CosmosCasino.Core.Game.Map.Grid;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides a base class for build context objects that encapsulate information about a build operation, including its
/// type and intent.
/// </summary>
/// <remarks>This class is intended to be inherited by types that define specific build contexts for various build
/// operations. It exposes members for identifying the build kind and for attempting to create a build intent based on
/// cursor positions. Implementations should ensure that the context accurately reflects the state and requirements of
/// the build process.</remarks>
public abstract class BuildContextBase
{
    #region Properties

    /// <summary>
    /// Gets the kind of build this context represents.
    /// Used to validate and route build intent.
    /// </summary>
    public abstract BuildKind Kind { get; }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Attempts to create a build intent between the specified start and end map cells.
    /// </summary>
    /// <param name="startCell">The coordinates of the starting map cell for the build intent.</param>
    /// <param name="endCell">The coordinates of the ending map cell for the build intent.</param>
    /// <param name="intent">When this method returns, contains the resulting build intent if the operation succeeds; otherwise, the default
    /// value.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public abstract bool TryCreateBuildIntent(MapCellCoord startCell, MapCellCoord endCell, out BuildIntent intent);

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing the path or sequence of cells between the
    /// specified start and end cells.
    /// </summary>
    /// <param name="startCell">The coordinate of the starting cell. Must be a valid map cell within the bounds of the map.</param>
    /// <param name="endCell">The coordinate of the ending cell. Must be a valid map cell within the bounds of the map.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> objects representing the cells between <paramref
    /// name="startCell"/> and <paramref name="endCell"/>. The list may be empty if no valid path exists.</returns>
    public abstract IReadOnlyList<MapCellCoord> GetCells(MapCellCoord startCell, MapCellCoord endCell);

    #endregion

    #region Get Cells Methods

    /// <summary>
    /// Returns a read-only list of map cell coordinates that cover the area defined by the specified start and end
    /// world positions.
    /// </summary>
    /// <param name="startWorld">The world position representing one corner of the area to retrieve. Typically specified in world coordinates.</param>
    /// <param name="endWorld">The world position representing the opposite corner of the area to retrieve. Typically specified in world
    /// coordinates.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> values representing all map cells within the area bounded by
    /// <paramref name="startWorld"/> and <paramref name="endWorld"/>.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsArea(Vector3 startWorld, Vector3 endWorld)
    {
        MapCellCoord startCell = MapGrid.WorldToCell(startWorld.X, startWorld.Y, startWorld.Z);
        MapCellCoord endCell = MapGrid.WorldToCell(endWorld.X, endWorld.Y, endWorld.Z);

        return GetCellsArea(startCell, endCell);
    }

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing all cells within the rectangular area defined by
    /// the specified start and end cells on the same layer.
    /// </summary>
    /// <remarks>The area includes all cells whose X and Y coordinates fall between those of startCell and
    /// endCell, inclusive. The Z coordinate of all returned cells matches that of startCell; the method does not
    /// support areas spanning multiple layers.</remarks>
    /// <param name="startCell">The coordinate of one corner of the area to retrieve. The X, Y, and Z values specify the position in the map
    /// grid.</param>
    /// <param name="endCell">The coordinate of the opposite corner of the area to retrieve. The X, Y, and Z values specify the position in
    /// the map grid.</param>
    /// <returns>A read-only list of MapCellCoord objects covering every cell within the rectangle defined by startCell and
    /// endCell, inclusive. All returned cells are on the same Z layer as startCell.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsArea(MapCellCoord startCell, MapCellCoord endCell)
    {
        int minX = Math.Min(startCell.X, endCell.X);
        int maxX = Math.Max(startCell.X, endCell.X);

        int minY = Math.Min(startCell.Y, endCell.Y);
        int maxY = Math.Max(startCell.Y, endCell.Y);

        int z = startCell.Z; // assume same layer for now

        var cells = new List<MapCellCoord>();

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                cells.Add(new MapCellCoord(x, y, z));
            }
        }

        return cells;
    }

    /// <summary>
    /// Returns a read-only list of map cell coordinates that form a straight line between two points in world space.
    /// </summary>
    /// <param name="startWorld">The starting point of the line, specified in world coordinates.</param>
    /// <param name="endWorld">The ending point of the line, specified in world coordinates.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> objects representing the cells traversed by the line from
    /// <paramref name="startWorld"/> to <paramref name="endWorld"/>. The list is ordered from the start cell to the end
    /// cell.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsLine(Vector3 startWorld, Vector3 endWorld)
    {
        MapCellCoord startCell = MapGrid.WorldToCell(startWorld.X, startWorld.Y, startWorld.Z);
        MapCellCoord endCell = MapGrid.WorldToCell(endWorld.X, endWorld.Y, endWorld.Z);

        return GetCellsLine(startCell, endCell);
    }

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing a straight horizontal or vertical line between two
    /// specified cells on the same Z level.
    /// </summary>
    /// <remarks>The line will be horizontal if the X coordinates differ by more than the Y coordinates;
    /// otherwise, it will be vertical. Only lines aligned strictly along the X or Y axis are supported. The Z
    /// coordinate of all returned cells matches the Z coordinate of the input cells.</remarks>
    /// <param name="startCell">The starting cell coordinate of the line. Must be on the same Z level as <paramref name="endCell"/>.</param>
    /// <param name="endCell">The ending cell coordinate of the line. Must be on the same Z level as <paramref name="startCell"/>.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> objects forming a straight line between <paramref
    /// name="startCell"/> and <paramref name="endCell"/>. The list includes both endpoints and all intermediate cells
    /// along the line.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsLine(MapCellCoord startCell, MapCellCoord endCell)
    {
        int z = startCell.Z;

        var cells = new List<MapCellCoord>();

        int dx = Math.Abs(endCell.X - startCell.X);
        int dy = Math.Abs(endCell.Y - startCell.Y);

        if (dx >= dy)
        {
            // Horizontal wall
            int minX = Math.Min(startCell.X, endCell.X);
            int maxX = Math.Max(startCell.X, endCell.X);

            for (int x = minX; x <= maxX; x++)
            {
                cells.Add(new MapCellCoord(x, startCell.Y, z));
            }
        }
        else
        {
            // Vertical wall
            int minY = Math.Min(startCell.Y, endCell.Y);
            int maxY = Math.Max(startCell.Y, endCell.Y);

            for (int y = minY; y <= maxY; y++)
            {
                cells.Add(new MapCellCoord(startCell.X, y, z));
            }
        }

        return cells;
    }

    #endregion
}