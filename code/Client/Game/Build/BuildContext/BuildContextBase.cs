using CosmosCasino.Core.Game.Build;
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

    /// <summary>
    /// Attempts to create a build intent based on the specified start and end cursor contexts.
    /// </summary>
    /// <param name="start">The starting cursor context that defines the beginning of the range for intent creation.</param>
    /// <param name="end">The ending cursor context that defines the end of the range for intent creation.</param>
    /// <param name="intent">When this method returns, contains the resulting build intent if creation was successful; otherwise, null.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public abstract bool TryCreateBuildIntent(CursorContext start, CursorContext end, out BuildIntent intent);

    /// <summary>
    /// Returns a read-only list of map cell coordinates that intersect the line segment between the specified start and
    /// end world positions.
    /// </summary>
    /// <param name="startWorld">The world-space position representing the start point of the line segment.</param>
    /// <param name="endWorld">The world-space position representing the end point of the line segment.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> values corresponding to each map cell intersected by the line
    /// segment. The list will be empty if no cells are intersected.</returns>
    protected abstract IReadOnlyList<MapCellCoord> GetCells(Vector3 startWorld, Vector3 endWorld);

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing the rectangular area between two world positions.
    /// </summary>
    /// <remarks>Both positions are mapped to cell coordinates, and all cells within the axis-aligned
    /// rectangle between them are included. Only the X and Y axes are considered; the Z layer is taken from the
    /// starting position. This method does not check for out-of-bounds coordinates.</remarks>
    /// <param name="startWorld">The world-space position that defines one corner of the area to retrieve. The position is converted to a map
    /// cell coordinate.</param>
    /// <param name="endWorld">The world-space position that defines the opposite corner of the area to retrieve. The position is converted to
    /// a map cell coordinate.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> values covering all cells within the rectangle defined by the two
    /// positions. The list is empty if the area contains no cells.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsArea(Vector3 startWorld, Vector3 endWorld)
    {
        MapCellCoord startCell = MapGrid.WorldToCell(startWorld.X, startWorld.Y, startWorld.Z);
        MapCellCoord endCell = MapGrid.WorldToCell(endWorld.X, endWorld.Y, endWorld.Z);

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
    /// Returns a sequence of map cell coordinates representing a straight wall line between two points in world space.
    /// </summary>
    /// <remarks>The wall line is calculated on the same Z level as the start point. Only horizontal or
    /// vertical lines are supported; diagonal lines are not generated.</remarks>
    /// <param name="startWorld">The starting point of the wall line in world coordinates.</param>
    /// <param name="endWorld">The ending point of the wall line in world coordinates.</param>
    /// <returns>An ordered, read-only list of map cell coordinates that form a straight horizontal or vertical wall between the
    /// specified start and end points. The list will contain all cells along the line, including the endpoints.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsLine(Vector3 startWorld, Vector3 endWorld)
    {
        MapCellCoord start = MapGrid.WorldToCell(startWorld.X, startWorld.Y, startWorld.Z);
        MapCellCoord end = MapGrid.WorldToCell(endWorld.X, endWorld.Y, endWorld.Z);

        int z = start.Z;

        var cells = new List<MapCellCoord>();

        int dx = Math.Abs(end.X - start.X);
        int dy = Math.Abs(end.Y - start.Y);

        if (dx >= dy)
        {
            // Horizontal wall
            int minX = Math.Min(start.X, end.X);
            int maxX = Math.Max(start.X, end.X);

            for (int x = minX; x <= maxX; x++)
            {
                cells.Add(new MapCellCoord(x, start.Y, z));
            }
        }
        else
        {
            // Vertical wall
            int minY = Math.Min(start.Y, end.Y);
            int maxY = Math.Max(start.Y, end.Y);

            for (int y = minY; y <= maxY; y++)
            {
                cells.Add(new MapCellCoord(start.X, y, z));
            }
        }

        return cells;
    }
    #endregion
}
