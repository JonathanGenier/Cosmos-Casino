using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;

/// <summary>
/// Resolves Client-side structure drag interactions into deterministic map-cell selections.
/// </summary>
public static class StructureDragCellResolver
{
    #region Enums

    private enum SnapDirection
    {
        Horizontal,
        Vertical,
        Diagonal
    }

    #endregion

    #region Public API

    /// <summary>
    /// Resolves the map cells affected by a build drag.
    /// </summary>
    /// <param name="buildTool">The Client-side build tool selected by the player.</param>
    /// <param name="buildOperation">The requested build operation.</param>
    /// <param name="buildInteractionMode">The active modifier-derived interaction mode.</param>
    /// <param name="startTarget">The cursor target where the drag began.</param>
    /// <param name="currentTarget">The current cursor target.</param>
    /// <returns>The resolved map cells in deterministic order, without duplicates.</returns>
    public static IReadOnlyList<MapCellCoord> Resolve(
        StructureBuildTool buildTool,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        CursorTarget startTarget,
        CursorTarget currentTarget)
    {
        return buildOperation switch
        {
            BuildOperation.Place => ResolvePlacement(
                buildTool,
                buildInteractionMode,
                startTarget.PlacementCell,
                currentTarget.PlacementCell),
            BuildOperation.Remove => ResolveRemoval(
                startTarget.TargetCell,
                currentTarget.TargetCell),
            BuildOperation.None => Array.Empty<MapCellCoord>(),
            _ => throw new InvalidOperationException($"Unsupported build operation: {buildOperation}")
        };
    }

    #endregion

    #region Placement

    private static IReadOnlyList<MapCellCoord> ResolvePlacement(
        StructureBuildTool buildTool,
        BuildInteractionMode buildInteractionMode,
        MapCellCoord startCell,
        MapCellCoord currentCell)
    {
        MapCellCoord endCell = WithStartingY(startCell, currentCell);

        return buildTool switch
        {
            StructureBuildTool.Floor => ResolveFloorPlacement(
                startCell,
                endCell,
                buildInteractionMode),
            StructureBuildTool.Wall => ResolveWallPlacement(
                startCell,
                endCell,
                buildInteractionMode),
            _ => throw new InvalidOperationException($"Unsupported structure build tool: {buildTool}")
        };
    }

    private static IReadOnlyList<MapCellCoord> ResolveFloorPlacement(
        MapCellCoord startCell,
        MapCellCoord endCell,
        BuildInteractionMode buildInteractionMode)
    {
        return buildInteractionMode switch
        {
            BuildInteractionMode.Default => GetCellsRectangleArea(startCell, endCell),
            BuildInteractionMode.ShiftAlternative => GetCellsSquareArea(startCell, endCell),
            BuildInteractionMode.CtrlAlternative => GetCellsStraightLine(startCell, endCell),
            BuildInteractionMode.AltAlternative => GetCellsDynamicLine(startCell, endCell),
            BuildInteractionMode.ShiftCtrlAlternative => GetCellsCircleArea(startCell, endCell),
            _ => throw new InvalidOperationException($"Unsupported build interaction mode: {buildInteractionMode}")
        };
    }

    private static IReadOnlyList<MapCellCoord> ResolveWallPlacement(
        MapCellCoord startCell,
        MapCellCoord endCell,
        BuildInteractionMode buildInteractionMode)
    {
        return buildInteractionMode switch
        {
            BuildInteractionMode.Default => GetCellsRectangleLine(startCell, endCell),
            BuildInteractionMode.ShiftAlternative => GetCellsSquareLine(startCell, endCell),
            BuildInteractionMode.CtrlAlternative => GetCellsStraightLine(startCell, endCell),
            BuildInteractionMode.AltAlternative => GetCellsDynamicLine(startCell, endCell),
            BuildInteractionMode.ShiftCtrlAlternative => GetCellsCircleLine(startCell, endCell),
            _ => throw new InvalidOperationException($"Unsupported build interaction mode: {buildInteractionMode}")
        };
    }

    #endregion

    #region Removal

    private static IReadOnlyList<MapCellCoord> ResolveRemoval(
        MapCellCoord startCell,
        MapCellCoord currentCell)
    {
        return GetCellsRectangleArea(startCell, WithStartingY(startCell, currentCell));
    }

    #endregion

    #region Areas

