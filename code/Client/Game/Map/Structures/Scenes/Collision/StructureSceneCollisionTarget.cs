using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Client-only pick identity attached to one scene-rendered structure collision proxy.
/// </summary>
internal sealed partial class StructureSceneCollisionTarget : Node
{
    #region Fields

    private MapCellCoord _occupiedCell;
    private bool _isInitialized;

    #endregion

    #region Resolution

    /// <summary>
    /// Attempts to resolve scene-rendered structure collision metadata attached directly to the specified collider.
    /// </summary>
    /// <param name="collider">The physics collider returned by a raycast.</param>
    /// <param name="hit">The complete physics hit returned by the raycast.</param>
    /// <param name="cell">The resolved global occupied structure cell.</param>
    /// <param name="face">The resolved logical structure face.</param>
    /// <returns><see langword="true"/> when proxy metadata was found and resolved; otherwise, <see langword="false"/>.</returns>
    internal static bool TryResolve(
        CollisionObject3D collider,
        CursorPhysicsHit hit,
        out MapCellCoord cell,
        out CursorSurfaceFace face)
    {
        foreach (Node child in collider.GetChildren())
        {
            if (child is StructureSceneCollisionTarget target
                && target.TryResolveHit(hit, out cell, out face))
            {
                return true;
            }
        }

        cell = default;
        face = default;
        return false;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes this marker with the occupied logical cell represented by its collider.
    /// </summary>
    /// <param name="occupiedCell">The occupied logical cell represented by the collider.</param>
    internal void Initialize(MapCellCoord occupiedCell)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(StructureSceneCollisionTarget)} is already initialized.");
        }

        _occupiedCell = occupiedCell;
        _isInitialized = true;
    }

    private bool TryResolveHit(
        CursorPhysicsHit hit,
        out MapCellCoord cell,
        out CursorSurfaceFace face)
    {
        if (!_isInitialized || !CursorSurfaceFaceResolver.TryResolve(hit.WorldNormal, out face))
        {
            cell = default;
            face = default;
            return false;
        }

        cell = _occupiedCell;
        return true;
    }

    #endregion
}
