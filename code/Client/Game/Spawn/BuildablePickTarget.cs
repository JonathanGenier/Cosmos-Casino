using Godot;
using System;

/// <summary>
/// Client-only pick identity attached to spawned buildable collision objects.
/// </summary>
internal sealed partial class BuildablePickTarget : Node
{
    #region Fields

    private CellSlotSpawnKey _spawnKey;
    private bool _hasSpawnKey;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the logical spawn identity represented by the owning buildable collider.
    /// </summary>
    internal CellSlotSpawnKey SpawnKey
    {
        get
        {
            if (!_hasSpawnKey)
            {
                throw new InvalidOperationException($"{nameof(BuildablePickTarget)} has not been initialized.");
            }

            return _spawnKey;
        }
    }

    #endregion

    #region Resolution

    /// <summary>
    /// Attempts to find a buildable pick identity attached directly to the specified collider.
    /// </summary>
    /// <param name="collider">The physics collider returned by a raycast.</param>
    /// <param name="spawnKey">When this method returns, contains the resolved spawn key if one was found.</param>
    /// <returns><see langword="true"/> if a buildable pick target was found; otherwise, <see langword="false"/>.</returns>
    internal static bool TryFind(CollisionObject3D collider, out CellSlotSpawnKey spawnKey)
    {
        foreach (Node child in collider.GetChildren())
        {
            if (child is BuildablePickTarget target && target.TryGetSpawnKey(out spawnKey))
            {
                return true;
            }
        }

        spawnKey = default;
        return false;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the pick target with the spawned buildable identity.
    /// </summary>
    /// <param name="spawnKey">The spawned buildable identity.</param>
    /// <exception cref="InvalidOperationException">Thrown if the pick target has already been initialized.</exception>
    internal void Initialize(CellSlotSpawnKey spawnKey)
    {
        if (_hasSpawnKey)
        {
            throw new InvalidOperationException($"{nameof(BuildablePickTarget)} is already initialized.");
        }

        _spawnKey = spawnKey;
        _hasSpawnKey = true;
    }

    private bool TryGetSpawnKey(out CellSlotSpawnKey spawnKey)
    {
        if (!_hasSpawnKey)
        {
            spawnKey = default;
            return false;
        }

        spawnKey = _spawnKey;
        return true;
    }

    #endregion
}
