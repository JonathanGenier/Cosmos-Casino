using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Resolves physical cursor hits and fallback positions into logical cursor targets.
/// </summary>
internal sealed class CursorTargetResolver
{
    #region Fields

    private readonly MapManager _mapManager;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new cursor target resolver.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager used for terrain elevation lookup.</param>
    public CursorTargetResolver(MapManager mapManager)
    {
        ArgumentNullException.ThrowIfNull(mapManager);
        _mapManager = mapManager;
    }

    #endregion

    #region Resolver

    /// <summary>
    /// Attempts to resolve a physical physics hit into a logical cursor target.
    /// </summary>
    /// <param name="hit">The physical hit returned by Godot physics.</param>
    /// <param name="target">When this method returns, contains the logical cursor target if one was resolved.</param>
    /// <returns><see langword="true"/> if a logical target was resolved; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a buildable collider has no buildable pick identity.</exception>
    public bool TryResolve(CursorPhysicsHit hit, out CursorTarget target)
    {
        if (IsInLayer(hit.Collider, CollisionLayers.Terrain))
        {
            return TryResolveTerrain(hit.WorldPosition, out target);
        }

        if (IsInLayer(hit.Collider, CollisionLayers.Buildable))
        {
            if (BuildablePickTarget.TryFind(hit.Collider, out var spawnKey))
            {
                target = CursorTarget.Buildable(spawnKey);
                return true;
            }

            throw new InvalidOperationException(
                $"Buildable collider '{hit.Collider.Name}' does not expose a {nameof(BuildablePickTarget)}.");
        }

        target = default;
        return false;
    }

    /// <summary>
    /// Attempts to resolve a fallback world position into a terrain cursor target.
    /// </summary>
    /// <param name="worldPosition">The fallback world position.</param>
    /// <param name="target">When this method returns, contains the terrain cursor target if one was resolved.</param>
    /// <returns><see langword="true"/> if a terrain target was resolved; otherwise, <see langword="false"/>.</returns>
    public bool TryResolveTerrain(Vector3 worldPosition, out CursorTarget target)
    {
        var coord = MapMath.WorldToCell(worldPosition.ToWorldCoord());

        if (!_mapManager.TryGetTerrainBaseElevation(coord, out var elevation))
        {
            target = default;
            return false;
        }

        target = CursorTarget.Terrain(coord, elevation);
        return true;
    }

    #endregion

    #region Helpers

    private static bool IsInLayer(CollisionObject3D collider, uint layer)
    {
        return (collider.CollisionLayer & layer) != 0;
    }

    #endregion
}
