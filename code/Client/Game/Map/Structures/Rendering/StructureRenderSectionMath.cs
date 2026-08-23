using CosmosCasino.Core.Game.Map;
using Godot;

/// <summary>
/// Client-owned partition math for generated structure render sections.
/// </summary>
internal static class StructureRenderSectionMath
{
    #region Section Coordinates

    /// <summary>
    /// Resolves the render section owning the specified global logical cell.
    /// </summary>
    /// <param name="cell">The global logical map cell.</param>
    /// <returns>The Client render section coordinate.</returns>
    internal static StructureRenderSectionCoord ToSectionCoord(MapCellCoord cell)
    {
        return new StructureRenderSectionCoord(
            FloorDivide(cell.X, StructureRenderSectionMetrics.SizeX),
            FloorDivide(cell.Y, StructureRenderSectionMetrics.SizeY),
            FloorDivide(cell.Z, StructureRenderSectionMetrics.SizeZ));
    }

    /// <summary>
    /// Gets the inclusive global logical cell bounds for a render section.
    /// </summary>
    /// <param name="coord">The render section coordinate.</param>
    /// <returns>The section bounds in global logical cell space.</returns>
    internal static StructureRenderSectionBounds GetBounds(StructureRenderSectionCoord coord)
    {
        return new StructureRenderSectionBounds(
            new MapCellCoord(
                GetSectionMin(coord.X, StructureRenderSectionMetrics.SizeX),
                GetSectionMin(coord.Y, StructureRenderSectionMetrics.SizeY),
                GetSectionMin(coord.Z, StructureRenderSectionMetrics.SizeZ)),
            new MapCellCoord(
                GetSectionMax(coord.X, StructureRenderSectionMetrics.SizeX),
                GetSectionMax(coord.Y, StructureRenderSectionMetrics.SizeY),
                GetSectionMax(coord.Z, StructureRenderSectionMetrics.SizeZ)));
    }

    /// <summary>
    /// Expands section bounds by a number of global logical cells with saturation at integer limits.
    /// </summary>
    /// <param name="bounds">The source bounds.</param>
    /// <param name="amount">The number of cells to expand in every direction.</param>
    /// <returns>The expanded bounds.</returns>
    internal static StructureRenderSectionBounds Expand(StructureRenderSectionBounds bounds, int amount)
    {
        return new StructureRenderSectionBounds(
            new MapCellCoord(
                ClampToInt((long)bounds.Min.X - amount),
                ClampToInt((long)bounds.Min.Y - amount),
                ClampToInt((long)bounds.Min.Z - amount)),
            new MapCellCoord(
                ClampToInt((long)bounds.Max.X + amount),
                ClampToInt((long)bounds.Max.Y + amount),
                ClampToInt((long)bounds.Max.Z + amount)));
    }

    /// <summary>
    /// Gets the Godot world-space origin for a render section.
    /// </summary>
    /// <param name="coord">The render section coordinate.</param>
    /// <returns>The world-space center of the section's minimum global cell.</returns>
    internal static Vector3 ToSectionWorldOrigin(StructureRenderSectionCoord coord)
    {
        return GetBounds(coord).Min.ToGodotCenter();
    }

    #endregion

    #region Offsets

    /// <summary>
    /// Adds a logical offset to a global cell without wrapping integer limits.
    /// </summary>
    /// <param name="cell">The source global cell.</param>
    /// <param name="offset">The logical offset to add.</param>
    /// <param name="result">The resolved global cell when representable.</param>
    /// <returns><c>true</c> when the result can be represented; otherwise, <c>false</c>.</returns>
    internal static bool TryAddOffset(MapCellCoord cell, MapCellOffset offset, out MapCellCoord result)
    {
        long x = (long)cell.X + offset.X;
        long y = (long)cell.Y + offset.Y;
        long z = (long)cell.Z + offset.Z;

        if (!CanRepresentAsInt(x) || !CanRepresentAsInt(y) || !CanRepresentAsInt(z))
        {
            result = default;
            return false;
        }

        result = new MapCellCoord((int)x, (int)y, (int)z);
        return true;
    }

    #endregion

    #region Helpers

    private static int FloorDivide(int value, int divisor)
    {
        int quotient = value / divisor;
        int remainder = value % divisor;

        if (remainder != 0 && value < 0)
        {
            quotient--;
        }

        return quotient;
    }

    private static int GetSectionMin(int sectionAxis, int size)
    {
        return ClampToInt((long)sectionAxis * size);
    }

    private static int GetSectionMax(int sectionAxis, int size)
    {
        return ClampToInt((((long)sectionAxis + 1) * size) - 1);
    }

    private static bool CanRepresentAsInt(long value)
    {
        return value >= int.MinValue && value <= int.MaxValue;
    }

    private static int ClampToInt(long value)
    {
        if (value < int.MinValue)
        {
            return int.MinValue;
        }

        if (value > int.MaxValue)
        {
            return int.MaxValue;
        }

        return (int)value;
    }

    #endregion
}
