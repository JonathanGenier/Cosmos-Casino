using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Maintains a mapping between logical spawn keys and live
/// Godot instance identifiers for spawned visuals.
/// </summary>
public sealed class SpawnRegistry
{
    #region Fields

    /// <summary>
    /// Internal mapping of spawn keys to Godot instance IDs.
    /// </summary>
    private readonly Dictionary<ISpawnKey, ulong> _entries = new();

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of active spawn registrations.
    /// </summary>
    public int Count => _entries.Count;

    #endregion

    #region Get Methods

    /// <summary>
    /// Creates a snapshot of all registered instance IDs.
    /// </summary>
    /// <returns>
    /// Read-only list containing the instance IDs of all spawned visuals.
    /// </returns>
    public IReadOnlyList<ulong> SnapshotIds()
    {
        return _entries.Values.ToList();
    }

    /// <summary>
    /// Attempts to retrieve the instance ID associated with the specified spawn key.
    /// </summary>
    /// <param name="key">
    /// Logical spawn key to look up.
    /// </param>
    /// <param name="objectId">
    /// When this method returns, contains the instance ID associated with the key
    /// if the key was found; otherwise, zero.
    /// </param>
    /// <returns>
    /// <c>true</c> if the spawn key exists; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGet(ISpawnKey key, out ulong objectId)
    {
        return _entries.TryGetValue(key, out objectId);
    }

    #endregion

    #region Add & Remove

    /// <summary>
    /// Registers a new spawn key and its associated instance ID.
    /// </summary>
    /// <param name="key">
    /// Logical spawn key identifying the visual.
    /// </param>
    /// <param name="objectId">
    /// Godot instance ID of the spawned visual.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// Thrown if a spawn is already registered for the specified key.
    /// </exception>
    public void Add(ISpawnKey key, ulong objectId)
    {
        if (_entries.ContainsKey(key))
        {
            throw new System.ArgumentException(
                $"Spawn already registered for key: {key}");
        }

        _entries[key] = objectId;
    }

    /// <summary>
    /// Removes the spawn registration associated with the specified key.
    /// </summary>
    /// <param name="key">
    /// Logical spawn key to remove.
    /// </param>
    /// <returns>
    /// <c>true</c> if the key was found and removed; otherwise, <c>false</c>.
    /// </returns>
    public bool Remove(ISpawnKey key)
    {
        return _entries.Remove(key);
    }

    /// <summary>
    /// Removes all spawn registrations from the registry.
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
    }

    #endregion
}
