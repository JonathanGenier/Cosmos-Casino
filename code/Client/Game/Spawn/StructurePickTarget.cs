using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Client-only pick identity attached to structure collision objects.
/// </summary>
internal sealed partial class StructurePickTarget : Node
{
    #region Fields

    private MapCellCoord _occupiedCell;
    private bool _hasOccupiedCell;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authoritative occupied cell represented by the owning structure collider.
    /// </summary>
    internal MapCellCoord OccupiedCell
    {
        get
        {
            if (!_hasOccupiedCell)
            {
                throw new InvalidOperationException($"{nameof(StructurePickTarget)} has not been initialized.");
            }

            return _occupiedCell;
        }
    }

    #endregion

    #region Resolution

    /// <summary>
    /// Attempts to find structure pick metadata attached directly to the specified collider.
    /// </summary>
    /// <param name="collider">The physics collider returned by a raycast.</param>
    /// <param name="occupiedCell">The occupied map cell represented by the collider.</param>
    /// <returns><see langword="true"/> if structure pick metadata was found; otherwise, <see langword="false"/>.</returns>
    internal static bool TryFind(CollisionObject3D collider, out MapCellCoord occupiedCell)
    {
        foreach (Node child in collider.GetChildren())
        {
            if (child is StructurePickTarget target && target.TryGetOccupiedCell(out occupiedCell))
            {
                return true;
            }
        }

        occupiedCell = default;
        return false;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the pick target with the occupied structure cell represented by the owning collider.
    /// </summary>
    /// <param name="occupiedCell">The authoritative occupied map cell represented by the collider.</param>
    /// <exception cref="InvalidOperationException">Thrown if the pick target has already been initialized.</exception>
    internal void Initialize(MapCellCoord occupiedCell)
    {
        if (_hasOccupiedCell)
        {
            throw new InvalidOperationException($"{nameof(StructurePickTarget)} is already initialized.");
        }

        _occupiedCell = occupiedCell;
        _hasOccupiedCell = true;
    }

    private bool TryGetOccupiedCell(out MapCellCoord occupiedCell)
    {
        if (!_hasOccupiedCell)
        {
            occupiedCell = default;
            return false;
        }

        occupiedCell = _occupiedCell;
        return true;
    }

    #endregion
}
