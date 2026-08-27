using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Resolves structure collision hits to global logical cells when triangle metadata is unavailable.
/// </summary>
internal static class StructureCollisionHitResolver
{
    #region Constants

    private const float SurfaceBiasEpsilon = WorldGridMetrics.VerticalGridUnitSize * 0.02f;

    #endregion

    #region Resolution

    /// <summary>
    /// Resolves the occupied cell by biasing the hit point slightly into the hit solid before grid conversion.
    /// </summary>
    /// <param name="worldPosition">The Godot world-space hit position.</param>
    /// <param name="worldNormal">The Godot world-space surface normal.</param>
    /// <param name="cell">The resolved global occupied cell.</param>
    /// <param name="face">The resolved logical surface face.</param>
    /// <returns><c>true</c> when the hit can be resolved to a representable cell and face.</returns>
    internal static bool TryResolve(
        Vector3 worldPosition,
        Vector3 worldNormal,
        out MapCellCoord cell,
        out CursorSurfaceFace face)
    {
        cell = default;
        face = default;

        if (!IsFinite(worldPosition)
            || !CursorSurfaceFaceResolver.TryResolve(worldNormal, out face))
        {
            return false;
        }

        if (worldNormal.LengthSquared() <= 0f)
        {
            face = default;
            return false;
        }

        Vector3 samplePoint = worldPosition - (worldNormal.Normalized() * SurfaceBiasEpsilon);
        MapCoord horizontalCell = MapMath.WorldToCell(samplePoint.ToWorldCoord());

        if (!TryResolveVerticalCell(samplePoint.Y, out int y))
        {
            face = default;
            return false;
        }

        cell = new MapCellCoord(horizontalCell.X, y, horizontalCell.Y);
        return true;
    }

    #endregion

    #region Helpers

    private static bool TryResolveVerticalCell(float worldY, out int cellY)
    {
        if (!float.IsFinite(worldY))
        {
            cellY = default;
            return false;
        }

        double scaled = ((double)worldY + (WorldGridMetrics.VerticalGridUnitSize * 0.5d))
            / WorldGridMetrics.VerticalGridUnitSize;
        double floored = Math.Floor(scaled);

        if (floored < int.MinValue || floored > int.MaxValue)
        {
            cellY = default;
            return false;
        }

        cellY = (int)floored;
        return true;
    }

    private static bool IsFinite(Vector3 vector)
    {
        return float.IsFinite(vector.X)
            && float.IsFinite(vector.Y)
            && float.IsFinite(vector.Z);
    }

    #endregion
}