    private static IReadOnlyList<MapCellCoord> GetCellsRectangleArea(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int minX = Math.Min(startCell.X, endCell.X);
        int maxX = Math.Max(startCell.X, endCell.X);
        int minZ = Math.Min(startCell.Z, endCell.Z);
        int maxZ = Math.Max(startCell.Z, endCell.Z);

        var cells = new List<MapCellCoord>((maxX - minX + 1) * (maxZ - minZ + 1));

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                cells.Add(new MapCellCoord(x, startCell.Y, z));
            }
        }

        return cells;
    }

    private static IReadOnlyList<MapCellCoord> GetCellsSquareArea(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        MapCellCoord squareEnd = GetSquareEnd(startCell, endCell);
        return GetCellsRectangleArea(startCell, squareEnd);
    }

    private static IReadOnlyList<MapCellCoord> GetCellsCircleArea(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        IReadOnlyList<MapCellCoord> outline = GetCellsCircleLine(startCell, endCell);

        if (outline.Count == 0)
        {
            return outline;
        }

        var rows = new SortedDictionary<int, (int MinX, int MaxX)>();

        foreach (MapCellCoord cell in outline)
        {
            if (!rows.TryGetValue(cell.Z, out var span))
            {
                rows[cell.Z] = (cell.X, cell.X);
            }
            else
            {
                rows[cell.Z] = (
                    Math.Min(span.MinX, cell.X),
                    Math.Max(span.MaxX, cell.X));
            }
        }

        var filled = new List<MapCellCoord>();

        foreach (var (z, span) in rows)
        {
            for (int x = span.MinX; x <= span.MaxX; x++)
            {
                filled.Add(new MapCellCoord(x, startCell.Y, z));
            }
        }

        return filled;
    }

    #endregion

    #region Lines

    private static IReadOnlyList<MapCellCoord> GetCellsRectangleLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int minX = Math.Min(startCell.X, endCell.X);
        int maxX = Math.Max(startCell.X, endCell.X);
        int minZ = Math.Min(startCell.Z, endCell.Z);
        int maxZ = Math.Max(startCell.Z, endCell.Z);

        var cells = new List<MapCellCoord>();

        for (int x = minX; x <= maxX; x++)
        {
            cells.Add(new MapCellCoord(x, startCell.Y, minZ));
        }

        if (maxZ != minZ)
        {
            for (int x = minX; x <= maxX; x++)
            {
                cells.Add(new MapCellCoord(x, startCell.Y, maxZ));
            }
        }

        for (int z = minZ + 1; z < maxZ; z++)
        {
            cells.Add(new MapCellCoord(minX, startCell.Y, z));
        }

        if (maxX != minX)
        {
            for (int z = minZ + 1; z < maxZ; z++)
            {
                cells.Add(new MapCellCoord(maxX, startCell.Y, z));
            }
        }

        return cells;
    }

    private static IReadOnlyList<MapCellCoord> GetCellsSquareLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        if (startCell.X == endCell.X && startCell.Z == endCell.Z)
        {
            return new[] { startCell };
        }

        return GetCellsRectangleLine(startCell, GetSquareEnd(startCell, endCell));
    }

    private static IReadOnlyList<MapCellCoord> GetCellsStraightLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int x0 = startCell.X;
        int z0 = startCell.Z;
        int x1 = endCell.X;
        int z1 = endCell.Z;

        int dx = x1 - x0;
        int dz = z1 - z0;

        double angle = Math.Atan2(dz, dx) * (180.0 / Math.PI);

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
                        cells.Add(new MapCellCoord(x0 + (i * step), startCell.Y, z0));
                    }

                    break;
                }

            case SnapDirection.Vertical:
                {
                    int step = z0 <= z1 ? 1 : -1;
                    int length = Math.Abs(dz);

                    for (int i = 0; i <= length; i++)
                    {
                        cells.Add(new MapCellCoord(x0, startCell.Y, z0 + (i * step)));
                    }

                    break;
                }

            case SnapDirection.Diagonal:
                {
                    int sx = dx >= 0 ? 1 : -1;
                    int sz = dz >= 0 ? 1 : -1;
                    int length = Math.Min(Math.Abs(dx), Math.Abs(dz));

                    for (int i = 0; i <= length; i++)
                    {
                        cells.Add(new MapCellCoord(x0 + (i * sx), startCell.Y, z0 + (i * sz)));
                    }

                    break;
                }
        }

        return cells;
    }

    private static IReadOnlyList<MapCellCoord> GetCellsDynamicLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        int x0 = startCell.X;
        int z0 = startCell.Z;
        int x1 = endCell.X;
        int z1 = endCell.Z;

        int dx = Math.Abs(x1 - x0);
        int dz = Math.Abs(z1 - z0);

        int sx = x0 < x1 ? 1 : -1;
        int sz = z0 < z1 ? 1 : -1;

        int err = dx - dz;
        int steps = Math.Max(dx, dz) + 1;

        var cells = new List<MapCellCoord>(steps);

        for (int i = 0; i < steps; i++)
        {
            cells.Add(new MapCellCoord(x0, startCell.Y, z0));

            int e2 = err << 1;

            if (e2 > -dz)
            {
                err -= dz;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                z0 += sz;
            }
        }

        return cells;
    }

    private static IReadOnlyList<MapCellCoord> GetCellsCircleLine(
        MapCellCoord startCell,
        MapCellCoord endCell)
    {
        MapCellCoord squareEnd = GetSquareEnd(startCell, endCell);

        int minX = Math.Min(startCell.X, squareEnd.X);
        int maxX = Math.Max(startCell.X, squareEnd.X);
        int minZ = Math.Min(startCell.Z, squareEnd.Z);
        int maxZ = Math.Max(startCell.Z, squareEnd.Z);

        int cx = (minX + maxX) / 2;
        int cz = (minZ + maxZ) / 2;
        int radius = (maxX - minX) / 2;

        var seen = new HashSet<MapCellCoord>();
        var cells = new List<MapCellCoord>();

        int x = radius;
        int z = 0;
        int decision = 1 - radius;

        while (z <= x)
        {
            AddCirclePoints(cells, seen, cx, cz, x, z, startCell.Y);

            z++;

            if (decision <= 0)
            {
                decision += (2 * z) + 1;
            }
            else
            {
                x--;
                decision += (2 * (z - x)) + 1;
            }
        }

        return cells;
    }

    #endregion

    #region Helpers

    private static MapCellCoord GetSquareEnd(MapCellCoord startCell, MapCellCoord endCell)
    {
        int dx = endCell.X - startCell.X;
        int dz = endCell.Z - startCell.Z;

        int side = Math.Max(Math.Abs(dx), Math.Abs(dz));
        int sx = dx >= 0 ? 1 : -1;
        int sz = dz >= 0 ? 1 : -1;

        return new MapCellCoord(
            startCell.X + (side * sx),
            startCell.Y,
            startCell.Z + (side * sz));
    }

    private static SnapDirection GetSnapDirection(double angle)
    {
        angle %= 360;

        if (angle <= 22.5 || angle >= 337.5 ||
            (angle >= 157.5 && angle <= 202.5))
        {
            return SnapDirection.Horizontal;
        }

        if ((angle >= 67.5 && angle <= 112.5) ||
            (angle >= 247.5 && angle <= 292.5))
        {
            return SnapDirection.Vertical;
        }

        return SnapDirection.Diagonal;
    }

    private static void AddCirclePoints(
        List<MapCellCoord> cells,
        HashSet<MapCellCoord> seen,
        int cx,
        int cz,
        int x,
        int z,
        int y)
    {
        AddUnique(cells, seen, new MapCellCoord(cx + x, y, cz + z));
        AddUnique(cells, seen, new MapCellCoord(cx - x, y, cz + z));
        AddUnique(cells, seen, new MapCellCoord(cx + x, y, cz - z));
        AddUnique(cells, seen, new MapCellCoord(cx - x, y, cz - z));

        AddUnique(cells, seen, new MapCellCoord(cx + z, y, cz + x));
        AddUnique(cells, seen, new MapCellCoord(cx - z, y, cz + x));
        AddUnique(cells, seen, new MapCellCoord(cx + z, y, cz - x));
        AddUnique(cells, seen, new MapCellCoord(cx - z, y, cz - x));
    }

    private static void AddUnique(
        List<MapCellCoord> cells,
        HashSet<MapCellCoord> seen,
        MapCellCoord cell)
    {
        if (seen.Add(cell))
        {
            cells.Add(cell);
        }
    }

    private static MapCellCoord WithStartingY(MapCellCoord startCell, MapCellCoord currentCell)
    {
        return new MapCellCoord(currentCell.X, startCell.Y, currentCell.Z);
    }

    #endregion
}
