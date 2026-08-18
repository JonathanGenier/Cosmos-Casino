using Godot;
using System;

/// <summary>
/// Client-only pick identity attached to spawned buildable collision objects.
/// </summary>
public sealed partial class BuildablePickTarget : Node
{
    #region Fields

    private CellSlotSpawnKey _spawnKey;
    private bool _hasSpawnKey;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the logical spawn identity represented by the owning buildable collider.
    /// </summary>
    public CellSlotSpawnKey SpawnKey
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
    /// Attempts to find a buildable pick identity on or above the specified node.
    /// </summary>
    /// <param name="node">The node to start from, typically the physics collider returned by a raycast.</param>
    /// <param name="spawnKey">When this method returns, contains the resolved spawn key if one was found.</param>
    /// <returns><see langword="true"/> if a buildable pick target was found; otherwise, <see langword="false"/>.</returns>
    public static bool TryFind(Node node, out CellSlotSpawnKey spawnKey)
    {
        for (Node? current = node; current != null; current = current.GetParent())
        {
            if (current is BuildablePickTarget target && target.TryGetSpawnKey(out spawnKey))
            {
                return true;
            }

            foreach (Node child in current.GetChildren())
            {
                if (child is BuildablePickTarget childTarget && childTarget.TryGetSpawnKey(out spawnKey))
                {
                    return true;
                }
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
    public void Initialize(CellSlotSpawnKey spawnKey)
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
