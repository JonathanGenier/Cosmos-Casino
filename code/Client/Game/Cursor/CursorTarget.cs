using CosmosCasino.Core.Game.Map;
using System;

/// <summary>
/// Immutable logical target currently under the cursor.
/// </summary>
public readonly struct CursorTarget : IEquatable<CursorTarget>
{
    #region Initialization

    private CursorTarget(
        CursorTargetKind kind,
        MapCellCoord targetCell,
        MapCellCoord placementCell,
        StructureId? structureId,
        CursorSurfaceFace? surfaceFace)
    {
        Kind = kind;
        TargetCell = targetCell;
        PlacementCell = placementCell;
        StructureId = structureId;
        SurfaceFace = surfaceFace;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the logical target kind.
    /// </summary>
    public CursorTargetKind Kind { get; }

    /// <summary>
    /// Gets the authoritative cell directly targeted by the cursor.
    /// </summary>
    public MapCellCoord TargetCell { get; }

    /// <summary>
    /// Gets the authoritative candidate cell used for new structure placement.
    /// </summary>
    public MapCellCoord PlacementCell { get; }

    /// <summary>
    /// Gets the authoritative structure identity when the target is a structure.
    /// </summary>
    public StructureId? StructureId { get; }

    /// <summary>
    /// Gets the logical surface face when the target is a structure surface.
    /// </summary>
    public CursorSurfaceFace? SurfaceFace { get; }

    #endregion

    #region Equality Operators

    /// <summary>
    /// Determines whether two cursor targets are equal.
    /// </summary>
    /// <param name="left">The first target.</param>
    /// <param name="right">The second target.</param>
    /// <returns><see langword="true"/> if the targets are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(CursorTarget left, CursorTarget right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two cursor targets are not equal.
    /// </summary>
    /// <param name="left">The first target.</param>
    /// <param name="right">The second target.</param>
    /// <returns><see langword="true"/> if the targets are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(CursorTarget left, CursorTarget right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Factories

    /// <summary>
    /// Creates a terrain cursor target.
    /// </summary>
    /// <param name="targetCell">The authoritative terrain surface cell.</param>
    /// <returns>A terrain cursor target.</returns>
    public static CursorTarget Terrain(MapCellCoord targetCell)
    {
        return new CursorTarget(
            CursorTargetKind.Terrain,
            targetCell,
            targetCell,
            null,
            null);
    }

    /// <summary>
    /// Creates a structure cursor target.
    /// </summary>
    /// <param name="targetCell">The authoritative occupied structure cell hit by the cursor.</param>
    /// <param name="placementCell">The adjacent candidate cell derived from the hit face.</param>
    /// <param name="structureId">The authoritative structure identity occupying <paramref name="targetCell"/>.</param>
    /// <param name="surfaceFace">The logical face hit by the cursor.</param>
    /// <returns>A structure cursor target.</returns>
    public static CursorTarget Structure(
        MapCellCoord targetCell,
        MapCellCoord placementCell,
        StructureId structureId,
        CursorSurfaceFace surfaceFace)
    {
        return new CursorTarget(
            CursorTargetKind.Structure,
            targetCell,
            placementCell,
            structureId,
            surfaceFace);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Determines whether this target equals another target.
    /// </summary>
    /// <param name="other">The target to compare against.</param>
    /// <returns><see langword="true"/> if the targets are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(CursorTarget other)
    {
        return Kind == other.Kind
            && TargetCell.Equals(other.TargetCell)
            && PlacementCell.Equals(other.PlacementCell)
            && Nullable.Equals(StructureId, other.StructureId)
            && SurfaceFace == other.SurfaceFace;
    }

    /// <summary>
    /// Determines whether this target equals the specified object.
    /// </summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns><see langword="true"/> if the object is an equal target; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is CursorTarget other && Equals(other);
    }

    /// <summary>
    /// Gets a hash code for this target.
    /// </summary>
    /// <returns>A hash code based on every logical field that affects placement.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Kind, TargetCell, PlacementCell, StructureId, SurfaceFace);
    }

    #endregion
}
