using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using System;
using System.Collections.Generic;
using System.Linq;

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
    #region Enums

    private enum SnapDirection
    {
        Horizontal,
        Vertical,
        Diagonal
    }

    #endregion

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
    /// <param name="buildOperation">The type of build operation to perform (e.g., place, remove).</param>
    /// <param name="buildInteractionMode">The interaction mode affecting the build operation.</param>
    /// <param name="intent">When this method returns, contains the resulting build intent if the operation succeeds; otherwise, the default
    /// value.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public abstract bool TryCreateBuildIntent(MapCellCoord startCell, MapCellCoord endCell, BuildOperation buildOperation, BuildInteractionMode buildInteractionMode, out BuildIntent intent);

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing the path or sequence of cells between the
    /// specified start and end cells.
    /// </summary>
    /// <param name="startCell">The coordinate of the starting cell. Must be a valid map cell within the bounds of the map.</param>
    /// <param name="endCell">The coordinate of the ending cell. Must be a valid map cell within the bounds of the map.</param>
    /// <param name="buildOperation">The type of build operation to consider when determining the path between the cells.</param>
    /// <param name="buildInteractionMode">The interaction mode affecting the build operation.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> objects representing the cells between <paramref
    /// name="startCell"/> and <paramref name="endCell"/>. The list may be empty if no valid path exists.</returns>
    public abstract IReadOnlyList<MapCellCoord> GetCells(MapCellCoord startCell, MapCellCoord endCell, BuildOperation buildOperation, BuildInteractionMode buildInteractionMode);

    #endregion

    #region Cells Area

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
    protected IReadOnlyList<MapCellCoord> GetCellsRectangleArea(MapCellCoord startCell, MapCellCoord endCell)
    {
        int minX = Math.Min(startCell.X, endCell.X);
        int maxX = Math.Max(startCell.X, endCell.X);

        int minY = Math.Min(startCell.Y, endCell.Y);
        int maxY = Math.Max(startCell.Y, endCell.Y);

        int z = startCell.Z;

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
    /// Returns a read-only list of map cell coordinates representing all cells within
    /// a perfect square area defined by the specified start and end cells.
    /// </summary>
    /// <remarks>
    /// The square is axis-aligned and anchored at <paramref name="startCell"/>.
    /// The size of the square is determined by the dominant axis of the drag
    /// between <paramref name="startCell"/> and <paramref name="endCell"/>.
    /// All returned cells lie on the same Z layer as <paramref name="startCell"/>.
    /// </remarks>
    /// <param name="startCell">
    /// The anchor corner of the square area.
    /// </param>
    /// <param name="endCell">
    /// A cell used to determine the square size and direction.
    /// </param>
    /// <returns>
    /// A read-only list of <see cref="MapCellCoord"/> covering every cell
    /// within the square area, inclusive.
    /// </returns>
    protected IReadOnlyList<MapCellCoord> GetCellsSquareArea(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int dx = endCell.X - startCell.X;
        int dy = endCell.Y - startCell.Y;

        int side = Math.Max(Math.Abs(dx), Math.Abs(dy));

        int sx = dx >= 0 ? 1 : -1;
        int sy = dy >= 0 ? 1 : -1;

        int x0 = startCell.X;
        int y0 = startCell.Y;
        int x1 = startCell.X + (side * sx);
        int y1 = startCell.Y + (side * sy);

        int minX = Math.Min(x0, x1);
        int maxX = Math.Max(x0, x1);
        int minY = Math.Min(y0, y1);
        int maxY = Math.Max(y0, y1);

        int z = startCell.Z;

        var cells = new List<MapCellCoord>(
        (maxX - minX + 1) * (maxY - minY + 1));

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
    /// Calculates and returns all map cell coordinates within the filled area of a circle defined by the specified
    /// start and end cells.
    /// </summary>
    /// <remarks>The returned cells are grouped and filled per row between the minimum and maximum X bounds
    /// for each Y coordinate. All returned cells share the Z value of <paramref name="startCell"/>.</remarks>
    /// <param name="startCell">The starting map cell coordinate that defines one edge of the circle. The Z value of this cell determines the Z
    /// layer for all returned cells.</param>
    /// <param name="endCell">The ending map cell coordinate that defines the opposite edge of the circle. Used together with <paramref
    /// name="startCell"/> to determine the circle's outline.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> objects representing all cells inside the filled circle area. If
    /// the outline is empty, returns an empty list.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsCircleArea(MapCellCoord startCell, MapCellCoord endCell)
    {
        // 1. Get authoritative outline
        var outline = GetCellsCircleLine(startCell, endCell);

        if (outline.Count == 0)
        {
            return outline;
        }

        int z = startCell.Z;

        // 2. Group outline cells by Y
        var rows = new Dictionary<int, (int MinX, int MaxX)>();

        foreach (var cell in outline)
        {
            if (!rows.TryGetValue(cell.Y, out var span))
            {
                rows[cell.Y] = (cell.X, cell.X);
            }
            else
            {
                rows[cell.Y] = (
                    Math.Min(span.MinX, cell.X),
                    Math.Max(span.MaxX, cell.X)
                );
            }
        }

        // 3. Fill between left/right bounds per row
        var filled = new List<MapCellCoord>();

        foreach (var (y, span) in rows)
        {
            for (int x = span.MinX; x <= span.MaxX; x++)
            {
                filled.Add(new MapCellCoord(x, y, z));
            }
        }

        return filled;
    }

    #endregion

    #region Cells Line

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
    protected IReadOnlyList<MapCellCoord> GetCellsStraightLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int z = startCell.Z;

        int x0 = startCell.X;
        int y0 = startCell.Y;
        int x1 = endCell.X;
        int y1 = endCell.Y;

        int dx = x1 - x0;
        int dy = y1 - y0;

        double angle = Math.Atan2(dy, dx) * (180.0 / Math.PI);
        if (angle < 0)
        {
            angle += 360.0;
        }

        SnapDirection direction = GetSnapDirection(angle);

        var cells = new List<MapCellCoord>();

        switch (direction)
        {
            case SnapDirection.Horizontal:
                {
                    int step = x0 <= x1 ? 1 : -1;
                    int length = Math.Abs(dx);

                    for (int i = 0; i <= length; i++)
                    {
                        cells.Add(new MapCellCoord(x0 + (i * step), y0, z));
                    }

                    break;
                }

            case SnapDirection.Vertical:
                {
                    int step = y0 <= y1 ? 1 : -1;
                    int length = Math.Abs(dy);

                    for (int i = 0; i <= length; i++)
                    {
                        cells.Add(new MapCellCoord(x0, y0 + (i * step), z));
                    }

                    break;
                }

            case SnapDirection.Diagonal:
                {
                    int sx = dx >= 0 ? 1 : -1;
                    int sy = dy >= 0 ? 1 : -1;
                    int length = Math.Min(Math.Abs(dx), Math.Abs(dy));

                    for (int i = 0; i <= length; i++)
                    {
                        cells.Add(new MapCellCoord(x0 + (i * sx), y0 + (i * sy), z));
                    }

                    break;
                }
        }

        return cells;
    }

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing a line
    /// between two cells on the same Z level. Supports horizontal, vertical, and diagonal lines.
    /// </summary>
    /// <param name="startCell">The starting cell coordinate.</param>
    /// <param name="endCell">The ending cell coordinate.</param>
    /// <returns>
    /// A read-only list of <see cref="MapCellCoord"/> forming a continuous line
    /// between the two cells, including both endpoints.
    /// </returns>
    protected IReadOnlyList<MapCellCoord> GetCellsDynamicLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int z = startCell.Z;

        int x0 = startCell.X;
        int y0 = startCell.Y;
        int x1 = endCell.X;
        int y1 = endCell.Y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        int steps = Math.Max(dx, dy) + 1;

        var cells = new List<MapCellCoord>(steps);

        for (int i = 0; i < steps; i++)
        {
            cells.Add(new MapCellCoord(x0, y0, z));

            int e2 = err << 1; // err * 2

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return cells;
    }

    /// <summary>
    /// Returns the perimeter cells of an axis-aligned rectangle defined
    /// by two diagonally opposite corners.
    /// </summary>
    /// <param name="startCell">First corner of the rectangle.</param>
    /// <param name="endCell">Opposite corner of the rectangle.</param>
    /// <returns>
    /// A read-only list of <see cref="MapCellCoord"/> representing the
    /// rectangle outline, including corners.
    /// </returns>
    protected IReadOnlyList<MapCellCoord> GetCellsRectangleLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int z = startCell.Z;

        int minX = Math.Min(startCell.X, endCell.X);
        int maxX = Math.Max(startCell.X, endCell.X);
        int minY = Math.Min(startCell.Y, endCell.Y);
        int maxY = Math.Max(startCell.Y, endCell.Y);

        var cells = new List<MapCellCoord>();

        // Top edge (minY)
        for (int x = minX; x <= maxX; x++)
        {
            cells.Add(new MapCellCoord(x, minY, z));
        }

        // Bottom edge (maxY)
        if (maxY != minY)
        {
            for (int x = minX; x <= maxX; x++)
            {
                cells.Add(new MapCellCoord(x, maxY, z));
            }
        }

        // Left edge (excluding corners)
        for (int y = minY + 1; y < maxY; y++)
        {
            cells.Add(new MapCellCoord(minX, y, z));
        }

        // Right edge (excluding corners)
        if (maxX != minX)
        {
            for (int y = minY + 1; y < maxY; y++)
            {
                cells.Add(new MapCellCoord(maxX, y, z));
            }
        }

        return cells;
    }

    /// <summary>
    /// Returns a sequence of map cell coordinates forming a straight line between two cells, extended to the edge of
    /// the square defined by the start and end points.
    /// </summary>
    /// <remarks>If the start and end cells are the same, the returned list contains only the start cell. The
    /// line is always axis-aligned or diagonal, and its length is determined by the greater of the X or Y distance
    /// between the start and end cells.</remarks>
    /// <param name="startCell">The starting cell coordinate of the line.</param>
    /// <param name="endCell">The ending cell coordinate of the line. The line will be extended to the square edge in the direction of this
    /// cell.</param>
    /// <returns>A read-only list of map cell coordinates representing the straight line from the start cell to the square edge
    /// in the direction of the end cell. The list contains at least one cell.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsSquareLine(MapCellCoord startCell, MapCellCoord endCell)
    {
        int z = startCell.Z;

        int dx = endCell.X - startCell.X;
        int dy = endCell.Y - startCell.Y;

        if (dx == 0 && dy == 0)
        {
            return new[] { startCell };
        }

        int side = Math.Max(Math.Abs(dx), Math.Abs(dy));

        int sx = dx >= 0 ? 1 : -1;
        int sy = dy >= 0 ? 1 : -1;

        int endX = startCell.X + (side * sx);
        int endY = startCell.Y + (side * sy);

        MapCellCoord squareEnd = new(endX, endY, z);

        return GetCellsRectangleLine(startCell, squareEnd);
    }

    /// <summary>
    /// Calculates the set of map cell coordinates that form a circle outline between two specified cells on the same Z
    /// level.
    /// </summary>
    /// <remarks>The circle is constructed by fitting the largest possible square between <paramref
    /// name="startCell"/> and <paramref name="endCell"/> and drawing a circle within that square. Only the outline
    /// cells are included; interior cells are not returned. Both input coordinates must be on the same Z level for
    /// correct results.</remarks>
    /// <param name="startCell">The starting cell coordinate, which determines one corner of the square bounding the circle. Must have the same
    /// Z value as <paramref name="endCell"/>.</param>
    /// <param name="endCell">The ending cell coordinate, which determines the opposite corner of the square bounding the circle. Must have
    /// the same Z value as <paramref name="startCell"/>.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> representing the coordinates of cells that lie on the
    /// circumference of the calculated circle. The list will be empty if the input coordinates do not define a valid
    /// circle.</returns>
    protected IReadOnlyList<MapCellCoord> GetCellsCircleLine(MapCellCoord startCell, MapCellCoord endCell)
    {
        int z = startCell.Z;

        // --- Build square from drag ---
        int dx = endCell.X - startCell.X;
        int dy = endCell.Y - startCell.Y;

        int side = Math.Max(Math.Abs(dx), Math.Abs(dy));
        int sx = dx >= 0 ? 1 : -1;
        int sy = dy >= 0 ? 1 : -1;

        int x0 = startCell.X;
        int y0 = startCell.Y;
        int x1 = startCell.X + (side * sx);
        int y1 = startCell.Y + (side * sy);

        int minX = Math.Min(x0, x1);
        int maxX = Math.Max(x0, x1);
        int minY = Math.Min(y0, y1);
        int maxY = Math.Max(y0, y1);

        // --- Circle center & radius ---
        int cx = (minX + maxX) / 2;
        int cy = (minY + maxY) / 2;
        int radius = (maxX - minX) / 2;

        var cells = new HashSet<MapCellCoord>();

        // --- Midpoint circle ---
        int x = radius;
        int y = 0;
        int decision = 1 - radius;

        while (y <= x)
        {
            AddCirclePoints(cells, cx, cy, x, y, z);

            y++;

            if (decision <= 0)
            {
                decision += (2 * y) + 1;
            }
            else
            {
                x--;
                decision += (2 * (y - x)) + 1;
            }
        }

        return cells.ToList();
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Determines the snap direction—horizontal, vertical, or diagonal—based on the specified angle in degrees.
    /// </summary>
    /// <remarks>Angles near 0°, 90°, 180°, and 270° are classified as horizontal or vertical; all other
    /// angles are considered diagonal.</remarks>
    /// <param name="angle">The angle, in degrees, for which to determine the snap direction. The value is normalized to the range 0 to 360
    /// degrees.</param>
    /// <returns>A value of type SnapDirection indicating whether the angle corresponds to a horizontal, vertical, or diagonal
    /// direction.</returns>
    private static SnapDirection GetSnapDirection(double angle)
    {
        // Normalize angle into 0–360
        angle %= 360;

        // Horizontal (0° or 180°)
        if (angle <= 22.5 || angle >= 337.5 ||
            (angle >= 157.5 && angle <= 202.5))
        {
            return SnapDirection.Horizontal;
        }

        // Vertical (90° or 270°)
        if ((angle >= 67.5 && angle <= 112.5) ||
            (angle >= 247.5 && angle <= 292.5))
        {
            return SnapDirection.Vertical;
        }

        // Otherwise → Diagonal
        return SnapDirection.Diagonal;
    }

    private static void AddCirclePoints(
        HashSet<MapCellCoord> cells,
        int cx,
        int cy,
        int x,
        int y,
        int z)
    {
        cells.Add(new MapCellCoord(cx + x, cy + y, z));
        cells.Add(new MapCellCoord(cx - x, cy + y, z));
        cells.Add(new MapCellCoord(cx + x, cy - y, z));
        cells.Add(new MapCellCoord(cx - x, cy - y, z));

        cells.Add(new MapCellCoord(cx + y, cy + x, z));
        cells.Add(new MapCellCoord(cx - y, cy + x, z));
        cells.Add(new MapCellCoord(cx + y, cy - x, z));
        cells.Add(new MapCellCoord(cx - y, cy - x, z));
    }

    #endregion
}