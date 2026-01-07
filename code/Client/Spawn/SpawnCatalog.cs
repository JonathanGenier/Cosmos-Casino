using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Immutable runtime catalog mapping spawn identifiers to spawnable
/// <see cref="PackedScene"/> instances.
/// The <see cref="SpawnCatalog"/> is loaded once during loading
/// and treated as read-only for the lifetime of the gameplay session.
/// This catalog contains no world state and performs no spawning
/// logic. It exists solely to resolve spawn identifiers into
/// concrete scene assets for use by <c>SpawnManager</c>.
/// </summary>
public sealed class SpawnCatalog
{
    #region Fields

    private readonly Dictionary<string, PackedScene> _spawnScenes;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new <see cref="SpawnCatalog"/> with the specified
    /// spawn scene entries.
    /// </summary>
    /// <param name="spawnScenes">
    /// Preloaded mapping of spawn identifiers to packed scenes.
    /// </param>
    private SpawnCatalog(Dictionary<string, PackedScene> spawnScenes)
    {
        _spawnScenes = new Dictionary<string, PackedScene>(spawnScenes);
    }

    #endregion

    #region OnLoad Methods

    /// <summary>
    /// Creates a <see cref="SpawnCatalog"/> from a Godot
    /// <see cref="ResourcePreloader"/> node.
    /// All resources contained in the preloader must be
    /// <see cref="PackedScene"/> instances. Any invalid
    /// resource will cause loading to fail immediately.
    /// </summary>
    /// <param name="preloader">
    /// Resource preloader containing spawnable scenes indexed
    /// by spawn identifier.
    /// </param>
    /// <returns>
    /// An initialized <see cref="SpawnCatalog"/> containing
    /// all preloaded spawn scenes.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="preloader"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a resource in the preloader is not a
    /// <see cref="PackedScene"/>.
    /// </exception>
    public static SpawnCatalog LoadFromResourcePreloader(ResourcePreloader preloader)
    {
        ArgumentNullException.ThrowIfNull(preloader, nameof(preloader));

        var spawnScenes = new Dictionary<string, PackedScene>();

        foreach (string key in preloader.GetResourceList())
        {
            var resource = preloader.GetResource(key);

            if (resource is not PackedScene scene)
            {
                throw new InvalidOperationException($"Resource '{key}' is not a PackedScene.");
            }

            spawnScenes.Add(key, scene);
        }

        return new SpawnCatalog(spawnScenes);
    }

    #endregion

    #region SpawnScene Methods

    /// <summary>
    /// Resolves a spawn identifier to its associated
    /// <see cref="PackedScene"/>.
    /// </summary>
    /// <param name="spawnId">
    /// The unique identifier of the spawnable scene.
    /// </param>
    /// <returns>
    /// The <see cref="PackedScene"/> associated with the specified spawn id.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the specified spawn id is not registered in the catalog.
    /// </exception>
    public PackedScene GetSpawnScene(string spawnId)
    {
        if (!_spawnScenes.TryGetValue(spawnId, out var scene))
        {
            throw new KeyNotFoundException(
                $"SpawnId '{spawnId}' is not registered.");
        }

        return scene;
    }

    #endregion
}