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
        MapCoord coord,
        Elevation elevation,
        CellSlot? slot)
    {
        Kind = kind;
        Coord = coord;
        Elevation = elevation;
        Slot = slot;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the logical target kind.
    /// </summary>
    public CursorTargetKind Kind { get; }

    /// <summary>
    /// Gets the authoritative target map coordinate.
    /// </summary>
    public MapCoord Coord { get; }

    /// <summary>
    /// Gets the authoritative target elevation.
    /// </summary>
    public Elevation Elevation { get; }

    /// <summary>
    /// Gets the target cell slot when the target is a buildable.
    /// </summary>
    public CellSlot? Slot { get; }

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
    /// <param name="coord">The terrain map coordinate.</param>
    /// <param name="elevation">The authoritative terrain base elevation.</param>
    /// <returns>A terrain cursor target.</returns>
    public static CursorTarget Terrain(MapCoord coord, Elevation elevation)
    {
        return new CursorTarget(CursorTargetKind.Terrain, coord, elevation, null);
    }

    /// <summary>
    /// Creates a buildable cursor target from a spawned cell-slot identity.
    /// </summary>
    /// <param name="spawnKey">The spawned buildable identity.</param>
    /// <returns>A buildable cursor target.</returns>
    public static CursorTarget Buildable(CellSlotSpawnKey spawnKey)
    {
        return new CursorTarget(
            CursorTargetKind.Buildable,
            spawnKey.Coord,
            spawnKey.Elevation,
            spawnKey.Slot);
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
            && Coord.Equals(other.Coord)
            && Elevation.Equals(other.Elevation)
            && Slot == other.Slot;
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
    /// <returns>A hash code based on the target kind, coordinate, elevation, and slot.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Kind, Coord, Elevation, Slot);
    }

    #endregion
}
