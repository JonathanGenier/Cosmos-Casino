using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Resolves Client-only cursor surface face data from Godot physics information.
/// </summary>
internal static class CursorSurfaceFaceResolver
{
    #region Resolution

    /// <summary>
    /// Attempts to resolve a logical face from a Godot world-space normal using the dominant axis.
    /// </summary>
    /// <param name="worldNormal">The world-space normal reported by physics.</param>
    /// <param name="face">The resolved logical face.</param>
    /// <returns><c>true</c> when the normal can be mapped to a face; otherwise, <c>false</c>.</returns>
    internal static bool TryResolve(Vector3 worldNormal, out CursorSurfaceFace face)
    {
        if (!float.IsFinite(worldNormal.X)
            || !float.IsFinite(worldNormal.Y)
            || !float.IsFinite(worldNormal.Z))
        {
            face = default;
            return false;
        }

        float absX = MathF.Abs(worldNormal.X);
        float absY = MathF.Abs(worldNormal.Y);
        float absZ = MathF.Abs(worldNormal.Z);

        if (absX == 0f && absY == 0f && absZ == 0f)
        {
            face = default;
            return false;
        }

        if (absY >= absX && absY >= absZ)
        {
            face = worldNormal.Y >= 0f ? CursorSurfaceFace.Top : CursorSurfaceFace.Bottom;
            return true;
        }

        if (absX >= absZ)
        {
            face = worldNormal.X >= 0f ? CursorSurfaceFace.East : CursorSurfaceFace.West;
            return true;
        }

        face = worldNormal.Z >= 0f ? CursorSurfaceFace.South : CursorSurfaceFace.North;
        return true;
    }

    /// <summary>
    /// Gets the logical neighboring-cell offset represented by a face.
    /// </summary>
    /// <param name="face">The logical face.</param>
    /// <returns>The adjacent map-cell offset for placement from that face.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="face"/> is unsupported.</exception>
    internal static MapCellOffset GetOffset(CursorSurfaceFace face)
    {
        return face switch
        {
            CursorSurfaceFace.Top => new MapCellOffset(0, 1, 0),
            CursorSurfaceFace.Bottom => new MapCellOffset(0, -1, 0),
            CursorSurfaceFace.North => new MapCellOffset(0, 0, -1),
            CursorSurfaceFace.South => new MapCellOffset(0, 0, 1),
            CursorSurfaceFace.East => new MapCellOffset(1, 0, 0),
            CursorSurfaceFace.West => new MapCellOffset(-1, 0, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(face), face, "Unsupported cursor surface face.")
        };
    }

    /// <summary>
    /// Attempts to apply a face offset to a map-cell coordinate without overflowing.
    /// </summary>
    /// <param name="cell">The occupied target cell.</param>
    /// <param name="offset">The face-derived neighboring-cell offset.</param>
    /// <param name="result">The adjacent placement cell, when representable.</param>
    /// <returns><c>true</c> when the adjacent cell can be represented; otherwise, <c>false</c>.</returns>
    internal static bool TryAddOffset(
        MapCellCoord cell,
        MapCellOffset offset,
        out MapCellCoord result)
    {
        if (!TryAddAxis(cell.X, offset.X, out int x)
            || !TryAddAxis(cell.Y, offset.Y, out int y)
            || !TryAddAxis(cell.Z, offset.Z, out int z))
        {
            result = default;
            return false;
        }

        result = new MapCellCoord(x, y, z);
        return true;
    }

    #endregion

    #region Helpers

    private static bool TryAddAxis(int cellAxis, int offsetAxis, out int result)
    {
        long value = (long)cellAxis + offsetAxis;

        if (value < int.MinValue || value > int.MaxValue)
        {
            result = default;
            return false;
        }

        result = (int)value;
        return true;
    }

    #endregion
}
