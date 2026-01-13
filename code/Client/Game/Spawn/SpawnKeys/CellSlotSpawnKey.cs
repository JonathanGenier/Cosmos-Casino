using CosmosCasino.Core.Game.Map.Cell;
using System;

/// <summary>
/// Represents a unique key identifying a specific logical slot within a map cell, used to distinguish spawned entities
/// by their cell and slot location.
/// </summary>
/// <remarks>A CellSlotSpawnKey combines a map cell coordinate with a logical slot (such as Floor, Structure, or
/// Furniture) to uniquely identify where an entity is spawned within a map. This struct is typically used as a key in
/// collections or systems that need to track or reference entities by their cell and slot. CellSlotSpawnKey supports
/// value equality and can be used in hash-based collections.</remarks>
public readonly struct CellSlotSpawnKey : IEquatable<CellSlotSpawnKey>, ISpawnKey
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CellSlotSpawnKey structure with the specified cell coordinates and slot.
    /// </summary>
    /// <param name="coord">The coordinates of the map cell to associate with this key.</param>
    /// <param name="slot">The slot within the specified map cell to associate with this key.</param>
    public CellSlotSpawnKey(MapCellCoord coord, MapCellSlot slot)
    {
        Coord = coord;
        Slot = slot;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the coordinates of this map cell.
    /// </summary>
    public MapCellCoord Coord { get; }

    /// <summary>
    /// Gets the slot information associated with this map cell.
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