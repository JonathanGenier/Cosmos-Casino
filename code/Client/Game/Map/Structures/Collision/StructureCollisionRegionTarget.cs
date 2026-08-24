using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Client-only pick identity attached to a generated structure collision region body.
/// </summary>
internal sealed partial class StructureCollisionRegionTarget : Node
{
    #region Fields

    private StructureCollisionRegionView? _regionView;

    #endregion

    #region Resolution

    /// <summary>
    /// Attempts to resolve generated structure collision metadata attached directly to the specified collider.
    /// </summary>
    /// <param name="collider">The physics collider returned by a raycast.</param>
    /// <param name="hit">The complete physics hit returned by the raycast.</param>
    /// <param name="cell">The resolved global occupied structure cell.</param>
    /// <param name="face">The resolved logical structure face.</param>
    /// <returns><see langword="true"/> when region metadata was found and resolved; otherwise, <see langword="false"/>.</returns>
    internal static bool TryResolve(
        CollisionObject3D collider,
        CursorPhysicsHit hit,
        out MapCellCoord cell,
        out CursorSurfaceFace face)
    {
        foreach (Node child in collider.GetChildren())
        {
            if (child is StructureCollisionRegionTarget target
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
    /// Initializes this marker with its owning collision region view.
    /// </summary>
    /// <param name="regionView">The owning collision region view.</param>
    internal void Initialize(StructureCollisionRegionView regionView)
    {
        if (_regionView != null)
        {
            throw new InvalidOperationException($"{nameof(StructureCollisionRegionTarget)} is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(regionView);

        _regionView = regionView;
    }

    private bool TryResolveHit(
        CursorPhysicsHit hit,
        out MapCellCoord cell,
        out CursorSurfaceFace face)
    {
        if (_regionView == null)
        {
            cell = default;
            face = default;
            return false;
        }

        return _regionView.TryResolveHit(hit, out cell, out face);
    }

    #endregion
}
