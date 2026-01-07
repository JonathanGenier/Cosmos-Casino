using CosmosCasino.Core.Map;
using CosmosCasino.Core.Map.Cell;
using System;

/// <summary>
/// Immutable spawn identity representing a specific logical slot
/// within a map cell.
/// A <see cref="CellSlotSpawnKey"/> uniquely identifies a single
/// visual entity occupying a given <see cref="MapCellCoord"/> and
/// <see cref="MapCellSlot"/> combination.
/// This key represents identity only and intentionally contains
/// no variant or visual information. Visual appearance is resolved
/// separately via spawn variants and the spawn resolver.
/// </summary>
public readonly struct CellSlotSpawnKey : IEquatable<CellSlotSpawnKey>, ISpawnKey
{
    #region Constructor

    /// <summary>
    /// Creates a spawn key identifying a specific slot within a map cell.
    /// </summary>
    /// <param name="coord">
    /// The logical map cell coordinate associated with this spawn key.
    /// </param>
    /// <param name="slot">
    /// The logical slot within the cell (e.g. Floor, Structure, Furniture).
    /// </param>
    public CellSlotSpawnKey(MapCellCoord coord, MapCellSlot slot)
    {
        Coord = coord;
        Slot = slot;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The logical map cell coordinate this spawn key refers to.
    /// </summary>
    public MapCellCoord Coord { get; }

    /// <summary>
    /// The logical slot within the cell occupied by the spawned entity.
    /// </summary>
    public MapCellSlot Slot { get; }

    #endregion

    #region Equality Operators

    /// <summary>
    /// Determines whether two spawn keys represent the same cell slot.
    /// </summary>
    /// <param name="left">
    /// The first spawn key to compare.
    /// </param>
    /// <param name="right">
    /// The second spawn key to compare.
    /// </param>
    /// <returns>
    /// <c>true</c> if both spawn keys refer to the same cell coordinate
    /// and slot; otherwise <c>false</c>.
    /// </returns>
    public static bool operator ==(CellSlotSpawnKey left, CellSlotSpawnKey right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two spawn keys represent different cell slots.
    /// </summary>
    /// <param name="left">
    /// The first spawn key to compare.
    /// </param>
    /// <param name="right">
    /// The second spawn key to compare.
    /// </param>
    /// <returns>
    /// <c>true</c> if the spawn keys do not refer to the same cell
    /// coordinate and slot; otherwise <c>false</c>.
    /// </returns>
    public static bool operator !=(CellSlotSpawnKey left, CellSlotSpawnKey right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Determines whether this spawn key is equal to another
    /// <see cref="CellSlotSpawnKey"/>.
    /// </summary>
    /// <param name="other">
    /// The spawn key to compare against.
    /// </param>
    /// <returns>
    /// <c>true</c> if both keys refer to the same cell coordinate
    /// and slot; otherwise <c>false</c>.
    /// </returns>
    public bool Equals(CellSlotSpawnKey other)
    {
        return Coord.Equals(other.Coord) && Slot == other.Slot;
    }

    /// <summary>
    /// Determines whether this spawn key is equal to the specified object.
    /// </summary>
    /// <param name="obj">
    /// The object to compare against.
    /// </param>
    /// <returns>
    /// <c>true</c> if the object is a <see cref="CellSlotSpawnKey"/>
    /// with identical cell and slot values; otherwise <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
        return obj is CellSlotSpawnKey other && Equals(other);
    }

    #endregion

    #region Hashcode

    /// <summary>
    /// Computes a hash code suitable for use in hash-based collections.
    /// </summary>
    /// <returns>
    /// A hash code derived from the cell coordinate and slot.
    /// </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = Coord.GetHashCode();
            hash = (hash * 397) ^ (int)Slot;
            return hash;
        }
    }

    #endregion

    #region Debug

    /// <summary>
    /// Returns a human-readable string representation of this spawn key.
    /// </summary>
    /// <returns>
    /// A string describing the cell coordinate and slot.
    /// </returns>
    public override string ToString()
    {
        return $"{Coord} [{Slot}]";
    }

    #endregion
}